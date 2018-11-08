using System.Collections.Generic;
using BackEndServer.Models.DBModels;
using BackEndServer.Models.ViewModels;

namespace BackEndServer.Services.AbstractServices
{
    public interface AbstractUserService
    {
        UserSettings GetUserSettings(int userId);
        bool ModifyUser(UserSettings userSettings);
        bool ModifyPassword(UserSettings userSettings);
        bool ResetPassword(PasswordReset passwordReset);
        bool SendResetPasswordLink(string email);
        UserSettings CreateAndReturnUser(UserSettings userSettings);
        UserSettings GetUserByUsername(string username);
        UserSettings GetUserByEmailAddress(string email);
        NavigationBarDetails GetNavigationBarDetailsForUser(int? userId);
    }
}