using System;
using BackEndServer.Models.DBModels;
using BackEndServer.Services;

namespace BackEndServer.Models.ViewModels
{
    public class FrameInformation
    {
        public string FrmJpgRelPath { get; set; }
        public int PeopleCount { get; set; }
        public DateTime Timestamp { get; set; }

        public FrameInformation(DatabasePerSecondStat dbPerSecondStat)
        {
            FrmJpgRelPath = CameraService.GetTempPathFromFullPath(dbPerSecondStat.FrameJpgPath);
            PeopleCount = dbPerSecondStat.NumDetectedObjects;
            Timestamp = dbPerSecondStat.DateTime;
        }
        
        public FrameInformation()
        {
        }
    }
}