using BackEndServer.Services.AbstractServices;

namespace BackEndServer.Services
{
    public class AuthenticationService : AbstractAuthenticationService
    {
        private readonly DatabaseQueryService _dbQueryService = new DatabaseQueryService();
        public bool ValidateCredentials(string username, string password)
        {
            return _dbQueryService.IsPasswordValidForUser(username, password);
        }
    }
}