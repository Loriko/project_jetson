using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            string cameraKey_1 = "AFRJNILIJHRU";
            string cameraKey_2 = "HGTIBNERMESD";
            string cameraKey_3 = "EPOVHTRKMQZU";

            string invalidCameraKey = "123ABC_Invalid_Key";

            // We must have the data insertion script insert a precalculated key and salt for testing purposes.
            string fakeAPIKey = "FAKE_API_KEY_123";

            // Test 1: Three cameras for 4 seconds total. Expected to PASS.
            PerSecondStat s1 = new PerSecondStat("2000-08-30 10:23:45", cameraKey_1, 5, false);
            PerSecondStat s2 = new PerSecondStat("2000-08-30 10:23:45", cameraKey_2, 3, false);
            PerSecondStat s3 = new PerSecondStat("2000-08-30 10:23:45", cameraKey_3, 4, false);

            PerSecondStat s4 = new PerSecondStat("2000-08-30 10:23:46", cameraKey_1, 4, false);
            PerSecondStat s5 = new PerSecondStat("2000-08-30 10:23:46", cameraKey_2, 4, false);
            PerSecondStat s6 = new PerSecondStat("2000-08-30 10:23:46", cameraKey_3, 4, false);

            PerSecondStat s7 = new PerSecondStat("2000-08-30 10:23:47", cameraKey_1, 15, true);
            PerSecondStat s8 = new PerSecondStat("2000-08-30 10:23:47", cameraKey_2, 2, false);
            PerSecondStat s9 = new PerSecondStat("2000-08-30 10:23:47", cameraKey_3, 5, false);

            PerSecondStat s10 = new PerSecondStat("2000-08-30 10:23:42", cameraKey_1, 15, false);
            PerSecondStat s11 = new PerSecondStat("2000-08-30 10:23:42", cameraKey_2, 2, false);
            PerSecondStat s12 = new PerSecondStat("2000-08-30 10:23:42", cameraKey_3, 8, true);

            PerSecondStat[] test1 = new PerSecondStat[] { s1,s2,s3,s4,s5,s6,s7,s8,s9,s10,s11,s12 };
            DataMessage d1 = new DataMessage(fakeAPIKey, test1);
            System.IO.File.WriteAllText(@"C:\Users\Mohamed\Desktop\DataReceival_Test1.json", JsonConvert.SerializeObject(d1));

            // Test 2: Expected to FAIL due to invalid DateTime format.
            s1 = new PerSecondStat("2000-08-30 10:23:48", cameraKey_1, 1, false); 
            s2 = new PerSecondStat("2000-08-30 10:23:49", cameraKey_1, 1, false);
            s3 = new PerSecondStat("2000-8-30 10:23:50", cameraKey_1, 2, false); // Error here in month.
            s4 = new PerSecondStat("2000-08-30 10:23:51", cameraKey_1, 2, false);

            PerSecondStat[] test2 = new PerSecondStat[] { s1, s2, s3, s4 };
            DataMessage d2 = new DataMessage(fakeAPIKey, test2);
            System.IO.File.WriteAllText(@"C:\Users\Mohamed\Desktop\DataReceival_Test2.json", JsonConvert.SerializeObject(d2));

            // Test 3: Expected to FAIL due to invalid CameraId.
            s1 = new PerSecondStat("2000-08-30 10:23:48", invalidCameraKey, 1, false); // Error here.
            s2 = new PerSecondStat("2000-08-30 10:23:48", cameraKey_1, 1, false);

            PerSecondStat[] test3 = new PerSecondStat[] { s1, s2 };
            DataMessage d3 = new DataMessage(fakeAPIKey, test3);
            System.IO.File.WriteAllText(@"C:\Users\Mohamed\Desktop\DataReceival_Test3.json", JsonConvert.SerializeObject(d3));

            // Test 4: Expected to FAIL due to invalid NumTrackedPeople.
            s1 = new PerSecondStat("2000-08-30 10:23:48", cameraKey_1, 1, false);
            s2 = new PerSecondStat("2000-08-30 10:23:49", cameraKey_1, -1, false); // Error here, negative integer.

            PerSecondStat[] test4 = new PerSecondStat[] { s1, s2 };
            DataMessage d4 = new DataMessage(fakeAPIKey, test4);
            System.IO.File.WriteAllText(@"C:\Users\Mohamed\Desktop\DataReceival_Test4.json", JsonConvert.SerializeObject(d4));

            // Test 5: Expected to FAIL due to multiple reasons (which should by identified in the HTTP response from API).
            s1 = new PerSecondStat("20000-08-30 10:23:48", invalidCameraKey, 1, false); // Invalid camera key and datetime format errors.
            s2 = new PerSecondStat("2000-08-30 10:23:48", cameraKey_1, -8, false); // Negative integer for num of people error.
            s3 = new PerSecondStat("2000/08/30 10:23:48", cameraKey_1, 5, false); // DateTime format error.

            PerSecondStat[] test5 = new PerSecondStat[] { s1, s2, s3 };
            DataMessage d5 = new DataMessage(fakeAPIKey, test5);
            System.IO.File.WriteAllText(@"C:\Users\Mohamed\Desktop\DataReceival_Test5.json", JsonConvert.SerializeObject(d5));

        }
    }
}
