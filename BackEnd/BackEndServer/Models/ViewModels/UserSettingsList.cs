using System.Collections;
using System.Collections.Generic;
using BackEndServer.Models.DBModels;
using NUnit.Framework;

namespace BackEndServer.Models.ViewModels
{
    public class UserSettingsList
    {

        public List<UserSettings> UserList { get; set; }
        public List<string> Names { get; set; }
        public int CameraId { get; set; }
        
        
        public UserSettingsList(List<DatabaseUser> dbUserList, int cameraId, List<string> userCameraList)
        {
            UserList = new List<UserSettings>();
            foreach (var dbUser in dbUserList)
            {
                UserList.Add(new UserSettings(dbUser));
            }
            Names = new List<string>();

            foreach (var userCamAss in userCameraList)
            {
                Names.Add(userCamAss);
            }

            CameraId = cameraId;
        }
    }
}