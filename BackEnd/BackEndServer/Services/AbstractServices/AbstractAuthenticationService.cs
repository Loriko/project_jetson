using System;

namespace BackEndServer.Services.AbstractServices
{
    public interface AbstractAuthenticationService
    {
        bool ValidateCredentials(string username, string password);
    }
}