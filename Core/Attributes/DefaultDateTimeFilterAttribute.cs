using System;

namespace Pandora.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    internal class DefaultDateTimeFilterAttribute : DefaultFilterAttribute
    {
        public int DateRange { get; set; }

        /// <summary>
        /// </summary>
        /// <param name="dateRange">Date range that will add today</param>
        internal DefaultDateTimeFilterAttribute(int dateRange)
        {
            DateRange = dateRange;
        }
    }
}