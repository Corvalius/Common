
using System.Collections.Generic;
using System.Text;
using System.Globalization;
using System;

namespace Corvalius
{
    public static class DateTimeOffsetExtensions
    {
        public static DateTimeOffset GetFirstDayOfWeek(this DateTimeOffset dayInWeek)
        {
            DayOfWeek firstDay = CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek;
            DateTime firstDayInWeek = dayInWeek.Date;

            while (firstDayInWeek.DayOfWeek != firstDay)
                firstDayInWeek = firstDayInWeek.AddDays(-1);

            return firstDayInWeek;
        }

        public static DateTimeOffset GetFirstHour(this DateTimeOffset day)
        {
            return new DateTimeOffset(day.Year, day.Month, day.Day, 0, 0, 0, day.Offset);
        }

        public static DateTimeOffset GetLastHour(this DateTimeOffset day)
        {
            return new DateTimeOffset(day.Year, day.Month, day.Day, 23, 59, 59, day.Offset);
        }

        public static DateTimeOffset GetLastDayOfWeek(this DateTimeOffset dayInWeek)
        {
            DayOfWeek lastDay = DayOfWeek.Saturday;

            if (CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek == DayOfWeek.Monday)
                lastDay = DayOfWeek.Sunday; 

            var lastDayInWeek = dayInWeek.Date;

            while (lastDayInWeek.DayOfWeek != lastDay)
                lastDayInWeek = lastDayInWeek.AddDays(+1);

            return lastDayInWeek;
        }

        public static DateTimeOffset GetFirstDayOfMonth(this DateTimeOffset input)
        {
            return input != DateTimeOffset.MinValue
                       ? new DateTimeOffset(input.Year, input.Month, 1, 0, 0, 0, input.Offset)
                       : new DateTimeOffset(DateTimeOffset.UtcNow.Ticks, input.Offset);
        }

        public static DateTimeOffset GetLastDayOfMonth(this DateTimeOffset input)
        {
            return input != DateTimeOffset.MinValue
                                ? new DateTimeOffset(input.Year, input.Month, DateTime.DaysInMonth(input.Year, input.Month), 23, 59, 59, input.Offset)
                                : new DateTimeOffset(DateTimeOffset.UtcNow.Year, 12, 31, 0, 0, 0, input.Offset);
        }

        public static DateTimeOffset GetFirstDayOfLastMonth(this DateTimeOffset input)
        {
            var date = input != DateTimeOffset.MinValue ? input : new DateTimeOffset(DateTimeOffset.UtcNow.Year, 1, 1, 0, 0, 0, input.Offset);

            date = date.GetFirstDayOfMonth()
                       .Subtract(new TimeSpan(0, 0, 1));

            return new DateTimeOffset(date.Year, date.Month, 1, 0, 0, 0, input.Offset);
        }

        public static DateTimeOffset GetLastDayOfLastMonth(this DateTimeOffset input)
        {
            var date = input != DateTimeOffset.MinValue ? input : new DateTimeOffset(DateTimeOffset.UtcNow.Year, 1, 1, 0, 0, 0, input.Offset);

            date = date.GetFirstDayOfMonth().Subtract(new TimeSpan(0, 0, 1));

            return new DateTimeOffset(date.Year, date.Month, date.Day, 23, 59, 59, input.Offset);
        }

        public static string ToJsArray(this IEnumerable<DateTimeOffset> dates, string format)
        {
            string ret = "[{0}]";
            string dateHolder = "'{0}',";
            var partial = new StringBuilder();

            foreach (var date in dates)
            {
                var adjustedDate = date.DateTime + date.Offset;
                partial.Append(String.Format(dateHolder, adjustedDate.ToString(format)));
            }

            var result = partial.ToString();

            if (!string.IsNullOrEmpty(result))
                result = result.Substring(0, result.LastIndexOf(","));

            return string.Format(ret, result);
        }

        public static long ToJavaScriptFormat(this DateTimeOffset input)
        {
            if (input != DateTimeOffset.MinValue)
            {
                TimeSpan span = new System.TimeSpan(DateTimeOffset.Parse("1/1/1970").Ticks);
                DateTimeOffset time = input.Subtract(span).Add(input.Offset);
                return (long) (time.Ticks/10000);
            }

            return 0;
        }

        /// <summary>
        /// Gets the 12:00:00 instance of a DateTime
        /// </summary>
        public static DateTimeOffset AbsoluteStart(this DateTimeOffset dateTime)
        {
            return new DateTimeOffset(dateTime.Year, dateTime.Month, dateTime.Day, 0, 0, 0, dateTime.Offset);
        }

        /// <summary>
        /// Gets the 11:59:59 instance of a DateTime
        /// </summary>
        public static DateTimeOffset AbsoluteEnd(this DateTimeOffset dateTime)
        {
            return AbsoluteStart(dateTime).AddDays(1).AddTicks(-1);
        }

    }
}
