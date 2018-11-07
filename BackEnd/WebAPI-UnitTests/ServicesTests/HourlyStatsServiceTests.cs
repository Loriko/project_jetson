using System;
using System.Collections.Generic;
using System.Text;
using BackEndServer.Services;
using BackEndServer.Classes.EntityDefinitionClasses;
using NUnit.Framework;
using NDbUnit.Core;
using NDbUnit.Core.MySqlClient;

namespace WebAPI_UnitTests.ServicesTests
{
    
    [TestFixture]
    class HourlyStatsServiceTests
    {
        private HourlyStatsService _hourlyStatsService;
        /*
        [OneTimeSetUp]
        public void SetUp()
        {
            // No _databaseQueryService initialised for the HourlyStatsService.
            _hourlyStatsService = new HourlyStatsService();
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            _hourlyStatsService = null;
        }

        [Test]
        public void CheckIfCalculationsRequiredTest()
        {
            Assert.Pass();
            /*
            DateTime dt_1 = new DateTime(1999, 8, 30, 4, 25, 0);
            DateTime dt_2 = new DateTime(1999, 8, 30, 4, 59, 0);
            DateTime dt_3 = new DateTime(1999, 8, 30, 4, 59, 59);

            PerSecondStat[] perSecondStats = new PerSecondStat[12];


            //perSecondStats[0] = new PerSecondStat();

            DataMessage input = new DataMessage("TESTING", perSecondStats);
        }
        */
    }
}