using System.Collections.Generic;
using BackEndServer.Models.DBModels;
using BackEndServer.Models.ViewModels;

namespace BackEndServer.Services.AbstractServices
{
    public interface AbstractUserService
    {
        UserSettings GetUserSettings(int userId);
        bool ModifyUser(UserSettings userSettings);
        bool ModifyPassword(UserPassword userPassword);
        bool ResetPassword(PasswordReset passwordReset);
        bool SendResetPasswordLink(string email);
        UserSettings CreateAndReturnUser(UserSettings userSettings);
        UserSettings GetUserByUsername(string username);
        UserSettings GetUserByEmailAddress(string email);
        bool IsUserAdministrator(int userId);
        NavigationBarDetails GetNavigationBarDetailsForUser(int? userId);
        bool ValidateUsername(string username);
        bool ValidateEmail(string emailAddress);
        List<UserSettings> GetUserSettingsForCamera(int cameraId);
    }
}