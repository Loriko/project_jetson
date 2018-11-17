using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BackEndServer.Models.ViewModels
{
    public class UserPassword
    {
        public int? UserId { get; set; }
        public string NewPassword { get; set; }
        public string OldPassword { get; set; }

        public UserPassword()
        {
        }
    }
}
