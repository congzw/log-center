using System;

namespace Common.Utilities
{
    public class DateHelper
    {
        public Func<DateTimeOffset> GetNow = () => DateTimeOffset.Now;
        public Func<DateTimeOffset> GetDefault = () => new DateTimeOffset(2000, 1, 1, 0, 0, 0, TimeSpan.Zero);
        
        #region auto resolve from di or default

        [LazySingleton]
        public static DateHelper Instance => LazySingleton.Instance.Resolve(() => new DateHelper());

        #endregion
    }

    public static class DatetimeExtensions
    {
        public static DateTime ToDateTime(this DateTimeOffset offset, DateTimeKind newDateTimeKind = DateTimeKind.Unspecified)
        {
            var dateTime = offset.UtcDateTime;
            if (newDateTimeKind != dateTime.Kind)
            {
                var newDateTime = DateTime.SpecifyKind(dateTime, newDateTimeKind);
                return newDateTime;
            }
            return dateTime;
        }
        public static DateTimeOffset ToDateTimeOffset(this DateTime dateTime, DateTimeKind newDateTimeKind = DateTimeKind.Unspecified)
        {
            if (newDateTimeKind != dateTime.Kind)
            {
                dateTime = DateTime.SpecifyKind(dateTime, newDateTimeKind);
            }
            DateTimeOffset utcTime = dateTime;
            return utcTime;
        }

        public static string AsFormat(this DateTime datetime, string dateFormat = "yyyyMMdd", string timeFormat = "-HH:mm:ss", string millisecondFormat = ".fff")
        {
            var format = $"{dateFormat}{timeFormat}{millisecondFormat}";
            return datetime.ToString(format);
        }
        public static string AsFormatDefault(this DateTime datetime, string format = "yyyyMMdd-HHmmss")
        {
            return datetime.ToString(format);
        }
        
        public static string AsFormat(this DateTimeOffset datetime, string dateFormat = "yyyyMMdd", string timeFormat = "-HH:mm:ss", string millisecondFormat = ".fff")
        {
            var format = $"{dateFormat}{timeFormat}{millisecondFormat}";
            return datetime.ToString(format);
        }
        public static string AsFormatDefault(this DateTimeOffset datetime, string format = "yyyyMMdd-HHmmss")
        {
            return datetime.ToString(format);
        }
    }
}
