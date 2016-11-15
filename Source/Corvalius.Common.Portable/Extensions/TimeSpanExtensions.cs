using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System
{
    public static class TimeSpanExtensions
    {
        public static string ToReadableString(this TimeSpan span)
        {
            string formatted = string.Format("{0}{1}{2}{3}",
                span.Days > 0 ? string.Format("{0:0} days, ", span.Days) : string.Empty,
                span.Hours > 0 ? string.Format("{0:0} hours, ", span.Hours) : string.Empty,
                span.Minutes > 0 ? string.Format("{0:0} minutes, ", span.Minutes) : string.Empty,
                span.Seconds > 0 ? string.Format("{0:0} seconds", span.Seconds) : string.Empty);

            if (formatted.EndsWith(", ")) formatted = formatted.Substring(0, formatted.Length - 2);

            return formatted;
        }

        public static string ToShortReadableString(this TimeSpan span)
        {
            string formatted = string.Format("{0}{1}:{2}:{3}",
                                             span.Days > 0 ? string.Format("{0:0} days ", span.Days) : string.Empty,
                                             string.Format("{0:00}", span.Hours),
                                             string.Format("{0:00}", span.Minutes),
                                             string.Format("{0:00}", span.Seconds));

            if (formatted.EndsWith(", ")) formatted = formatted.Substring(0, formatted.Length - 2);

            return formatted;
        }

        public static string ToHMSString(this TimeSpan span)
        {
            var hours = span.Days * 24 + span.Hours;
            var seconds = span.TotalSeconds;
            var finalFormat = "{0}";

            if (seconds > 0 && seconds < 1)
            {
                seconds = 1;
                finalFormat = "<{0}";
            }
            else
            {
                seconds = span.Seconds;
            }

            string formatted = string.Format("{0}:{1}:{2}",
                                             hours,
                                             string.Format("{0:00}", span.Minutes),
                                             string.Format("{0:00}", seconds));

            if (formatted.EndsWith(", ")) 
                formatted = formatted.Substring(0, formatted.Length - 2);


            return string.Format(finalFormat, formatted);
        }

        public static string ToHMSMilliString(this TimeSpan span)
        {
            var hours = span.Days * 24 + span.Hours;
            var minutes = span.Minutes;
            var seconds = span.Seconds;
            var milliseconds = span.ToString("fffff");

            if (span.TotalMilliseconds == 0 )
                return "0.0000";

            return string.Format("{0}:{1}:{2}.{3}",
                                             hours,
                                             string.Format("{0:00}", minutes),
                                             string.Format("{0:00}", seconds),
                                             milliseconds);
        }
    }
}
