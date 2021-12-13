// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using Pandora.Core.Attributes;

[assembly: InternalsVisibleTo("Pandora.Test")]
namespace Pandora.Core.Utils
{
    [DebuggerStepThrough]
    public static class Check
    {
        public static T Condition<T>([ValidatedNotNull, NoEnumeration] T value, [ValidatedNotNull, NotNull] Predicate<T> condition, [InvokerParameterName, ValidatedNotNull, NotNull] string parameterName)
        {
            NotNull(condition, nameof(condition));

            if (!condition(value))
            {
                NotEmpty(parameterName, nameof(parameterName));

                throw new ArgumentOutOfRangeException(parameterName);
            }

            return value;
        }

        [ContractAnnotation("value:null => halt")]
        public static T NotNull<T>([ValidatedNotNull, NoEnumeration] T value, [InvokerParameterName, ValidatedNotNull, NotNull] string parameterName)
        {
            if (value == null)
            {
                NotEmpty(parameterName, nameof(parameterName));

                throw new ArgumentNullException(parameterName);
            }

            return value;
        }

        [ContractAnnotation("value:null => halt")]
        public static T NotNull<T>(
            [NoEnumeration] T value,
            [InvokerParameterName, ValidatedNotNull, NotNull] string parameterName,
            [ValidatedNotNull, NotNull] string propertyName)
        {
            if (value == null)
            {
                NotEmpty(parameterName, nameof(parameterName));
                NotEmpty(propertyName, nameof(propertyName));

                throw new ArgumentException(CoreStrings.ArgumentPropertyNull(propertyName, parameterName));
            }

            return value;
        }

        [ContractAnnotation("value:null => halt")]
        public static string NotEmpty(string value, [InvokerParameterName, ValidatedNotNull, NotNull] string parameterName)
        {
            Exception e = null;
            if (value is null)
            {
                e = new ArgumentNullException(parameterName);
            }
            else if (value.Trim().Length == 0)
            {
                e = new ArgumentException(CoreStrings.ArgumentIsEmpty(parameterName));
            }

            if (e != null)
            {
                NotEmpty(parameterName, nameof(parameterName));

                throw e;
            }

            return value;
        }

        [ContractAnnotation("value:null => halt")]
        public static string NotEmpty(
            string value,
            [InvokerParameterName, ValidatedNotNull, NotNull] string parameterName,
            [ValidatedNotNull, NotNull] string propertyName)
        {
            Exception e = null;
            if (value is null)
            {
                e = new ArgumentException(CoreStrings.ArgumentPropertyNull(propertyName, parameterName));
            }
            else if (value.Trim().Length == 0)
            {
                e = new ArgumentException(CoreStrings.ArgumentPropertyEmpty(propertyName, parameterName));
            }

            if (e != null)
            {
                NotEmpty(parameterName, nameof(parameterName));
                NotEmpty(propertyName, nameof(propertyName));

                throw e;
            }
            return value;
        }

        [ContractAnnotation("value:null => halt")]
        public static int GreaterThanZero(int value, [InvokerParameterName, ValidatedNotNull, NotNull] string parameterName)
        {
            if (value <= 0)
            {
                NotEmpty(parameterName, nameof(parameterName));
                throw new ArgumentOutOfRangeException(CoreStrings.ArgumentIsLowerOrEqualZero(parameterName));
            }

            return value;
        }

        public static IList<T> HasNoNulls<T>(IList<T> value, [InvokerParameterName, ValidatedNotNull, NotNull] string parameterName)
            where T : class
        {
            NotNull(value, parameterName);

            if (value.Any(e => e == null))
            {
                NotEmpty(parameterName, nameof(parameterName));

                throw new ArgumentException(parameterName);
            }

            return value;
        }
    }
}