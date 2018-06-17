using BackEndServer.Models.DBModels;

namespace BackEndServer.Services.AbstractServices
{
    interface AbstractAPIKeyService
    {
        string RegisterNewAPIKey();
        string GenerateRandomAPIKey();
        string GenerateRandomSalt();
        bool PersistAPIKey(DatabaseAPIKey apiKey);
        bool UnregisterAPIKey(string key);
        bool RemoveAPIKey(DatabaseAPIKey apiKey);
        bool VerifyAPIKey(string key);
    }
}