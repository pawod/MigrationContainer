using System;

namespace Pawod.MigrationContainer.Extensions
{
    public static class DateTimeExtensions
    {
        private static DateTime _unixTimeStampBase = new DateTime(1970, 1, 1, 0, 0, 0);

        public static DateTime Truncate(this DateTime dateTime, TimeSpan timeSpan)
        {
            return timeSpan == TimeSpan.Zero ? dateTime : dateTime.AddTicks(-(dateTime.Ticks%timeSpan.Ticks));
        }

        public static DateTime? UnixTimeStampToDateTime(this long? timestamp)
        {
            if (timestamp.HasValue) return timestamp.Value.UnixTimeStampToUtcDateTime();

            return null;
        }

        public static DateTime UnixTimeStampToUtcDateTime(this long timestamp)
        {
            return _unixTimeStampBase.AddSeconds(timestamp);
        }

        public static long UtcDateTimeToUnixTimeStamp(this DateTime dateTime)
        {
            return Convert.ToInt64(dateTime.Subtract(_unixTimeStampBase).TotalSeconds);
        }

        public static long? UtcDateTimeToUnixTimeStamp(this DateTime? dateTime)
        {
            return dateTime.HasValue ? (long?) dateTime.Value.UtcDateTimeToUnixTimeStamp() : null;
        }
    }
}