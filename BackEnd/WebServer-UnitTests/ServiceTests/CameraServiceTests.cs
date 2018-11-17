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
    public class CameraServiceTests
    {
        [Test]
        public void getCameraDetailsByCameraIDTest()
        {
            DatabaseCamera testCamera = new DatabaseCamera
            {
                CameraId = 1,
                CameraName = "cameraName",
                LocationId = 1,
                UserId = 1
            };
            DatabaseLocation testLocation = new DatabaseLocation
            {
                LocationId = 1,
                AddressLine = "333 King Edwards Dr",
                City = "Ottawa",
                LocationName = "DMV",
                State = "ON",
                Zip = "M2I9R4"
            };
            DatabasePerSecondStat testStat = new DatabasePerSecondStat
            {
                CameraId = 1,
                HasSavedImage = false,
                DateTime = new DateTime(2018, 4, 3, 10, 0, 0),
                NumDetectedObjects = 23,
                PerSecondStatId = 2
            };
            List<DatabasePerSecondStat> listOfStats = new List<DatabasePerSecondStat>();
            listOfStats.Add(testStat);



            Mock<IDatabaseQueryService> mockDBService = new Mock<IDatabaseQueryService>(MockBehavior.Strict);
            mockDBService.Setup(x => x.GetCameraById(1)).Returns(testCamera);
            mockDBService.Setup(x => x.GetLatestPerSecondStatForCamera(It.IsAny<int>())).Returns(testStat);
            mockDBService.Setup(x => x.GetPerSecondStatsForCamera(1)).Returns(listOfStats);
            mockDBService.Setup(x => x.GetLocationById(1)).Returns(testLocation);

            CameraService cameraService = new CameraService(mockDBService.Object, new GraphStatisticService(mockDBService.Object), new LocationService(mockDBService.Object));
            Assert.That(cameraService.getCameraStatisticsForNowById(1).MostRecentPeopleCount.Value, Is.EqualTo(23));

        }
        
        [Test]
        public void registerNewCameraTest()
        {
            DatabaseCamera testCamera = new DatabaseCamera
            {
                CameraId = 1,
                CameraName = "cameraName",
                LocationId = 1,
                UserId = 1
            };

            Mock<IDatabaseQueryService> mockDBService = new Mock<IDatabaseQueryService>(MockBehavior.Strict);
            mockDBService.Setup(x => x.PersistExistingCameraByCameraKey(It.IsAny<DatabaseCamera>(),false)).Returns(true);
            CameraService cameraService = new CameraService(mockDBService.Object, new GraphStatisticService(mockDBService.Object), new LocationService(mockDBService.Object));

            CameraDetails cameraDetails = new CameraDetails(testCamera);
            cameraDetails.ImageDeleted = false;
            cameraDetails.ExistingRoomId = 1;


            Assert.That(cameraService.RegisterCamera(cameraDetails), Is.EqualTo(true));


        }
        [Test]
        public void createNewKeyTest()
        {
            Mock<IDatabaseQueryService> mockDBService = new Mock<IDatabaseQueryService>(MockBehavior.Strict);
            mockDBService.Setup(x => x.GetCameraIdFromKey(It.IsAny<string>())).Returns(-1);
            mockDBService.Setup(x => x.PersistNewCamera(It.IsAny<DatabaseCamera>())).Returns(true);
            CameraService cameraService = new CameraService(mockDBService.Object, new GraphStatisticService(mockDBService.Object), new LocationService(mockDBService.Object));

            NewCameraKey cameraKey = cameraService.GenerateUniqueCameraKey();
            Assert.That(cameraKey.CameraKey.Length, Is.EqualTo(12));


        }
        [Ignore("Not implemented")]
        [Test]
        public void editCameraTest()
        {
            Assert.Fail();
        }


    }
}