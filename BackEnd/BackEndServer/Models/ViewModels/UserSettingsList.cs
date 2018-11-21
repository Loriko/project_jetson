using System.Collections;
using System.Collections.Generic;
using BackEndServer.Models.DBModels;
using NUnit.Framework;

namespace BackEndServer.Models.ViewModels
{
    public class UserSettingsList
    {

        public List<UserSettings> UserList { get; set; }
        public CameraDetails CameraDetails { get; set; }
    }
}