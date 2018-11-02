using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BackEndServer.Models.ViewModels
{
    public class PasswordReset
    {
        public string Token { get; set; }
        public string Password { get; set; }


        public PasswordReset()
        {
        }

        public PasswordReset(string token, string password)
        {
            Token = token;
            Password = password;
        }
    }
}
