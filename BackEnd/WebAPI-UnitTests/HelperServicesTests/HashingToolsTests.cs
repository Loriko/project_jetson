using NUnit.Framework;
using BackEndServer.Services.HelperServices;

namespace WebAPI_UnitTests.HelperServicesTests
{
    class HashingToolsTests
    {
        // Test the hashing of an empty string.
        [Test]
        public void MD5HashToPass_1()
        {
            string expected = "D41D8CD98F00B204E9800998ECF8427E";
            string md5HashResult = HashingTools.MD5Hash("");
            Assert.AreEqual(expected, md5HashResult);
        }

        // Test the hashing of a non-empty string.
        [Test]
        public void MD5HashToPass_2()
        {
            string toHash = "let it rock let it roll let it go";
            string expected = "A0C7DA379F081624DD1AE00762969BCD";
            string md5HashResult = HashingTools.MD5Hash(toHash);
            Assert.AreEqual(expected, md5HashResult);
        }
    }
}