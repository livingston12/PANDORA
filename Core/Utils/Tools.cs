using System;
using System.Reflection;

namespace Pandora.Core.Utils
{
    public static class Tools
    {
        public static TypeCode GetTypeCode(PropertyInfo property)
        {
            Check.NotNull(property, nameof(property));
            TypeCode typeCode = IsNotNullable(property)
                ? Type.GetTypeCode(property.PropertyType)
                : Type.GetTypeCode(Nullable.GetUnderlyingType(property.PropertyType));
            return typeCode;
        }

        public static bool IsNullable(PropertyInfo property)
        {
            Check.NotNull(property, nameof(property));
            return Nullable.GetUnderlyingType(property.PropertyType) != null;
        }

        public static bool IsNotNullable(PropertyInfo property)
        {
            return !IsNullable(property);
        }
    }
}