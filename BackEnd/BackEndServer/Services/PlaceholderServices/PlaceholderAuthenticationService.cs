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

        public int? GetUserId(string username)
        {
            if (username == "johndoe")
            {
                return 1;
            }
            else if (username == "bobDylan321")
            {
                return 2;
            }
            else
            {
                return null;
            }
        }
    }
}