namespace BackEndServer.Services.AbstractServices
{
    interface AbstractAPIKeyService
    {
        string RegisterNewAPIKey(int? userId);
        bool UnregisterAPIKey(string unsalted_unhashed_api_key);
        int VerifyAPIKey(string unsalted_unhashed_api_key);
    }
}