using BackEndServer.Services.HelperServices;
using BackEndServer.Services.AbstractServices;
using BackEndServer.Models.DBModels;
using BackEndServer.Models.APIModels;
using System.Collections.Generic;

namespace BackEndServer.Services
{
    public class APIKeyService : AbstractAPIKeyService
    {
        // Attribute
        private readonly DatabaseQueryService _dbQueryService;

        // Constructor
        public APIKeyService(DatabaseQueryService dbQueryService)
        {
            this._dbQueryService = dbQueryService;
        }

        // Constructor for unit testing purposes.
        public APIKeyService()
        {
            this._dbQueryService = null;
        }

        #region Methods Offered By APIKeyService

        // Returns a string representing a randomly generated, unhashed and unsalted API Key.
        public string GenerateRandomAPIKey()
        {
            return StringGenerator.GenerateRandomString(24, 48);
        }

        // Returns a string representing a randomly generated Salt for an API Key.
        public string GenerateRandomSalt()
        {
            return StringGenerator.GenerateRandomString(12, 24);
        }

        // Returns a string representing the input API Key, but in a salted (using input salt) and hashed form.
        public string HashAndSaltAPIKey(string unsalted_unhashed_api_key, string salt)
        {
            // Salted API Key = First five character of Salt + Key + Remaining Characters of Salt
            string salted_api_key = salt.Substring(0,5) + unsalted_unhashed_api_key + salt.Substring(5);

            // Return the MD5 hash of the salted password.
            return HashingTools.MD5Hash(salted_api_key);
        }

        #endregion

        #region Methods For Controllers

        // Creates a new API Key in the database and returns the plain text version for display to the user.
        public string RegisterNewAPIKey(int? userId)
        {
            string new_Salt = GenerateRandomSalt();

            bool unusedAPIKeyGenerated = false;
            string new_APIKey = "";

            // The following loop ensures that no duplicate API Keys are created in the database.
            while (unusedAPIKeyGenerated == false)
            {
                new_APIKey = GenerateRandomAPIKey();

                int databaseCheckResult = VerifyAPIKey(new_APIKey);

                // Criteria for the new key is satisfied only with a -1. 
                // We do not want to reuse a deactivated key (result = -2) or an exisiting active key (result >= 0).
                if (databaseCheckResult == -1)
                {
                    unusedAPIKeyGenerated = true;
                    break;
                } 
            }

            string salted_hashed_api_key = HashAndSaltAPIKey(new_APIKey, new_Salt);

            APIKey api_key_to_persist = new APIKey(salted_hashed_api_key, new_Salt, userId);

            // Attempt to persist the newly created API Key and Salt to the Database. The stored API key is salted and hashed for security reasons.
            bool persistSuccess = _dbQueryService.PersistNewAPIKey(api_key_to_persist);

            if (persistSuccess == false)
            {
                // LOG ERROR HERE

                return null;
            }

            // Return the newly generated API Key in plain text format (shown on front-end) for the system admin to use.
            return new_APIKey;
        }
        
        // Verifies the specified API Key against the database. 
        // Returns a positive integer (>= 0) (the API key's ID in the database) if found.
        // If API Key is not found, returns a -1.
        public int VerifyAPIKey(string unsalted_unhashed_api_key)
        {
            // Get a list of salted and hashed Database API Keys.
            List<DatabaseAPIKey> db_all_api_keys = _dbQueryService.GetAllAPIKeys();

            // If no API Keys exist, return NOT FOUND code immediately.
            if (db_all_api_keys == null)
            {
                return -1;
            }

            // Else, verify the provided API Key against all API Keys in the database.
            foreach (DatabaseAPIKey db_api_key in db_all_api_keys)
            {
                // Using the Database API Key object's Salt attribute, Hash and Salt the plain text API Key to verify/compare.
                string salted_hashed_api_key_to_check = HashAndSaltAPIKey(unsalted_unhashed_api_key, db_api_key.Salt);

                // If the salted and hashed API Keys are identical, then we have a match.
                if (salted_hashed_api_key_to_check == db_api_key.Key)
                {
                    // Return a positive integer (Database API Key's ID), to indicate it is a valid API Key.
                    return db_api_key.APIKeyId;
                }
            }

            // If API key was not found in entire list of Database API Keys, return a -1, indicating it was not found.
            return -1;
        }

        #endregion
    }
}