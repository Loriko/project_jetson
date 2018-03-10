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


            
            PerSecondStats t = new PerSecondStats(1,2000,9,20,12,34,50,5);
            DataMessage m = new DataMessage(1);
            m.RealTimeStats[0] = t;

            string json = JsonConvert.SerializeObject(m);

            System.IO.File.WriteAllText(@"C:\Users\MohamedRamadan\Desktop\DataMessage.txt", json);
        }
    }
}
