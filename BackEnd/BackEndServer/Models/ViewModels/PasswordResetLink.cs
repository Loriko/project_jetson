using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BackEndServer.Models.ViewModels
{
    public class PasswordResetLink
    {
        public string URL { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }

        public PasswordResetLink(string uRL, string email, string name)
        {
            URL = uRL;
            Email = email;
            Name = name;
        }

        public PasswordResetLink()
        {
        }
    }
}
