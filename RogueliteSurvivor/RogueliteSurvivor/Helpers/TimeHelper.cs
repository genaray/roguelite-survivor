namespace RogueliteSurvivor.Helpers
{
    public static class TimeHelper
    {
        public static string ToFormattedTime(this float time)
        {
            string retVal;

            int days = 0, hours = 0, minutes = 0;
            while (time > 60)
            {
                time -= 60;
                minutes++;
                if (minutes == 60)
                {
                    hours++;
                    minutes = 0;
                }
                if (hours == 24)
                {
                    days++;
                    hours = 0;
                }
            }

            if (days > 0)
            {
                retVal = string.Concat(days, ":", hours.ToTwoDigits(), ":", minutes.ToTwoDigits(), ":", ((int)time).ToTwoDigits());
            }
            else if (hours > 0)
            {
                retVal = string.Concat(hours, ":", minutes.ToTwoDigits(), ":", ((int)time).ToTwoDigits());
            }
            else if (minutes > 0)
            {
                retVal = string.Concat(minutes, ":", ((int)time).ToTwoDigits());
            }
            else
            {
                retVal = string.Concat(":", ((int)time).ToTwoDigits());
            }

            return retVal;
        }

        public static string ToFormattedSeconds(this float time)
        {
            return time < 10 ? string.Concat(" ", (int)time) : ((int)time).ToString();
        }

        private static string ToTwoDigits(this int timeMeasurement)
        {
            return timeMeasurement < 10 ? string.Concat("0", timeMeasurement) : timeMeasurement.ToString();
        }
    }
}
