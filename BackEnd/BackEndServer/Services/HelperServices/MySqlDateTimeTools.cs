using System.Text.RegularExpressions;

namespace BackEndServer.Services.HelperServices
{
    // My SQL DateTime format: '9999-12-31 23:59:59'

    public static class MySqlDateTimeTools
    {
        // Checks if the provided MySQL DateTime represents the last second of an hour.
        public static bool IsLastSecondOfHour(this string dateTimeString)
        {
            Regex regex = new Regex(@"^\d{4}-\d{2}-\d{2} \d{2}:59:59$");
            return regex.IsMatch(dateTimeString);
        }
    }
}