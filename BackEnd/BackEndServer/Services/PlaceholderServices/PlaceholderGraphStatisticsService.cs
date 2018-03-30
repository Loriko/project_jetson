using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BackEndServer.Services.AbstractServices;
using BackEndServer.Models.ViewModels;
using System.Collections;
using static System.Random;

namespace BackEndServer.Services.PlaceholderServices
{
    public class PlaceholderGraphStatisticsService : AbstractGraphStatisticService
    {
        private static readonly AbstractCameraService CameraService = new CameraService();
        public GraphStatistics getMaxStatistics(int cameraId)
        {
            GraphStatistics graphStatistics = new GraphStatistics(CameraService.getCameraInformationById(cameraId));

            List<string[]> maxStats = new List<string[]>();
            Random rnd = new Random();

            int hour = DateTime.Now.Hour;
            int counter = hour;

            for (int i = 0; i <= counter; i++)
            {

                string hourString = (hour - i).ToString() + ":00";

                maxStats.Add(new string[2] { hourString, rnd.Next(0, 20).ToString() });
            }
            maxStats.Add(new string[2] { "Time", "People" });
            maxStats.Reverse();
            graphStatistics.Stats = maxStats.ToArray();

            return graphStatistics;

        }
    }
}
