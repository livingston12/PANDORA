using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Pandora.Core.Utils;

namespace Pandora.Core.Extensions
{
    public static class StringExtension
    {
        public static IEnumerable<string> AsEnumerable(this string value, char separator = ',')
        {
            if (value.IsNotNullOrEmpty())
            {
                foreach (var str in value.Split(separator))
                {
                    yield return str.Trim();
                }
            }
        }

        public static List<string> AsList(this string value)
        {
            List<string> result = null;
            if (value.IsNotNullOrEmpty())
            {
                result = value.AsEnumerable().ToList();
            }
            return result;
        }

        public static string[] AsArray(this string value)
        {
            string[] result = null;
            if (value.IsNotNullOrEmpty())
            {
                result = value.AsEnumerable().ToArray();
            }
            return result;
        }

        public static bool IsNotNullOrEmpty(this string value)
        {
            return !string.IsNullOrEmpty(value);
        }

        public static string Ofuscate(this string source)
        {
            Check.NotNull(source, nameof(source));
            Check.NotEmpty(source, nameof(source));
            var result = Regex.Replace(source, @".*([0-9]{4})$", "$1", RegexOptions.Singleline);
            return result.PadLeft(source.Length, '*');
        }

        public static T ToEnum<T>(this string value)
            where T : Enum
        {
            Check.NotEmpty(value, nameof(value));
            return (T)Enum.Parse(typeof(T), value);
        }
    }
}