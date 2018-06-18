using System;
using System.Security.Cryptography;
using BackEndServer.Services.HelperServices;
using BackEndServer.Services.AbstractServices;
using BackEndServer.Models.DBModels;
using BackEndServer.Models.APIModels;

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

        #region Private Methods

        // Returns a string representing a randomly generated, unhashed and unsalted API Key.
        private string GenerateRandomAPIKey()
        {
            Random random = new Random();
            int randomNumberOfBytes = random.Next(24,32);
            byte[] key_in_bytes = new byte[randomNumberOfBytes];

            using (RNGCryptoServiceProvider randomNumberGenerator = new RNGCryptoServiceProvider())
            {
                randomNumberGenerator.GetBytes(key_in_bytes);
                return Convert.ToBase64String(key_in_bytes);
            }
        }

        // Returns a string representing a randomly generated Salt for an API Key.
        private string GenerateRandomSalt()
        {
            Random random = new Random();
            int randomNumberOfBytes = random.Next(8,16);
            byte[] salt_in_bytes = new byte[randomNumberOfBytes];

            using (RNGCryptoServiceProvider randomNumberGenerator = new RNGCryptoServiceProvider())
            {
                randomNumberGenerator.GetBytes(salt_in_bytes);
                return Convert.ToBase64String(salt_in_bytes);
            }
        }

        // Returns a string representing the input API Key, but in a salted (using input salt) and hashed form.
        private string HashAndSaltAPIKey(string unsalted_unhashed_api_key, string salt)
        {
            // Salted API Key = First five character of Salt + Key + Remaining Characters of Salt
            string salted_api_key = salt.Substring(0,5) + unsalted_unhashed_api_key + salt.Substring(5);

            // Return the MD5 hash of the salted password.
            return HashingTools.MD5Hash(salted_api_key);
        }

        #endregion

        #region Public Methods For Controllers

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

                if (VerifyAPIKey(new_APIKey) == false)
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
            // Check if the API key is in the database and is active. If not, return false.
            if (VerifyAPIKey(unsalted_unhashed_api_key) == false)
            {
                return false;
            }

            // The API key is active, so attempt to deactivate it.
            DatabaseAPIKey db_api_key = _dbQueryService.GetAPIKey(unsalted_unhashed_api_key);
            bool deactivateSuccess = _dbQueryService.DeactivateAPIKey(db_api_key);

            if (deactivateSuccess == false)
            {
                // LOG ERROR HERE
                return false;
            }

            return true;
        }

        // Verifies the specified API Key against the database and returns false if it does not exist or is set to Inactive.
        public bool VerifyAPIKey(string unsalted_unhashed_api_key)
        {
            DatabaseAPIKey db_api_key = _dbQueryService.GetAPIKey(unsalted_unhashed_api_key);

            if (db_api_key == null)
            {
                // API key was not found.
                return false;
            }
            else if (db_api_key.IsActive == (int)DatabaseAPIKey.API_Key_Status.INACTIVE)
            {
                // API key is deactivated.
                return false;
            }
            else if (String.IsNullOrWhiteSpace(db_api_key.API_Key))
            {
                // LOG ERROR HERE, the database contained an empty string as one of the api keys. Specify ID.
                return false;
            }

            return true;
        }

        #endregion
    }
}