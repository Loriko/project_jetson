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
    class LocationServiceTests
    {

        DatabaseLocation testLocation;
        DatabaseLocation testLocation2;

        [OneTimeSetUp]
        public void setup()
        {
            testLocation = new DatabaseLocation
            {
                LocationId = 1,
                AddressLine = "333 King Edwards Dr",
                City = "Ottawa",
                LocationName = "DMV",
                State = "ON",
                Zip = "M2I9R4"
            };
            testLocation2 = new DatabaseLocation
            {
                LocationId = 2,
                AddressLine = "334 King Edwards Dr",
                City = "Ottawa",
                LocationName = "DMV2",
                State = "ON",
                Zip = "M2I9R4"
            };
        }

        [Test]
        public void getLocationByUserIDTest()
        {
            List<DatabaseLocation> dbAddressList = new List<DatabaseLocation>();
            dbAddressList.Add(testLocation);
            dbAddressList.Add(testLocation2);

            Mock<IDatabaseQueryService> mockDBService = new Mock<IDatabaseQueryService>(MockBehavior.Strict);
            LocationService locationService = new LocationService(mockDBService.Object);
            mockDBService.Setup(x => x.GetLocationsForUser(1)).Returns(dbAddressList);

            LocationInformationList locationInformationList = locationService.GetAvailableLocationsForUser(1);
            Assert.That(locationInformationList.LocationList[0].LocationName, Is.EqualTo("DMV"));
            Assert.That(locationInformationList.LocationList[1].LocationName, Is.EqualTo("DMV2"));
        }
        [Test]
        public void modifyExistingLocationTest()
        {
            Mock<IDatabaseQueryService> mockDBService = new Mock<IDatabaseQueryService>(MockBehavior.Strict);
            mockDBService.Setup(x => x.PersistExistingLocation(It.IsAny<DatabaseLocation>())).Returns(true);
            LocationService locationService = new LocationService(mockDBService.Object);
            LocationDetails locationDetails = new LocationDetails(testLocation);


            locationService.SaveLocation(locationDetails);
            mockDBService.Verify(m => m.PersistExistingLocation(It.IsAny<DatabaseLocation>()), Times.AtLeastOnce());
            
        }
        [Test]
        public void createNewLocationTest()
        {
            Mock<IDatabaseQueryService> mockDBService = new Mock<IDatabaseQueryService>(MockBehavior.Strict);
            mockDBService.Setup(x => x.PersistNewLocation(It.IsAny<DatabaseLocation>())).Returns(true);
            LocationService locationService = new LocationService(mockDBService.Object);
            LocationDetails locationDetails = new LocationDetails();


            locationService.SaveLocation(locationDetails);
            mockDBService.Verify(m => m.PersistNewLocation(It.IsAny<DatabaseLocation>()), Times.AtLeastOnce());
        }

        [Test]
        public void deleteLocation()
        {
            Mock<IDatabaseQueryService> mockDBService = new Mock<IDatabaseQueryService>(MockBehavior.Strict);
            mockDBService.Setup(x => x.DeleteLocation(1)).Returns(true);
            mockDBService.Setup(x => x.DeleteRoomsAtLocation(1)).Returns(true);
            LocationService locationService = new LocationService(mockDBService.Object);
            LocationDetails locationDetails = new LocationDetails();


            locationService.DeleteLocation(1);
            mockDBService.Verify(m => m.DeleteLocation(It.IsAny<int>()), Times.AtLeastOnce());
        }
    }
}
