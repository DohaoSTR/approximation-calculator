using System;

namespace ApproximationCalculator.Misc
{
    public static class DateTimeExtension
    {
        public static string GetTimeDifference(this DateTime dateTime, DateTime startTime)
        {
            DateTime endTime = DateTime.Now;
            TimeSpan timeDifference = endTime - startTime;

            return timeDifference.TotalSeconds.ToString();
        }
    }
}