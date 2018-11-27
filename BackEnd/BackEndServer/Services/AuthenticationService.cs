using BackEndServer.Models.DBModels;
using BackEndServer.Services.AbstractServices;
using BackEndServer.Services.HelperServices;
using System.Collections.Generic;

namespace BackEndServer.Services
{
    public class AuthenticationService : AbstractAuthenticationService
    {
        private readonly IDatabaseQueryService _dbQueryService;

        public AuthenticationService(IDatabaseQueryService dbQueryService)
        {
            this._dbQueryService = dbQueryService;
        }

        public bool ValidateCredentials(string username, string unsalted_unhashed_password)
        {
            // Get a list of users which in the database, have salted and hashed passwords.
            List<DatabaseUser> db_users = _dbQueryService.GetAllUsers();

            // If no Users exist, return false.
            if (db_users == null)
            {
                return false;
            }

            // Else, verify the provided credentials against all user credentials in the database.
            foreach (DatabaseUser user in db_users)
            {
                if (user.Username.ToUpper() == username.ToUpper()) // If plaintext username is correct, check the password.
                {
                    // Using the DatabaseUser's Salt attribute, Hash and Salt the plain text password to verify/compare.
                    string salted_hashed_password_to_check = UserPasswordTools.HashAndSaltPassword(unsalted_unhashed_password, user.Salt);

                    // If the salted and hashed passwords are identical, then we have a match.
                    if (salted_hashed_password_to_check == user.Password)
                    {
                        return true;
                    }
                }
            }

            return false;
        }
        
        public int? GetUserId(string username)
        {
            return _dbQueryService.GetUserIdByUsername(username);
        }
    }
}