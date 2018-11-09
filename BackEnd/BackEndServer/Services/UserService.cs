using BackEndServer.Models.DBModels;
using BackEndServer.Models.ViewModels;
using BackEndServer.Services.AbstractServices;
using System.Collections.Generic;
using BackEndServer.Services.HelperServices;
using System.Security.Cryptography;
using System;
using System.Text;

namespace BackEndServer.Services
{
    public class UserService : AbstractUserService
    {
        private readonly IDatabaseQueryService _dbQueryService;
        private readonly EmailService _emailService;
        private readonly AbstractNotificationService _notificationService;
        private string _hostname;


        public UserService(IDatabaseQueryService dbQueryService, AbstractNotificationService notificationService, 
                           string hostname, EmailService emailService)
        {
            _dbQueryService = dbQueryService;
            _hostname = hostname;
            _emailService = emailService;
            _notificationService = notificationService;
        }

        public UserSettings GetUserSettings(int userId)
        {
            DatabaseUser databaseUser = _dbQueryService.GetUserById(userId);
            return new UserSettings(databaseUser);
        }

        public bool ModifyUser(UserSettings userSettings)
        {
            return _dbQueryService.PersistExistingUser(new DatabaseUser(userSettings));
        }
        public bool ModifyPassword(UserSettings userSettings)
        {
            return _dbQueryService.PersistPasswordChange(new DatabaseUser(userSettings));
        }

        public UserSettings CreateAndReturnUser(UserSettings userSettings)
        {
            if (_dbQueryService.PersistNewUser(new DatabaseUser(userSettings)))
            {
                UserSettings createdUser = GetUserByUsername(userSettings.Username);
                createdUser.CreateAPIKey = userSettings.CreateAPIKey;
                if (createdUser.CreateAPIKey)
                {
                    createdUser.APIKey = GenerateUniqueAPIKey(createdUser.UserId);
                }
                return createdUser;
            }

            return null;
        }

        public UserSettings GetUserByUsername(string username)
        {
            return new UserSettings(_dbQueryService.GetUserByUsername(username));
        }
        public UserSettings GetUserByEmailAddress(string email)
        {
            DatabaseUser databaseUser = _dbQueryService.GetUserByEmailAddress(email);
            if(databaseUser == null)
            {
                return null;
            }
            else
            {
                return new UserSettings(databaseUser);
            }
        }
        public bool ResetPassword(PasswordReset passwordReset)
        {
            DatabaseUser databaseUser = _dbQueryService.GetUserByPasswordResetToken(passwordReset.Token);
            if (databaseUser == null)
            {
                return false;
            }
            databaseUser.Password = passwordReset.Password;
            if (_dbQueryService.PersistPasswordChange(databaseUser))
            {
                _dbQueryService.PersistRemovePasswordResetToken(databaseUser.UserId);
                return true;
            }
            else
            {
                return false;
            }

        }
        public bool SendResetPasswordLink(string email)
        {
            string token = GeneratePasswordResetToken();
            string url = "http://" + _hostname + "/User/PasswordReset?id=" + token;
            DatabaseUser user = _dbQueryService.GetUserByEmailAddress(email);
            if (user == null)
            {
                return false;
            }
            PasswordResetLink passwordResetLink = new PasswordResetLink(url,email, user.FirstName + " " + user.LastName);

            if(SendResetPasswordLink(passwordResetLink))
            {
                return _dbQueryService.PersistPasswordResetToken(token, email);
            }
            else
            {
                return false;
            }
        }
        
        private bool SendResetPasswordLink(PasswordResetLink passwordResetLink)
        {
            string emailSubject = "Project Jetson Password Reset";
            string emailBody = GetEmailBody(passwordResetLink);
            return _emailService.SendEmail(passwordResetLink.Email, emailSubject, emailBody);
        }
        private string GetEmailBody(PasswordResetLink passwordResetLink)
        {
            string emailBody =
                RazorEngineWrapper.RunCompile("Views/User", "PasswordResetEmailBodyTemplate.cshtml", passwordResetLink);
            return emailBody;
        }
        private string GeneratePasswordResetToken()
        {
            int length = 64;
            const string valid = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            StringBuilder res = new StringBuilder();
            using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
            {
                byte[] uintBuffer = new byte[sizeof(uint)];

                while (length-- > 0)
                {
                    rng.GetBytes(uintBuffer);
                    uint num = BitConverter.ToUInt32(uintBuffer, 0);
                    res.Append(valid[(int)(num % (uint)valid.Length)]);
                }
            }

            return res.ToString();
        }

        public string GenerateUniqueAPIKey(int userId)
        {
            int count = 0;
            while (count < 5)
            {
                count++;
                // API Key must be exactly 12 characters.
                string randomAPIKey = StringGenerator.GenerateRandomString(12, 12);

                // Ensure Key does not exist in database (return value is -1).
                if (_dbQueryService.GetAPIKeyIdFromKey(randomAPIKey) == -1)
                {
                    // Persist new api key to database.
                    DatabaseAPIKey apiKey = new DatabaseAPIKey
                    {
                        Key = randomAPIKey,
                        UserId = userId,
                        Salt = "placeholder"
                    };

                    if (_dbQueryService.PersistNewAPIKey(apiKey))
                    {
                        return randomAPIKey;
                    }

                    return null;
                }
            }

            return null;
        }

        public NavigationBarDetails GetNavigationBarDetailsForUser(int? userId)
        {
            NavigationBarDetails barDetails = new NavigationBarDetails();
            if (userId != null && userId.Value > 0)
            {
                barDetails.NotificationList = _notificationService.GetNotificationsForUser(userId.Value);
                barDetails.SignedIn = true;
                barDetails.IsAdministrator = _dbQueryService.IsUserAdministrator(userId.Value);
            }
            else
            {
                barDetails.SignedIn = false;
                barDetails.IsAdministrator = false;
            }
            
            return barDetails;
        }

        public bool ValidateUsername(string username)
        {
            return _dbQueryService.GetUserByUsername(username) == null;
        }
    }
}