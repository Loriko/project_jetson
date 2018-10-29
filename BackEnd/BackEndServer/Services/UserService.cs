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
        
        public UserService(IDatabaseQueryService dbQueryService)
        {
            _dbQueryService = dbQueryService;
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
    }
}