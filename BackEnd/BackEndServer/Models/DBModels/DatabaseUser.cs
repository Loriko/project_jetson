using BackEndServer.Models.ViewModels;

namespace BackEndServer.Models.DBModels
{
    public class DatabaseUser
    {
        // Table Name
        public static readonly string TABLE_NAME = "user";
        // Attributes of PerSecondStat table.
        public static readonly string USER_ID_LABEL = "id";
        public static readonly string USERNAME_LABEL = "username";
        public static readonly string PASSWORD_LABEL = "password";
        public static readonly string API_KEY_LABEL = "api_key";
        public static readonly string EMAIL_ADDRESS_LABEL = "email_address";
        public static readonly string FIRST_NAME_LABEL = "first_name";
        public static readonly string LAST_NAME_LABEL = "last_name";

        public DatabaseUser()
        {
        }

        public DatabaseUser(UserSettings userSettings)
        {
            UserId = userSettings.UserId;
            Username = userSettings.Username;
            EmailAddress = userSettings.EmailAddress;
            FirstName = userSettings.FirstName;
            LastName = userSettings.LastName;
        }

        // Database Model Class Attributes
        public int UserId { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string ApiKey { get; set; }
        public string EmailAddress { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}