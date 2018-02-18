using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPI.Object_Classes
{
    /// <summary>
    /// Represents a DateTime interval. Simplified without time zones.
    /// </summary>
    public class TimeInterval
    {
        // Start DateTime.
        public int? StartYear { get; }
        public int? StartMonth { get; }
        public int? StartDay { get; }
        public int? StartHour { get; }
        public int? StartMinute { get; }
        public int? StartSecond { get; }

        // End DateTime.
        public int? EndYear { get; }
        public int? EndMonth { get; }
        public int? EndDay { get; }
        public int? EndHour { get; }
        public int? EndMinute { get; }
        public int? EndSecond { get; }

        public TimeInterval() { }

        /// <summary>
        /// Method to validate a TimeInterval object.
        /// </summary>
        /// <returns>Returns TRUE if the Time Interval object has all 12 attributes checked to be valid and that the Start DateTime is smaller or equal to the End DateTime.</returns>
        public bool isValidInterval()
        {
            #region Verify Start Attributes (lower bound of interval)
            // Start Year
            if (this.StartYear < 2000 || this.StartYear > 9999)
                return (false);

            //Start Month
            if (this.StartMonth < 1 || this.StartMonth > 12)
                return (false);

            // Validate StartDay based on the month and leap year (for February).
            if (this.StartMonth == 1 || this.StartMonth == 3 || this.StartMonth == 5 || this.StartMonth == 7 || this.StartMonth == 8 || this.StartMonth == 10 || this.StartMonth == 12)
            {
                if (this.StartDay < 1 || this.StartDay > 31)
                    return (false);
            }
            else if (this.StartMonth == 4 || this.StartMonth == 6 || this.StartMonth == 9 || this.StartMonth == 11)
            {
                if (this.StartDay < 1 || this.StartDay > 30)
                    return (false);
            }
            else
            {
                // Only occurs when: this.StartMonth == 2
                if (DateTime.IsLeapYear((int)this.StartYear))
                {
                    if (this.StartDay < 1 || this.StartDay > 29)
                        return (false);
                }
                else
                {
                    if (this.StartDay < 1 || this.StartDay > 28)
                        return (false);
                }
            }

            // Start Hour
            if (this.StartHour < 1 || this.StartHour > 23)
                return (false);

            // Start Minute
            if (this.StartMinute < 0 || this.StartMinute > 59)
                return (false);

            // Start Second
            if (this.StartSecond < 0 || this.StartSecond > 59)
                return (false);

            #endregion

            #region Verify End Attributes (upper bound of interval)
            // End Year
            if (this.EndYear < 2000 || this.EndYear > 9999)
                return (false);

            //End Month
            if (this.EndMonth < 1 || this.EndMonth > 12)
                return (false);

            // Validate EndDay based on the month and leap year (for February).
            if (this.EndMonth == 1 || this.EndMonth == 3 || this.EndMonth == 5 || this.EndMonth == 7 || this.EndMonth == 8 || this.EndMonth == 10 || this.EndMonth == 12)
            {
                if (this.EndDay < 1 || this.EndDay > 31)
                    return (false);
            }
            else if (this.EndMonth == 4 || this.EndMonth == 6 || this.EndMonth == 9 || this.EndMonth == 11)
            {
                if (this.EndDay < 1 || this.EndDay > 30)
                    return (false);
            }
            else
            {
                // Only occurs when: this.EndMonth == 2
                if (DateTime.IsLeapYear((int)this.EndYear))
                {
                    if (this.EndDay < 1 || this.EndDay > 29)
                        return (false);
                }
                else
                {
                    if (this.EndDay < 1 || this.EndDay > 28)
                        return (false);
                }
            }

            // End Hour
            if (this.EndHour < 1 || this.EndHour > 23)
                return (false);

            // End Minute
            if (this.EndMinute < 0 || this.EndMinute > 59)
                return (false);

            // End Second
            if (this.EndSecond < 0 || this.EndSecond > 59)
                return (false);

            #endregion

            #region Since start and end DateTimes are checked to be valid, compare if start datetime is before/equal to end datetime.
            // https://msdn.microsoft.com/en-us/library/272ba130(v=vs.110).aspx

            DateTime start = new DateTime((int)this.StartYear, (int)this.StartMonth, (int)this.StartDay, (int)this.StartHour, (int)this.StartMinute, (int)this.StartSecond);
            DateTime end = new DateTime((int)this.EndYear, (int)this.EndMonth, (int)this.EndDay, (int)this.EndHour, (int)this.EndMinute, (int)this.EndSecond);

            // Check if start time is later than end time.
            if (DateTime.Compare(start,end) > 0)
                return (false);
            #endregion

            return (true);
        }
    }
}
