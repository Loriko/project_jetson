using System;
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
        public string Password { get; set; }
        public bool CreateAPIKey { get; set; }
        public string APIKey { get; set; }
        public bool IsAdministrator { get; set; }

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
            Password = dbUser.Password;
            IsAdministrator = dbUser.IsAdministrator;
        }
    }
}