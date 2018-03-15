using System;

namespace WebAPI.Helper_Classes
{
    public static class DateTimeTools
    {
        /// <summary>
        /// Checks if a provided DateTime object is valid.
        /// </summary>
        /// <param name="time">DateTime object to validate.</param>
        /// <returns>True or false indicating validity of DateTime object.</returns>
        public static bool validateDateTime(this DateTime time)
        {
            int Year = time.Year;
            int Month = time.Month;
            int Day = time.Day;
            int Hour = time.Hour;
            int Minute = time.Minute;
            int Second = time.Second;

            //  Year
            if (Year < 1900 || Year > 9999)
                return (false);

            // Month
            if (Month < 1 || Month > 12)
                return (false);

            // Validate Day based on the month and leap year (for February).
            if (Month == 1 || Month == 3 || Month == 5 || Month == 7 || Month == 8 || Month == 10 || Month == 12)
            {
                if (Day < 1 || Day > 31)
                    return (false);
            }
            else if (Month == 4 || Month == 6 || Month == 9 || Month == 11)
            {
                if (Day < 1 || Day > 30)
                    return (false);
            }
            else
            {
                // Only occurs when: Month == 2
                if (DateTime.IsLeapYear((int)Year))
                {
                    if (Day < 1 || Day > 29)
                        return (false);
                }
                else
                {
                    if (Day < 1 || Day > 28)
                        return (false);
                }
            }

            //  Hour
            if (Hour < 1 || Hour > 23)
                return (false);

            //  Minute
            if (Minute < 0 || Minute > 59)
                return (false);

            //  Second
            if (Second < 0 || Second > 59)
                return (false);

            return (true);
        }

        /// <summary>
        /// Returns the first second of the specified day of the date. Used for queries.
        /// Creates a new datetime object by removing all seconds, minutes and hours.
        /// </summary>
        /// <param name="time">A DateTime object.</param>
        /// <returns>DateTime object of the first second of a certain specified date.</returns>
        public static DateTime getDayBeginning(this DateTime time)
        {
            DateTime dayStart = new DateTime(time.Year, time.Month, time.Day, 0, 0, 0);
            return (dayStart);
        }

        /// <summary>
        /// Returns the last second of the specified day of the date. Used for queries.
        /// Creates a new datetime object by setting all seconds, minutes and hours to the highest value possible.
        /// </summary>
        /// <param name="time">A DateTime object.</param>
        /// <returns>DateTime object of the last second of a certain specified date.</returns>
        public static DateTime getDayEnd(this DateTime time)
        {
            DateTime dayEnd = new DateTime(time.Year, time.Month, time.Day, 23, 59, 59);
            return (dayEnd);
        }
    }
}
