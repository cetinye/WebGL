using System;
using UnityEngine;

namespace Unity_CS
{
    /// <summary>
    /// DateTime Extensions
    /// </summary>
    public static class _CSDateTime
    {
        public static int _ToUnixTimestamp(this DateTime date)
        {
            DateTime unixEpoch = new DateTime(1970, 1, 1, 0, 0, 0);
            TimeSpan unixTimeSpan = date - unixEpoch;

            var r = (int)unixTimeSpan.TotalSeconds;
            //_.Log("Date: " + date.ToString() + " return:" + r.ToString());
            return r;
        }

        public static int _ToMillTimestamp(this DateTime date)
        {
            DateTime mill = new DateTime(2000, 1, 1, 0, 0, 0);
            TimeSpan mTimeSpan = date - mill;

            var r = (int)mTimeSpan.TotalSeconds;
            //_.Log("Date: " + date.ToString() + " return:" + r.ToString());
            return r;
        }

        public static DateTime GetDateTimeFromUnixTime(ulong unixTime)
        {
            return GetDateTimeFromUnixTime((long)unixTime);
        }

        public static DateTime GetDateTimeFromMillTime(ulong millTime)
        {
            return GetDateTimeFromMillTime((long)millTime);
        }

        public static DateTime GetDateTimeFromUnixTime(long unixTime)
        {
            DateTime unixEpoch = new DateTime(1970, 1, 1, 0, 0, 0);
            return unixEpoch.AddSeconds(unixTime);
        }

        public static DateTime GetDateTimeFromMillTime(long millTime)
        {
            DateTime mill = new DateTime(2000, 1, 1, 0, 0, 0);
            return mill.AddSeconds(millTime);
        }

        public static bool _IsExpired(this DateTime date, int hour)
        {
            return (DateTime.Now - date).TotalHours > hour;
        }

        public static string _ToFileName(this DateTime date, string prefix = "", string format = "yyyyMMdd_HHmmss")
        {
            return prefix + date.ToString(format);
        }

        public static string _ToFileName(string prefix = "", string format = "yyyyMMdd_HHmmss")
        {
            return prefix + DateTime.Now.ToString(format);
        }

    
    }

}