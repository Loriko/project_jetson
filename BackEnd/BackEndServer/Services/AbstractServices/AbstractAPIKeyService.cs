namespace BackEndServer.Services.AbstractServices
{
    interface AbstractAPIKeyService
    {
        string RegisterNewAPIKey(int? userId);
        int VerifyAPIKey(string unsalted_unhashed_api_key);
    }
}