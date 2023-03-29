using System;
using System.Globalization;

namespace SoundpaysAdd.Core.Helpers
{
    public class DateTimeExt
    {
        const int SECOND = 1;
        const int MINUTE = 60 * SECOND;
        const int HOUR = 60 * MINUTE;
        const int DAY = 24 * HOUR;
        const int MONTH = 30 * DAY;

        public static Int64 GetJavascriptTime()
        {
            Int64 retval = 0;
            var st = new DateTime(1970, 1, 1);
            TimeSpan t = (DateTime.UtcNow.ToUniversalTime() - st);
            retval = (Int64)(t.TotalMilliseconds + 0.5);
            return retval;
        }

        public static DateTime LastDayOfMonth()
        {
            DateTime today = DateTime.Today;
            return new DateTime(today.Year, today.Month, DateTime.DaysInMonth(today.Year, today.Month));
        }

        public static string RelativeDateTime(DateTime date)
        {
            var ts = new TimeSpan(DateTime.UtcNow.Ticks - date.Ticks);
            double delta = Math.Abs(ts.TotalSeconds);

            if (delta < 0)
            {
                return "not yet";
            }
            if (delta < 1 * MINUTE)
            {
                return ts.Seconds == 1 ? "one second ago" : ts.Seconds + " seconds ago";
            }
            if (delta < 2 * MINUTE)
            {
                return "a minute ago";
            }
            if (delta < 45 * MINUTE)
            {
                return ts.Minutes + " minutes ago";
            }
            if (delta < 90 * MINUTE)
            {
                return "an hour ago";
            }
            if (delta < 24 * HOUR)
            {
                return ts.Hours + " hours ago";
            }
            if (delta < 48 * HOUR)
            {
                return "yesterday";
            }
            if (delta < 30 * DAY)
            {
                return ts.Days + " days ago";
            }
            if (delta < 12 * MONTH)
            {
                int months = Convert.ToInt32(Math.Floor((double)ts.Days / 30));
                return months <= 1 ? "one month ago" : months + " months ago";
            }
            else
            {
                int years = Convert.ToInt32(Math.Floor((double)ts.Days / 365));
                return years <= 1 ? "one year ago" : years + " years ago";
            }
        }
        /// <summary>
        /// Standard Date Time
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static string StandardDateTime(TimeSpan? timeSpan)
        {
            if (timeSpan.HasValue)
            {
                DateTime time = DateTime.Today.Add(timeSpan.Value);
                return time.ToString("hh:mm tt");
            }
            return "";
        }
        /// <summary>
        /// get current month complete name
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static string GetFullMonthName(DateTime dateTime)
        {
            return CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(dateTime.Month);
        }

        /// <summary>
        /// get current month short name
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static string GetShortMonthName(DateTime dateTime)
        {
            return CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(dateTime.Month);
        }
    }
}
