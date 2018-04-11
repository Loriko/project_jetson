using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using Moq;
using BackEndServer.Services;
using BackEndServer.Models.DBModels;
using BackEndServer.Services.AbstractServices;

namespace BackEndServer.Tests
{
    [TestFixture]
    public class TestCameraService
    {
        [Test]
        public void ShouldRetrieveItem()
        {
            DatabaseCamera testCamera = new DatabaseCamera()
            {
                CameraId = 1,
                CameraName = "cameraName",
                LocationId = 1,
                UserID = 1,
                MonitoredArea = "Room"
            };
            DatabasePerSecondStat testStat = new DatabasePerSecondStat()
            {
                CameraId = 1,
                HasSavedImage = false,
                DateTime = "2018-04-3 10:00:00",
                NumDetectedObjects = 23,
                StatId = 2
            };
            Mock<IDatabaseQueryService> mockDBService = new Mock<IDatabaseQueryService>(MockBehavior.Strict);
            mockDBService.Setup(x => x.GetCameraById(1)).Returns(testCamera);
            mockDBService.Setup(x => x.GetLatestPerSecondStatForCamera(It.IsAny<int>())).Returns(testStat);

            CameraService cameraService = new CameraService(mockDBService.Object);
            Assert.That(cameraService.getCameraStatisticsForNowById(1).MostRecentPeopleCount, Is.EqualTo(23));

        }
    }
}
