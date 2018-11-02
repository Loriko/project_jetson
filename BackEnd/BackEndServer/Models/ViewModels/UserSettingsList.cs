using System.Collections;
using System.Collections.Generic;
using BackEndServer.Models.DBModels;
using NUnit.Framework;

namespace BackEndServer.Models.ViewModels
{
    public class UserSettingsList
    {

        public List<UserSettings> UserList { get; set; }
        public int CameraId { get; set; }
        
        
        public UserSettingsList(List<DatabaseUser> dbUserList, int cameraId)
        {
            UserList = new List<UserSettings>();
            foreach (var dbUser in dbUserList)
            {
                UserList.Add(new UserSettings(dbUser));
            }

            CameraId = cameraId;
        }
    }
}