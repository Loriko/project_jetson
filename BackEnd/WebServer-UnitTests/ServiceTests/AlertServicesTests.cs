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
using System.Threading;

namespace WebServer_UnitTests.ServiceTests
{
    [TestFixture]
    class AlertServicesTests
    {
        List<DatabaseAlert> dbAlerts;
        [SetUp]
        public void setup()
        {



            DatabaseAlert existingDatabaseAlert = new DatabaseAlert()
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
            DatabaseAlert newDatabaseAlert = new DatabaseAlert()
            {
                AlertId = 0,
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
            dbAlerts.Add(existingDatabaseAlert);
            dbAlerts.Add(newDatabaseAlert);

        }
        [Test]
        public void createAlertTest()
        {
            Mock<IDatabaseQueryService> mockDBService = new Mock<IDatabaseQueryService>(MockBehavior.Strict);
            mockDBService.Setup(x => x.PersistNewAlert(It.Is<DatabaseAlert>(p => p.CameraId == 1))).Returns(true);
            AlertService alertService = new AlertService(mockDBService.Object,
                new CameraService(mockDBService.Object, new GraphStatisticService(mockDBService.Object), new LocationService(mockDBService.Object)),
                new NotificationService(mockDBService.Object));
            AlertDetails alertDetails = new AlertDetails(dbAlerts[1]);


            Assert.That(alertService.SaveAlert(alertDetails), Is.True);
            mockDBService.Verify(m => m.PersistExistingAlert(It.IsAny<DatabaseAlert>()), Times.Never);

        }
        [Test]
        public void getAlertsForASpecifiedUserTest()
        {
            Mock<IDatabaseQueryService> mockDBService = new Mock<IDatabaseQueryService>(MockBehavior.Strict);
            mockDBService.Setup(x => x.GetAllAlerts(1)).Returns(dbAlerts);
            AlertService alertService = new AlertService(mockDBService.Object, 
                new CameraService(mockDBService.Object, new GraphStatisticService(mockDBService.Object), new LocationService(mockDBService.Object)),
                new NotificationService(mockDBService.Object));

            AlertDetails alertDetails = new AlertDetails(dbAlerts[0]);

            Assert.That(alertService.GetAllAlertsForUser(1)[0].AlertId, Is.EqualTo(1));


        }
        [Test]
        public void deleteAnAlertTest()
        {
            Mock<IDatabaseQueryService> mockDBService = new Mock<IDatabaseQueryService>(MockBehavior.Strict);
            mockDBService.Setup(x => x.DeleteAlert(1)).Returns(true);

            AlertService alertService = new AlertService(mockDBService.Object,
                new CameraService(mockDBService.Object, new GraphStatisticService(mockDBService.Object), new LocationService(mockDBService.Object)),
                new NotificationService(mockDBService.Object));


            alertService.DeleteAlert(1);
            mockDBService.Verify(m => m.DeleteAlert(It.IsAny<int>()), Times.Once);
        }

        [Test]
        public void modifyExistingAlertTest()
        {
            Mock<IDatabaseQueryService> mockDBService = new Mock<IDatabaseQueryService>(MockBehavior.Strict);
            mockDBService.Setup(x => x.PersistExistingAlert(It.Is<DatabaseAlert>(p => p.CameraId == 1))).Returns(true);
            AlertService alertService = new AlertService(mockDBService.Object,
                new CameraService(mockDBService.Object, new GraphStatisticService(mockDBService.Object), new LocationService(mockDBService.Object)),
                new NotificationService(mockDBService.Object));
            AlertDetails alertDetails = new AlertDetails(dbAlerts[0]);

            Assert.That(alertService.SaveAlert(alertDetails), Is.True);
            mockDBService.Verify(m => m.PersistExistingAlert(It.IsAny<DatabaseAlert>()), Times.Once);
            mockDBService.Verify(m => m.PersistNewAlert(It.IsAny<DatabaseAlert>()), Times.Never);
        }

        [Test]
        public void getNotificationWhenAlertConditionIsMetTest()
        {
            DatabasePerSecondStat perSecondStat = new DatabasePerSecondStat()
            {
                CameraId = 1,
                DateTime = DateTime.Now,
                DateTimeReceived = DateTime.Now,
                HasSavedImage = false,
                NumDetectedObjects = 1,
                PerSecondStatId = 1,
                PerHourStatId = 1
            };

            Mock<IDatabaseQueryService> mockDBService = new Mock<IDatabaseQueryService>(MockBehavior.Loose);
            List<DatabaseAlert> singleDBAlerts = new List<DatabaseAlert>();
            singleDBAlerts.Add(dbAlerts[0]);
            mockDBService.Setup(x => x.GetAllAlerts(0)).Returns(singleDBAlerts);
            mockDBService.Setup(x => x.GetEarliestPerSecondStatTriggeringAlert(singleDBAlerts[0],It.IsAny<DateTime>(), It.IsAny<DateTime>())).Returns(perSecondStat);
            mockDBService.Setup(x => x.PersistNewNotification(It.Is<DatabaseNotification>(p => p.AlertId == 1))).Returns(true);

            Thread alertMonitoringThread = new Thread(delegate ()
            {
                int snoozeDurationMinutes = 5;
                AlertMonitoringService alertMonitoringService = new AlertMonitoringService(mockDBService.Object, new EmailService("test", "test"), snoozeDurationMinutes);
                alertMonitoringService.StartMonitoring();
            });
            alertMonitoringThread.Start();
            Thread.Sleep(1000);
            mockDBService.Verify(m => m.PersistNewNotification(It.Is<DatabaseNotification>(p => p.AlertId == 1)), Times.Once);
        }
    }
}
