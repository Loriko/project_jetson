using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using Moq;
using BackEndServer.Services;
using BackEndServer.Models.DBModels;
using BackEndServer.Services.AbstractServices;
namespace WebServer_UnitTests.ServiceTests
{
    [TestFixture]
    class UserServiceTests
    {

        [Test]
        public void authenticateUserTest()
        {
            Mock<IDatabaseQueryService> mockDBService = new Mock<IDatabaseQueryService>(MockBehavior.Strict);
            mockDBService.Setup(x => x.IsPasswordValidForUser("username","password")).Returns(true);
            mockDBService.Setup(x => x.IsPasswordValidForUser("username", "wrongPassword")).Returns(false);
            AuthenticationService authenticationService = new AuthenticationService(mockDBService.Object);
            Assert.That(authenticationService.ValidateCredentials("username","password"), Is.EqualTo(true));
            Assert.That(authenticationService.ValidateCredentials("username", "wrongPassword"), Is.EqualTo(false));

        }
        [Test]
        public void modifyUserDetailsTest()
        {
            Assert.Fail();
        }
    }
}
