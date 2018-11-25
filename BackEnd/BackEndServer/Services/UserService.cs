using BackEndServer.Models.DBModels;
using BackEndServer.Models.ViewModels;
using BackEndServer.Services.AbstractServices;
using System.Collections.Generic;
using BackEndServer.Services.HelperServices;

namespace BackEndServer.Services
{
    public class UserService : AbstractUserService
    {
        private readonly IDatabaseQueryService _dbQueryService;
        private readonly EmailService _emailService;
        private readonly AbstractNotificationService _notificationService;
        private string _hostname;
        private AbstractAPIKeyService _apiKeyService;

        public UserService(IDatabaseQueryService dbQueryService, AbstractNotificationService notificationService,
            string hostname, EmailService emailService, AbstractAPIKeyService apiKeyService)
        {
            _dbQueryService = dbQueryService;
            _hostname = hostname;
            _emailService = emailService;
            _notificationService = notificationService;
            _apiKeyService = apiKeyService;
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
                    createdUser.APIKey = _apiKeyService.RegisterNewAPIKey(createdUser.UserId);
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

            databaseUser.Salt = UserPasswordTools.GenerateRandomPasswordSalt();
            databaseUser.Password = UserPasswordTools.HashAndSaltPassword(passwordReset.Password, databaseUser.Salt);

            if (_dbQueryService.PersistPasswordChange(databaseUser))
            {
                _dbQueryService.PersistRemovePasswordResetToken(passwordReset.Token);
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
            /*
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
            */

            return StringGenerator.GenerateRandomString(64,64);
        }

        public bool IsUserAdministrator(int userId)
        {
            return _dbQueryService.IsUserAdministrator(userId); 
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

        public bool ValidateEmail(string emailAddress)
        {
            return _dbQueryService.GetUserByEmailAddress(emailAddress) == null;
        }

        public List<UserSettings> GetUserSettingsForCamera(int cameraId)
        {
            List<DatabaseUser> dbUserList = _dbQueryService.GetUsersWithCameraViewAccess(cameraId);
            List<UserSettings> userSettings = new List<UserSettings>();
            foreach (DatabaseUser dbUser in dbUserList)
            {
                userSettings.Add(new UserSettings(dbUser));
            }

            return userSettings;
        }
    }
}