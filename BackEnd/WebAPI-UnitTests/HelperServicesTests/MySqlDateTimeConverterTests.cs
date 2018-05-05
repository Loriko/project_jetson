using System;
using NUnit.Framework;
using BackEndServer.Services.HelperServices;

// http://www.alteridem.net/2017/05/04/test-net-core-nunit-vs2017/

namespace WebAPI_UnitTests.HelperServicesTests
{
    [TestFixture]
    class MySqlDateTimeConverterTests
    {
        [Test]
        public void CheckIfSQLFormatTestToPass()
        {
            // Does not verify validity, only format.
            string test = "1901-28-99 30:60:90"; 
            Assert.AreEqual(true, MySqlDateTimeConverter.CheckIfSQLFormat(test));
        }

        [Test]
        public void CheckIfSQLFormatTestToFail_1()
        {
            string test = "2000-08-3010:23:45";
            Assert.AreEqual(false, MySqlDateTimeConverter.CheckIfSQLFormat(test));
        }

        [Test]
        public void CheckIfSQLFormatTestToFail_2()
        {
            string test = "200-08-30 10:23:45";
            Assert.AreEqual(false, MySqlDateTimeConverter.CheckIfSQLFormat(test));
        }

        [Test]
        public void CheckIfSQLFormatTestToFail_3()
        {
            string test = "2000-8-30 10:23:45";
            Assert.AreEqual(false, MySqlDateTimeConverter.CheckIfSQLFormat(test));
        }

        [Test]
        public void CheckIfSQLFormatTestToFail_4()
        {
            string test = "2000-08-3 10:23:45";
            Assert.AreEqual(false, MySqlDateTimeConverter.CheckIfSQLFormat(test));
        }

        [Test]
        public void CheckIfSQLFormatTestToFail_5()
        {
            string test = "2000/08-30 10:23:45";
            Assert.AreEqual(false, MySqlDateTimeConverter.CheckIfSQLFormat(test));
        }

        [Test]
        public void CheckIfSQLFormatTestToFail_6()
        {
            string test = "2000-08-30 10-23:45";
            Assert.AreEqual(false, MySqlDateTimeConverter.CheckIfSQLFormat(test));
        }

        [Test]
        public void CheckIfSQLFormatTestToFail_7()
        {
            string test = "2000-08-30 10-o3:45";
            Assert.AreEqual(false, MySqlDateTimeConverter.CheckIfSQLFormat(test));
        }

        [Test]
        public void CheckIfSQLFormatTestToFail_8()
        {
            string test = "2000-08-30  10-03:45";
            Assert.AreEqual(false, MySqlDateTimeConverter.CheckIfSQLFormat(test));
        }

        [Test]
        public void ToMySqlDateTimeStringTest()
        {
            DateTime dateTime = new DateTime(2000, 08, 31, 10, 55, 24);
            string mySqlDateTimeString = "2000-08-31 10:55:24";

            if (MySqlDateTimeConverter.CheckIfSQLFormat(mySqlDateTimeString) == false)
            {
                Assert.Fail();
            }

            Assert.AreEqual(mySqlDateTimeString, MySqlDateTimeConverter.ToMySqlDateTimeString(dateTime));
        }

        [Test]
        public void ToDateTimeTest()
        {
            DateTime test = new DateTime(1901,08,30,10,59,20);
            Assert.AreEqual(test, MySqlDateTimeConverter.ToDateTime("1901-08-30 10:59:20"));
        }
    }
}
