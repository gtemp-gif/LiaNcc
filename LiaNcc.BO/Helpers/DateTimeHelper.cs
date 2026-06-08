using System;
using System.Runtime.InteropServices;

namespace LiaNcc.BO.Helpers
{
    public static class DateTimeHelper
    {
        private static readonly TimeZoneInfo RomeTimeZone;

        static DateTimeHelper()
        {
            string tzId = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
                ? "W. Europe Standard Time"
                : "Europe/Rome";

            try
            {
                RomeTimeZone = TimeZoneInfo.FindSystemTimeZoneById(tzId);
            }
            catch (TimeZoneNotFoundException)
            {
                // Fallback if the specific ID is not found
                RomeTimeZone = TimeZoneInfo.Local;
            }
        }

        public static DateTime ToRomeTime(this DateTime utcDateTime)
        {
            if (utcDateTime.Kind == DateTimeKind.Unspecified)
            {
                utcDateTime = DateTime.SpecifyKind(utcDateTime, DateTimeKind.Utc);
            }
            return TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, RomeTimeZone);
        }

        public static DateTime? ToRomeTime(this DateTime? utcDateTime)
        {
            if (!utcDateTime.HasValue) return null;
            return ToRomeTime(utcDateTime.Value);
        }

        public static string ToDisplayString(this DateTime utcDateTime, bool includeTime = true)
        {
            var romeTime = ToRomeTime(utcDateTime);
            return romeTime.ToString(includeTime ? "dd/MM/yyyy HH:mm" : "dd/MM/yyyy");
        }

        public static string ToDisplayString(this DateTime? utcDateTime, bool includeTime = true)
        {
            if (!utcDateTime.HasValue) return "-";
            return ToDisplayString(utcDateTime.Value, includeTime);
        }

        public static DateTime ToUtcFromRome(this DateTime romeDateTime)
        {
            return TimeZoneInfo.ConvertTimeToUtc(romeDateTime, RomeTimeZone);
        }
    }
}
