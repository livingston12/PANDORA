using System;
using System.Globalization;

namespace Pandora.Core.Extensions
{
    public static class ObjectExtension
    {
        public static DateTime ToDateTime(this object date)
        {
            var asNullableDate = date as DateTime?;
            var returnDate = new DateTime();

            if (asNullableDate.HasValue)
            {
                returnDate = asNullableDate.Value.Date;
            }

            return returnDate;
        }

        public static bool ToBoolean(this object value)
        {
            bool result = false;
            if (value is bool boolean)
            {
                result = boolean;
            }

            return result;
        }

        public static int ToInt32(this object value)
        {
            int result = 0;
            if (value != null)
            {
                result = int.Parse(value.ToString(), CultureInfo.CurrentCulture);
            }

            return result;
        }
    }
}