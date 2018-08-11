using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ZennoSite.Utils
{
    public static class DateTimeHelper
    {
        public static DateTime GetCurrentTime()
        {
            var serverTime = DateTime.Now;
            var localTime = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(serverTime, TimeZoneInfo.Local.Id, "Russian Standard Time");
            return localTime;
        }
    }
}
