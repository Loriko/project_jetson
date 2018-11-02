using System.Collections.Generic;
using BackEndServer.Models.DBModels;
using BackEndServer.Models.ViewModels;

namespace BackEndServer.Services.AbstractServices
{
    public interface AbstractUserService
    {
        UserSettings GetUserSettings(int userId);
        bool ModifyUser(UserSettings userSettings);
        UserSettings CreateAndReturnUser(UserSettings userSettings);
        UserSettings GetUserByUsername(string username);
        string GenerateUniqueAPIKey(int userId);
    }
}