using System;

namespace PediaStatDevice
{
    public class UnixTime
    {
        private static readonly DateTime _epoch = new DateTime(1970, 1, 1, 0, 0, 0, 0);

        public static DateTime FromUnixTime(int seconds)
        {
            return _epoch.Add(TimeSpan.FromSeconds(seconds));
        }

        public static int ToUnixTime(DateTime dt)
        {
            TimeSpan ts = dt.Subtract(_epoch);
            return (int)ts.TotalSeconds;
        }
    }
}
