using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using Moq;
using BackEndServer.Services;
using BackEndServer.Models.DBModels;
using BackEndServer.Services.AbstractServices;
using BackEndServer.Models.ViewModels;

namespace WebServer_UnitTests.ServiceTests
{
    [TestFixture]
    class AlertServicesTests
    {
        List<DatabaseAlert> dbAlerts;
        [SetUp]
        public void setup()
        {



            DatabaseAlert databaseAlert = new DatabaseAlert()
            {
                AlertId = 1,
                AlertName = "test alert",
                AlwaysActive = true,
                CameraId = 1,
                ContactMethod = "Notification",
                DisabledUntil = null,
                EndTime = "",
                SnoozedUntil = null,
                StartTime = "",
                TriggerNumber = 1,
                TriggerOperator = "More",
                UserId = 1,
            };
            dbAlerts = new List<DatabaseAlert>();
            dbAlerts.Add(databaseAlert);

        }
        [Ignore("Not implemented")]
        [Test]
        public void createAlertTest()
        {
            Assert.Fail();
        }
        [Test]
        public void getAlertsForASpecifiedUserTest()
        {
            Mock<IDatabaseQueryService> mockDBService = new Mock<IDatabaseQueryService>(MockBehavior.Strict);
            mockDBService.Setup(x => x.GetAllAlerts(1)).Returns(dbAlerts);
            AlertService alertService = new AlertService(mockDBService.Object, new CameraService(mockDBService.Object, new GraphStatisticService(mockDBService.Object), new LocationService(mockDBService.Object)));

            AlertDetails alertDetails = new AlertDetails(dbAlerts[0]);

            Assert.That(alertService.GetAllAlertsForUser(1)[0].AlertId, Is.EqualTo(1));


        }
        [Ignore("Not implemented")]
        [Test]
        public void deleteAnAlertTest()
        {
            Assert.Fail();
        }
        [Ignore("Not implemented")]
        [Test]
        public void modifyExistingAlertTest()
        {
            Assert.Fail();
        }
    }
}
