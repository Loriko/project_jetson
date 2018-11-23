namespace BackEndServer.Services.AbstractServices
{
    public interface AbstractAPIKeyService
    {
        string RegisterNewAPIKey(int? userId);
        int VerifyAPIKey(string unsalted_unhashed_api_key);
    }
}