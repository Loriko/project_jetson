namespace BackEndServer.Services.AbstractServices
{
    interface AbstractAPIKeyService
    {
        string RegisterNewAPIKey();
        bool UnregisterAPIKey(string unsalted_unhashed_api_key);
        bool VerifyAPIKey(string unsalted_unhashed_api_key);
    }
}