using System.Collections.Generic;
using BackEndServer.Models.DBModels;

namespace BackEndServer.Models.ViewModels
{
    public class CameraKeyList
    {
        public List<CameraKey> ListOfCameraKeys { get; set; }

        public CameraKeyList()
        {
            ListOfCameraKeys = new List<CameraKey>();
        }

        public CameraKeyList (List<DatabaseCamera> dbCameraList)
        {
            ListOfCameraKeys = new List<CameraKey>();

            foreach (var dbCamera in dbCameraList)
            {
                // A camera is registered if it is associated to a User.
                bool isRegistered = false;

                if (dbCamera.UserId != null)
                {
                    if (dbCamera.UserId >= 0)
                    {
                        isRegistered = true;
                    }
                }

                ListOfCameraKeys.Add(new CameraKey(dbCamera.CameraId, dbCamera.CameraKey, isRegistered));
            }
        }
    }
}
