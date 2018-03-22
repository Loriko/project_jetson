using BackEndServer.Services.AbstractServices;

namespace BackEndServer.Services.PlaceholderServices
{
    public class PlaceholderAuthenticationService : AbstractAuthenticationService
    {
        public bool ValidateCredentials(string username, string password)
        {
            if (username == "johndoe" && password == "Hunter12")
            {
                return true;
            }
            else if (username == "bobDylan321" && password == "thisIsNotAPassword321")
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}