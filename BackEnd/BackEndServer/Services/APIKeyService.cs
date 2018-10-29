using System;
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

        #region Methods For APIKeyService

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
        public string RegisterNewAPIKey()
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

            APIKey api_key_to_persist = new APIKey(salted_hashed_api_key, new_Salt);

            // Attempt to persist the newly created API Key and Salt to the Database. The stored API key is salted and hashed for security reasons.
            bool persistSuccess = _dbQueryService.PersistAPIKey(api_key_to_persist);

            if (persistSuccess == false)
            {
                // LOG ERROR HERE

                return null;
            }

            // Return the newly generated API Key in plain text format (shown on front-end) for the system admin to use.
            return new_APIKey;
        }

        // Attempts to first verify that the key exists in the database and then sets the API key to inactive. 
        public bool UnregisterAPIKey(string unsalted_unhashed_api_key)
        {
            int api_key_id = VerifyAPIKey(unsalted_unhashed_api_key);

            // Check if the API key is in the database and is active. If not, return false.
            if (api_key_id < 0)
            {
                return false;
            }

            // The API key is active, so attempt to deactivate it.
            bool deactivateSuccess = _dbQueryService.DeactivateAPIKey(api_key_id);

            if (deactivateSuccess == false)
            {
                // LOG ERROR HERE
                return false;
            }

            return true;
        }

        // Verifies the specified API Key against the database. Returns the API key's ID in the database if found and is active, if not returns -1.
        public int VerifyAPIKey(string unsalted_unhashed_api_key)
        {
            List<DatabaseAPIKey> db_all_api_keys = _dbQueryService.GetAllAPIKeys();

            if (db_all_api_keys == null)
            {
                return -1;
            }

            foreach(DatabaseAPIKey db_api_key in db_all_api_keys)
            {
                string salted_hashed_api_key_to_check = HashAndSaltAPIKey(unsalted_unhashed_api_key, db_api_key.Salt);

                // Compare the values.
                if (salted_hashed_api_key_to_check == db_api_key.Key)
                {
                    // If they are identical, then the API key was found in the database.
                    if (db_api_key.IsActive == (int)DatabaseAPIKey.API_Key_Status.INACTIVE)
                    {
                        // API key is deactivated. Returns a -2.
                        return -2;
                    }
                    else if (String.IsNullOrWhiteSpace(db_api_key.Key))
                    {
                        // LOG ERROR HERE, the database contained an empty string as one of the api keys. Specify ID.
                        return -3;
                    }
                    else
                    {
                        // API key was found and is Active.
                        return db_api_key.APIKeyId;
                    }
                }
            }

            // If API key was not found, return a -1.
            return -1;
        }

        #endregion
    }
}