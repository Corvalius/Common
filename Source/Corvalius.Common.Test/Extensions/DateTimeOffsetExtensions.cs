using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Corvalius.Common.Test.Extensions
{
    public class DateTimeOffsetExtensionsTests
    {
        [Fact]
        public void ConvertToJavascriptTicks()
        { 
            var utcTimezone = TimeZoneInfo.Utc;

            var midday = new DateTime(2014, 10, 5, 12, 0, 0, DateTimeKind.Utc);

            var arizonaTime = new DateTimeOffset(midday.Ticks, TimeZoneInfo.FindSystemTimeZoneById("US Mountain Standard Time").BaseUtcOffset);
            var buenosAiresTime = new DateTimeOffset(midday.Ticks, TimeZoneInfo.FindSystemTimeZoneById("Argentina Standard Time").BaseUtcOffset);
            var koreaTime = new DateTimeOffset(midday.Ticks, TimeZoneInfo.FindSystemTimeZoneById("Korea Standard Time").BaseUtcOffset);

            long utcInJavascript = midday.ToJavaScriptFormat();
            long arizonaInJavascript = arizonaTime.ToJavaScriptFormat();
            long buenosAiresInJavascript = buenosAiresTime.ToJavaScriptFormat();
            long koreaInJavascript = koreaTime.ToJavaScriptFormat();

            Assert.True(arizonaInJavascript < buenosAiresInJavascript);
            Assert.True(buenosAiresInJavascript < utcInJavascript);
            Assert.True(utcInJavascript < koreaInJavascript);
        }
    }
}
