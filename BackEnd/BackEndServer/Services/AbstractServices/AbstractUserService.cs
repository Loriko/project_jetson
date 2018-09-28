using BackEndServer.Models.ViewModels;

namespace BackEndServer.Services.AbstractServices
{
    public interface AbstractUserService
    {
        UserSettings GetUserSettings(int userId);
        bool ModifyUser(UserSettings userSettings);
    }
}