using System;

namespace BackEndServer.Services.AbstractServices
{
    public interface AbstractAuthenticationService
    {
        bool validateCredentials(string username, string password);
    }
}