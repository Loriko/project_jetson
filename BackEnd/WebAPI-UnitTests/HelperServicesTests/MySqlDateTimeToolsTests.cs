using System;
using NUnit.Framework;
using BackEndServer.Services.HelperServices;

namespace WebAPI_UnitTests.HelperServicesTests
{
    [TestFixture]
    class MySqlDateTimeToolsTests
    {
        [Test]
        public void IsLastSecondOfHourTestToPass()
        {
            string test = "1805-28-99 65:59:59";
            Assert.IsTrue(MySqlDateTimeTools.IsLastSecondOfHour(test));
        }

        [Test]
        public void IsLastSecondOfHourTestToFail_1()
        {
            string test = "2004-33-12 75:58:59";
            Assert.IsFalse(MySqlDateTimeTools.IsLastSecondOfHour(test));
        }

        [Test]
        public void IsLastSecondOfHourTestToFail_2()
        {
            string test = "2004-33-12 75:59:58";
            Assert.IsFalse(MySqlDateTimeTools.IsLastSecondOfHour(test));
        }
    }
}