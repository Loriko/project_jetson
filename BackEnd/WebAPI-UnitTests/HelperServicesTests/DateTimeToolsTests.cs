using System;
using NUnit.Framework;
using BackEndServer.Services.HelperServices;
using NUnit.Framework.Constraints;

namespace WebAPI_UnitTests.HelperServicesTests
{
    [TestFixture]
    class DateTimeToolsTests
    {
        [Test]
        public void GetDayBeginningTest()
        {
            DateTime expected = new DateTime(1999, 12, 31, 0, 0, 0);
            DateTime test = new DateTime(1999, 12, 31, 14, 54, 57);
            Assert.AreEqual(expected, DateTimeTools.GetDayBeginning(test));
        }

        [Test]
        public void GetDayEndTest()
        {
            DateTime expected = new DateTime(1999, 12, 31, 23, 59, 59);
            DateTime test = new DateTime(1999, 12, 31, 14, 54, 57);
            Assert.AreEqual(expected, DateTimeTools.GetDayEnd(test));
        }

        [Test]
        public void ValidateDateTimeStringToPass()
        {
            Assert.IsTrue(DateTimeTools.ValidateDateTimeString("1985-07-20 22:26:38"));
        }

        [Test]
        public void ValidateDateTimeStringToFail_1()
        {
            // Should fail due to valid datetime issues.
            var ex = Assert.Throws<FormatException>(() => DateTimeTools.ValidateDateTimeString("1985-11-32 22:26:38"));
            Assert.IsTrue(ex.Message.Contains("Invalid SQL DateTime Format When Parsing."));
        }

        [Test]
        public void ValidateDateTimeStringToFail_2()
        {
            // Should fail due to format issues.
            Assert.IsFalse(DateTimeTools.ValidateDateTimeString("1985/11/24 22:26:38"));
        }

        [Test]
        public void ValidateDateTimeToPass_1()
        {
            DateTime test = new DateTime(1900, 1, 1, 1, 1, 1);
            Assert.IsTrue(DateTimeTools.ValidateDateTime(test));
        }

        [Test]
        public void ValidateDateTimeToPass_2()
        {
            DateTime test = new DateTime(9999, 12, 31, 23, 59, 59);
            Assert.IsTrue(DateTimeTools.ValidateDateTime(test));
        }

        [Test]
        public void ValidateDateTimeToFail()
        {
            DateTime test = new DateTime(1899, 12, 31, 23, 59, 59);
            Assert.IsFalse(DateTimeTools.ValidateDateTime(test));
        }

        // All other scenarios are covered by .Net with thrown exceptions on DateTime Constructor.
        // https://msdn.microsoft.com/en-us/library/272ba130(v=vs.110).aspx

        [Test]
        public void GetHourBeginningTest()
        {
            DateTime expected = new DateTime(2005, 6, 3, 10, 0, 0);
            DateTime test = new DateTime(2005, 6, 3, 10, 45, 36);
            Assert.AreEqual(expected, DateTimeTools.GetHourBeginning(test));
        }

        [Test]
        public void GetHourEndTest()
        {
            DateTime expected = new DateTime(2005, 6, 3, 10, 59, 59);
            DateTime test = new DateTime(2005, 6, 3, 10, 45, 36);
            Assert.AreEqual(expected, DateTimeTools.GetHourEnd(test));
        }
    }
}