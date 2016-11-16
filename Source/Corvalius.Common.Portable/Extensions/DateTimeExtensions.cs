
using System.Collections.Generic;
using System.Text;
using System.Globalization;
using System;

namespace Corvalius
{
    public static class DateTimeExtensions
    {
        public static DateTime GetFirstDayOfWeek(this DateTime dayInWeek)
        {
            DayOfWeek firstDay = CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek;
            DateTime firstDayInWeek = dayInWeek.Date;

            while (firstDayInWeek.DayOfWeek != firstDay)
                firstDayInWeek = firstDayInWeek.AddDays(-1);

            return firstDayInWeek;
        }

        public static DateTime GetFirstHour(this DateTime day)
        {
            return new DateTime(day.Year, day.Month, day.Day, 0, 0, 0);
        }

        public static DateTime GetLastHour(this DateTime day)
        {
            return new DateTime(day.Year, day.Month, day.Day, 23, 59, 59);
        }

        public static DateTime GetLastDayOfWeek(this DateTime dayInWeek)
        {
            DayOfWeek lastDay = DayOfWeek.Saturday;

            if (CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek == DayOfWeek.Monday)
                lastDay = DayOfWeek.Sunday; 

            var lastDayInWeek = dayInWeek.Date;

            while (lastDayInWeek.DayOfWeek != lastDay)
                lastDayInWeek = lastDayInWeek.AddDays(+1);

            return lastDayInWeek;
        }

        public static DateTime GetFirstDayOfMonth(this DateTime input)
        {
            return input != DateTime.MinValue
                       ? new DateTime(input.Year, input.Month, 1, 0, 0, 0)
                       : new DateTime(DateTime.Now.Year, 1, 1);
        }

        public static DateTime GetLastDayOfMonth(this DateTime input)
        {
            return input != DateTime.MinValue
                       ? new DateTime(input.Year, input.Month, DateTime.DaysInMonth(input.Year, input.Month), 23, 59, 59)
                       : new DateTime(DateTime.Now.Year, 12, 31);
        }

        public static DateTime GetFirstDayOfLastMonth(this DateTime input)
        {
            var date = input != DateTime.MinValue ? input : new DateTime(DateTime.Now.Year, 1, 1);

            date = date.GetFirstDayOfMonth().Subtract(new TimeSpan(0, 0, 1));

            return new DateTime(date.Year, date.Month, 1, 0, 0, 0);
        }

        public static DateTime GetLastDayOfLastMonth(this DateTime input)
        {
            var date = input != DateTime.MinValue ? input : new DateTime(DateTime.Now.Year, 1, 1);

            date = date.GetFirstDayOfMonth().Subtract(new TimeSpan(0, 0, 1));

            return new DateTime(date.Year, date.Month, date.Day, 23, 59, 59);
        }

        public static string ToJsArray(this IEnumerable<DateTime> dates, string format)
        {
            string ret = "[{0}]";
            string dateHolder = "'{0}',";
            var partial = new StringBuilder();

            foreach (var date in dates)
            {
                partial.Append(String.Format(dateHolder, date.ToString(format)));
            }

            var result = partial.ToString();

            if (!string.IsNullOrEmpty(result))
                result = result.Substring(0, result.LastIndexOf(","));

            return string.Format(ret, result);
        }

        public static long ToJavaScriptFormat(this System.DateTime input)
        {
            if (input != DateTime.MinValue)
            {
                TimeSpan span = new System.TimeSpan(System.DateTime.Parse("1/1/1970").Ticks);
                DateTime time = input.Subtract(span);
                return (long) (time.Ticks/10000);
            }

            return 0;
        }

        /// <summary>
        /// Gets the 12:00:00 instance of a DateTime
        /// </summary>
        public static DateTime AbsoluteStart(this DateTime dateTime)
        {
            return dateTime.Date;
        }

        /// <summary>
        /// Gets the 11:59:59 instance of a DateTime
        /// </summary>
        public static DateTime AbsoluteEnd(this DateTime dateTime)
        {
            return AbsoluteStart(dateTime).AddDays(1).AddTicks(-1);
        }

    }
}
