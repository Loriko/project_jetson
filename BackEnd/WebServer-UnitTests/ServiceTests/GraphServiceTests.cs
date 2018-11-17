using BackEndServer.Models.DBModels;
using BackEndServer.Models.ViewModels;
using BackEndServer.Services.AbstractServices;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using Moq;
using BackEndServer.Services;

namespace WebServer_UnitTests.ServiceTests
{
    [TestFixture]
    class GraphServiceTests
    {
        DatabaseGraphStat[] graphStats = new DatabaseGraphStat[3];

        [SetUp]
        public void setup()
        {
 
            for (int c = 0; c < 3; c++)
            {

                DatabaseGraphStat graphStat = new DatabaseGraphStat
                {
                    AverageDetectedObjects = 20 + c,
                    MaximumDetectedObjects = 15 + c + 1,
                    MinimumDetectedObjects = 2 + c + 2,
                    Start = new DateTime(2018, 1, c + 1, 12, 0, 0, 0),
                    End = new DateTime(2018, 1, c + 2, 12, 0, 0, 0),
                };
                graphStats[c] = graphStat;
            }
        }

        [Test]
        public void getCorrectGraphStatFor1DayTest()
        {
            Mock<IDatabaseQueryService> mockDBService = new Mock<IDatabaseQueryService>(MockBehavior.Loose);
            mockDBService.Setup(x => x.getGraphStatByTimeInterval(1, graphStats[0].Start, It.IsAny<DateTime>())).Returns(graphStats[0]);
            mockDBService.Setup(x => x.getGraphStatByTimeInterval(1, graphStats[1].Start, It.IsAny<DateTime>())).Returns(graphStats[1]);
            mockDBService.Setup(x => x.getGraphStatByTimeInterval(1, graphStats[2].Start, It.IsAny<DateTime>())).Returns(graphStats[2]);


            GraphStatisticService graphStatisticsService = new GraphStatisticService(mockDBService.Object);
            GraphStatistics graphStatistics = graphStatisticsService.GetGraphStatisticsByInterval(1, 1514808000, 1514980800, 86400);
            string[][] stats = new string[2][];
            stats[0] = new string[2] { "1515067200", "18" };
            stats[1] = new string[2] { "1514980800", "17" };
            Assert.That(graphStatistics.Stats, Is.EqualTo(stats));

        }
        /**
        [Test]
        public void getReveresedDatesTest()
        {
            Assert.Fail();
        }

        [Test]
        public void noDataForSelectedDatesTest()
        {
            Assert.Fail();
        }
    **/





    }
}
