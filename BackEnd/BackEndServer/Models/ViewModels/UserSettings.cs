using BackEndServer.Models.DBModels;

namespace BackEndServer.Models.ViewModels
{
    public class UserSettings
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public string EmailAddress { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public UserSettings()
        {
        }

        public UserSettings(DatabaseUser dbUser)
        {
            UserId = dbUser.UserId;
            Username = dbUser.Username;
            EmailAddress = dbUser.EmailAddress;
            FirstName = dbUser.FirstName;
            LastName = dbUser.LastName;
        }
    }
}