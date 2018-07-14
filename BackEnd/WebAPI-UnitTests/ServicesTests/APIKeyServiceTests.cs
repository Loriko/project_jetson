using BackEndServer.Services;
using NUnit.Framework;
using System;

// https://stackoverflow.com/questions/13378815/base64-length-calculation

namespace WebAPI_UnitTests.ServicesTests
{
    [TestFixture]
    class APIKeyServiceTests
    {
        private APIKeyService _apiKeyService;

        [SetUp]
        public void BaseSetUp()
        {
            // No _databaseQueryService initialised for the APIKeyService.
            _apiKeyService = new APIKeyService();
        }

        [Test]
        public void GenerateRandomAPIKeyTest()
        {
            string generatedString = _apiKeyService.GenerateRandomAPIKey();
            Assert.IsTrue(!String.IsNullOrWhiteSpace(generatedString) && generatedString.Length >= 24 && generatedString.Length <= 48);
        }

        [Test]
        public void GenerateRandomSaltTest()
        {
            string generatedString = _apiKeyService.GenerateRandomSalt();
            Assert.IsTrue(!String.IsNullOrWhiteSpace(generatedString) && generatedString.Length >= 12 && generatedString.Length <= 24);
        }

        [Test]
        public void HashAndSaltAPIKeyTest()
        {
            string salt = "abcde12345";
            string key = "yankeerose";
            // expected value to hash is "abcdeyankeerose12345"
            string expected = "4D8F85FCDF9AB130208255155BA15BEE";
            string generatedString = _apiKeyService.HashAndSaltAPIKey(key, salt);
            Assert.AreEqual(expected, generatedString);
        }
    }
}