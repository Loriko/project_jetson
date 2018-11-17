using BackEndServer.Services.AbstractServices;

namespace BackEndServer.Services
{
    public class AuthenticationService : AbstractAuthenticationService
    {
        private readonly IDatabaseQueryService _dbQueryService;

        public AuthenticationService(IDatabaseQueryService dbQueryService)
        {
            this._dbQueryService = dbQueryService;
        }

        public bool ValidateCredentials(string username, string password)
        {
            return _dbQueryService.IsPasswordValidForUser(username, password);
        }
        
        public int? GetUserId(string username)
        {
            return _dbQueryService.GetUserIdByUsername(username);
        }
    }
}