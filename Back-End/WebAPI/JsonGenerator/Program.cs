using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JsonGenerator.Classes;
using Newtonsoft.Json;
using System.IO;

namespace JsonGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            PerSecondStats t = new PerSecondStats(5, 1521080558, 18,true);
            PerSecondStats t2 = new PerSecondStats(5, 152108058, 18, true);
            PerSecondStats[] perSecondStats = new PerSecondStats[] { t, t2 };
            DataMessage m = new DataMessage(perSecondStats);
            string json = JsonConvert.SerializeObject(m);
            File.WriteAllText(@"C:\Users\MohamedRamadan\Desktop\DataMessage.txt", json);

            SingleSecondTime singleSecondTime = new SingleSecondTime(1521080559);
            json = JsonConvert.SerializeObject(singleSecondTime);
            File.WriteAllText(@"C:\Users\MohamedRamadan\Desktop\SingleSecondTime.txt", json);

            TimeInterval timeInterval = new TimeInterval(1521080550, 1521080559);
            json = JsonConvert.SerializeObject(timeInterval);
            File.WriteAllText(@"C:\Users\MohamedRamadan\Desktop\TimeInterval.txt", json);

            AveragesOfDayRequest averagesOfDayRequest = new AveragesOfDayRequest(new SingleSecondTime(1521080557));
            json = JsonConvert.SerializeObject(averagesOfDayRequest);
            File.WriteAllText(@"C:\Users\MohamedRamadan\Desktop\AveragesOfDayRequest.txt", json);
        }
    }
}
