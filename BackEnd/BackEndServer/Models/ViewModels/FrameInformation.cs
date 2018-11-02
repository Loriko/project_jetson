using System;
using BackEndServer.Models.DBModels;

namespace BackEndServer.Models.ViewModels
{
    public class FrameInformation
    {
        public string FrmJpgPath { get; set; }
        public int PeopleCount { get; set; }
        public DateTime Timestamp { get; set; }

        public FrameInformation(DatabasePerSecondStat dbPerSecondStat)
        {
            FrmJpgPath = dbPerSecondStat.FrameJpgPath;
            PeopleCount = dbPerSecondStat.NumDetectedObjects;
            Timestamp = dbPerSecondStat.DateTime;
        }
        
        public FrameInformation()
        {
        }
    }
}