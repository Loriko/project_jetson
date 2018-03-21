using System;
using System.Collections.Generic;
using BackEndServer.Models.DBModels;

namespace BackEndServer.Models.ViewModels
{
    public class CameraInformationList
    {
        public CameraInformationList(List<DatabaseCamera> dbCameraList)
        {
            CameraList = new List<CameraInformation>();
            foreach (var dbCamera in dbCameraList)
            {
                CameraList.Add(new CameraInformation(dbCamera));
            }
        }

        public CameraInformationList()
        {
            CameraList = new List<CameraInformation>();
        }

        public List<CameraInformation> CameraList { get; set; }
    }
}
