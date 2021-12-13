using Cronos;
using System;
using System.Linq;

namespace Pandora.Core.Utils
{
    public static class CronTranslate
    {
        public static DateTime? GetNextRun(string frequence)
        {
            return Next(CronExpression.Parse(frequence, CronFormat.IncludeSeconds));
        }

        public static DateTime? GetLastRun(string frequence)
        {
            return Last(CronExpression.Parse(frequence, CronFormat.IncludeSeconds));
        }

        private static DateTime? Next(CronExpression expression)
        {
            DateTimeOffset? nextOccurrence = expression.GetNextOccurrence(DateTimeOffset.UtcNow, TimeZoneInfo.Local);
            return nextOccurrence?.DateTime;
        }

        private static DateTime? Last(CronExpression expression)
        {
            DateTime? lastOccurrence = Next(expression) - GetOccurrences(expression);
            return lastOccurrence;
        }

        private static TimeSpan GetOccurrences(CronExpression expression)
        {
            var occurrences = expression.GetOccurrences(DateTimeOffset.UtcNow, DateTimeOffset.UtcNow.AddDays(2), TimeZoneInfo.Local);
            return occurrences.ElementAt(1) - occurrences.ElementAt(0);
        }
    }
}