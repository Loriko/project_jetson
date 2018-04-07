using BackEndServer.Services.AbstractServices;

namespace BackEndServer.Services
{
    public class AuthenticationService : AbstractAuthenticationService
    {
        private readonly DatabaseQueryService _dbQueryService;

        public AuthenticationService(DatabaseQueryService dbQueryService)
        {
            this._dbQueryService = dbQueryService;
        }

        public bool ValidateCredentials(string username, string password)
        {
            return _dbQueryService.IsPasswordValidForUser(username, password);
        }
    }
}