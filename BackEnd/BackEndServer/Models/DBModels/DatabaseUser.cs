using BackEndServer.Models.ViewModels;
using BackEndServer.Services.HelperServices;

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
        public static readonly string SALT_LABEL = "salt";
        public static readonly string EMAIL_ADDRESS_LABEL = "email_address";
        public static readonly string FIRST_NAME_LABEL = "first_name";
        public static readonly string LAST_NAME_LABEL = "last_name";
        public static readonly string PASSWORD_RESET_TOKEN_LABEL = "password_reset_token";
        public static readonly string IS_ADMINISTRATOR_LABEL = "is_administrator";

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
            Salt = UserPasswordTools.GenerateRandomPasswordSalt();
            Password = UserPasswordTools.HashAndSaltPassword(userSettings.Password, this.Salt);
            IsAdministrator = userSettings.IsAdministrator;
        }

        // Database Model Class Attributes
        public int UserId { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Salt { get; set; }
        public string EmailAddress { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool IsAdministrator { get; set; }
    }
}