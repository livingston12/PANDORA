using Microsoft.EntityFrameworkCore;
using Pandora.Core.Attributes;
using Pandora.Core.Models.Entities;
using Pandora.Core.Models.Enums;
using Pandora.Core.Models.Requests;
using Pandora.Core.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Pandora.Core.Extensions
{
    public static class QueryableExtension
    {
        public static IQueryable<TSource> FilterList<TSource>(this IQueryable<TSource> source, string data, string field)
            where TSource : Entity
        {
            var result = source;
            if (data.IsNotNullOrEmpty())
            {
                var list = data.AsList();
                result = source.Where(s => list.Contains(s.GetType().GetProperty(field).GetValue(s, null)));
            }
            return result;
        }

        public static IQueryable<TSource> FilterList<TSource>(this IQueryable<TSource> source, IEnumerable<string> data, string field)
            where TSource : Entity
        {
            return source.Where(s => data.Contains(s.GetType().GetProperty(field).GetValue(s, null) as string));
        }

        public static IQueryable<TSource> OrderBy<TSource>(this IQueryable<TSource> source, string sorting)
            where TSource : Entity
        {
            Check.NotNull(source, nameof(source));
            var sourceType = typeof(TSource);
            var param = GetParameterExpression<TSource>();

            (PropertyInfo Property, SortDirection Direction)? sortInfo = null;

            if (string.IsNullOrEmpty(sorting))
            {
                sortInfo = GetSortInfo(sourceType);
            }
            else
            {
                sortInfo = GetSortInfo(sorting, sourceType);
            }

            if (HasSortInfo(sortInfo))
            {
                var sortExpression = Expression.Lambda<Func<TSource, object>>
                    (Expression.Convert(Expression.Property(param, sortInfo.Value.Property.Name), typeof(object)), param);
                source = sortInfo.Value.Direction == SortDirection.ASC ?
                            source.OrderBy(sortExpression) : source.OrderBy(sortExpression);
            }
            return source;
        }

        private static (PropertyInfo, SortDirection)? GetSortInfo(Type sourceType)
        {
            var attribute = sourceType
                                    .GetCustomAttributes(typeof(DefaultSortPropertyAttribute), true)
                                    .FirstOrDefault() as DefaultSortPropertyAttribute;
            (PropertyInfo, SortDirection)? sortInfo = null;
            if (attribute != null && attribute.Name.IsNotNullOrEmpty())
            {
                var propertyInfo = GetPropertyInfoByName(sourceType, attribute.Name);
                var direction = attribute.SortDirection;
                sortInfo = (propertyInfo, direction);
            }

            return sortInfo;
        }

        private static (PropertyInfo, SortDirection)? GetSortInfo(string sorting, Type sourceType)
        {
            (PropertyInfo, SortDirection)? sortInfo = null;
            var direction = GetDirection(sorting);
            var name = GetPropertyName(sorting);
            var property = GetPropertyInfoByName(sourceType, name);
            sortInfo = (property, direction);

            return sortInfo;
        }

        private static SortDirection GetDirection(string sorting)
        {
            SortDirection direction = SortDirection.ASC;

            if (sorting.EndsWith("_DESC", StringComparison.OrdinalIgnoreCase))
            {
                direction = SortDirection.DESC;
            }

            return direction;
        }

        private static string GetPropertyName(string sorting)
        {
            if (sorting.IsNotNullOrEmpty() && sorting.Contains("_"))
            {
                sorting = sorting.Substring(0, sorting.IndexOf("_", StringComparison.OrdinalIgnoreCase));
            }

            return sorting;
        }

        private static PropertyInfo GetPropertyInfoByName(Type sourceType, string propertyName)
        {
            return sourceType.GetProperties()
                .FirstOrDefault(e => e.Name.Equals(propertyName, StringComparison.OrdinalIgnoreCase)
                    && e.GetCustomAttribute(typeof(NotMappedAttribute)) == null);
        }

        private static bool HasSortInfo((PropertyInfo Property, SortDirection Direction)? sortInfo)
        {
            return sortInfo != null && sortInfo.Value.Property != null;
        }

        public static IQueryable<TSource> Where<TSource, TFilter>(this IQueryable<TSource> source, TFilter filter)
            where TSource : Entity
            where TFilter : GetRequest
        {
            Check.NotNull(source, nameof(source));
            var lambdas = HasValue(filter)
                            ? FilterBy<TSource, TFilter>(filter) : FilterBy<TSource>();
            foreach (var lambda in lambdas)
            {
                source = source.Where(lambda);
            }

            return source;
        }

        private static bool HasValue<TFilter>(TFilter filter) where TFilter : GetRequest
        {
            return typeof(TFilter).GetProperties()
                        .Where(f => !f.IsDefined(typeof(NotMappedAttribute)))
                        .Any(p => p.GetValue(filter) != null);
        }

        private static IEnumerable<Expression<Func<TSource, bool>>> FilterBy<TSource, TFilter>(TFilter filter)
            where TSource : Entity
            where TFilter : GetRequest
        {
            var parameter = GetParameterExpression<TSource>();
            Expression expression = null;

            var leftPorps = typeof(TSource).GetProperties()
                                    .Where(e => !e.PropertyType.IsSubclassOf(typeof(Entity))
                                        && !e.IsDefined(typeof(NotMappedAttribute)));
            var rightProps = typeof(TFilter).GetProperties()
                                    .Where(f => !f.IsDefined(typeof(NotMappedAttribute)));

            var compareProps = rightProps
                        .SelectMany(
                            r => leftPorps
                                    .Where(l =>
                                        r.Name.StartsWith(l.Name, StringComparison.Ordinal)
                                        && r.GetValue(filter) != null
                                    )
                                    .OrderBy(g => g.Name)
                                    .Select(e => (Left: e, Right: r))
                        );
            foreach (var compareProp in compareProps)
            {
                var leftProp = compareProp.Left;
                var rightProp = compareProp.Right;

                TypeCode typeCode = Utils.Tools.GetTypeCode(rightProp);
                var rightValue = rightProp.GetValue(filter);
                switch (typeCode)
                {
                    case TypeCode.String:
                        {
                            var isEqual = rightProp
                                            .GetCustomAttribute(typeof(EqualFilterAttribute), false) != null;
                            var value = rightValue as string;
                            expression = GetStringExpression(parameter, leftProp.Name, value, isEqual);
                        }
                        break;
                    case TypeCode.DateTime:
                        {
                            var value = rightValue.ToDateTime();
                            expression = GetDateExpression(parameter, leftProp.Name, rightProp.Name, value);
                        }
                        break;
                    case TypeCode.Boolean:
                        {
                            var value = rightValue.ToBoolean();
                            expression = GetBoolExpression(parameter, leftProp.Name, value);
                        }
                        break;
                    case TypeCode.Int32:
                        {
                            var value = rightValue.ToInt32();
                            expression = GetIntExpression(parameter, leftProp.Name, value);
                        }
                        break;
                }

                yield return GenerateLambda<TSource>(expression, parameter);
            }
        }

        private static Expression GetBoolExpression(ParameterExpression parameter, string propName, bool value)
        {
            var property = Expression.Property(parameter, propName);
            var convert = Expression.Convert(property, typeof(bool));
            var expression = Expression.Constant(value);
            return Expression.Equal(convert, expression);
        }

        private static Expression GetIntExpression(ParameterExpression parameter, string propName, int value)
        {
            var property = Expression.Property(parameter, propName);
            var convert = Expression.Convert(property, typeof(int));
            var expression = Expression.Constant(value);
            return Expression.Equal(convert, expression);
        }

        private static IEnumerable<Expression<Func<TSource, bool>>> FilterBy<TSource>() where TSource : Entity
        {
            var parameter = GetParameterExpression<TSource>();
            Expression expression = null;

            foreach (var prop in typeof(TSource).GetProperties()
                        .Where(e => e.IsDefined(typeof(DefaultFilterAttribute), true)))
            {
                var attribute = (DefaultFilterAttribute)Attribute
                        .GetCustomAttribute(prop, typeof(DefaultFilterAttribute));
                var typeCode = Utils.Tools.GetTypeCode(prop);

                if (typeCode == TypeCode.String)
                {
                    var stringAttribute = attribute as DefaultStringFilterAttribute;
                    expression = GetStringExpression(parameter, prop.Name, stringAttribute.Search, false);
                }
                else
                {
                    var dateAttribute = attribute as DefaultDateTimeFilterAttribute;
                    var value = DateTime.Today.AddDays(dateAttribute.DateRange).Date;

                    expression = GetDateExpression(parameter, prop.Name, null, value);
                }
                yield return GenerateLambda<TSource>(expression, parameter);
            }
        }

        private static ParameterExpression GetParameterExpression<TSource>()
            where TSource : class
        {
            return Expression.Parameter(typeof(TSource));
        }

        private static Expression GetStringExpression(ParameterExpression parameter, string propName, string search, bool isEqual)
        {
            Expression result = null;
            if (isEqual)
            {
                var property = Expression.Property(parameter, propName);
                var value = Expression.Constant(search);
                result = Expression.Equal(property, value);
            }
            else
            {
                var pattern = Expression.Constant($"%{search}%");
                result = Expression.Call(
                    typeof(DbFunctionsExtensions), "Like", Type.EmptyTypes,
                    Expression.Constant(EF.Functions),
                    Expression.Property(parameter, propName), pattern);
            }
            return result;
        }

        private static Expression GetDateExpression(
            ParameterExpression parameter, string propName, string filterPropName, DateTime value)
        {
            Expression expression;

            if (IsLessThanOrEqual(filterPropName))
            {
                value = value.Date.AddDays(1).AddMilliseconds(-1);
                expression = GetExpressionBy(parameter, propName, value, ExpressionType.LessThanOrEqual);
            }
            else
            {
                expression = GetExpressionBy(parameter, propName, value, ExpressionType.GreaterThanOrEqual);
            }

            return expression;
        }

        public static bool IsLessThanOrEqual(string filterPropName)
        {
            return (filterPropName.IsNotNullOrEmpty() &&
                (filterPropName.EndsWith("To", StringComparison.OrdinalIgnoreCase) ||
                filterPropName.StartsWith("End", StringComparison.OrdinalIgnoreCase)));
        }

        private static Expression GetExpressionBy(
            ParameterExpression parameter, string propName, DateTime value, ExpressionType expressionType)
        {
            var pattern = Expression.Constant(value);
            Expression result = null;

            var member = Expression.Property(parameter, propName);
            var convert = Expression.Convert(member, typeof(DateTime));
            switch (expressionType)
            {
                case ExpressionType.GreaterThanOrEqual:
                    result = Expression.GreaterThanOrEqual(convert, pattern);
                    break;
                case ExpressionType.LessThanOrEqual:
                    result = Expression.LessThanOrEqual(convert, pattern);
                    break;
            }

            return result;
        }

        private static Expression<Func<TSource, bool>> GenerateLambda<TSource>(
            Expression expression, ParameterExpression parameter)
                where TSource : Entity
        {
            return Expression.Lambda<Func<TSource, bool>>(expression, parameter);
        }
    }
}