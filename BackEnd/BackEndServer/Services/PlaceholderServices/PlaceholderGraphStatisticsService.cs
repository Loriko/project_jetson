using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BackEndServer.Services.AbstractServices;
using BackEndServer.Models.ViewModels;
using System.Collections;
using static System.Random;
using BackEndServer.Models.DBModels;
using BackEndServer.Models.ViewModels;

//SHOULD BE DELETED AT SOME POINT.
//Currently not being used now
namespace BackEndServer.Services.PlaceholderServices
{
    public class PlaceholderGraphStatisticsService
    {
        public GraphStatistics GetYearlyGraphStatistics(int cameraId)
        {
            
            GraphStatistics graphStatistics = new GraphStatistics();

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

        public GraphStatistics GetLast30MinutesStatistics(int cameraId)
        {
            throw new NotImplementedException();
        }
    }
}
