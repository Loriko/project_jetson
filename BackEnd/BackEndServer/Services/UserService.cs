using BackEndServer.Models.DBModels;
using BackEndServer.Models.ViewModels;
using BackEndServer.Services.AbstractServices;
using System.Collections.Generic;

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

        public UserSettings CreateUser(UserSettings userSettings)
        {
            _dbQueryService.PersistNewUser(new DatabaseUser(userSettings));
            UserSettings newUser = new UserSettings(_dbQueryService.GetUserByUsername(userSettings.Username));
            newUser.CreateAPIKey = userSettings.CreateAPIKey;
            return newUser;
        }
    }
}