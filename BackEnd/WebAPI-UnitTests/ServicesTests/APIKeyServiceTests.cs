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

        [OneTimeSetUp]
        public void SetUp()
        {
            // No _databaseQueryService initialised for the APIKeyService.
            _apiKeyService = new APIKeyService();
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            _apiKeyService = null;
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

        // Total input (key + salt) to MD5 is under 32 characters.
        [Test]
        public void HashAndSaltAPIKeyTestToPass_1()
        {
            string salt = "abcde12345";
            string key = "yankeerose";
            // expected value to hash is "abcdeyankeerose12345"
            string expected = "4D8F85FCDF9AB130208255155BA15BEE";
            string generatedString = _apiKeyService.HashAndSaltAPIKey(key, salt);

            if (generatedString.Length != 32)
            {
                Assert.Fail();
            }

            Assert.AreEqual(expected, generatedString);
        }

        // Total input (key + salt) to MD5 is over 32 characters. Salt is 5 characters specificaly.
        [Test]
        public void HashAndSaltAPIKeyTestToPass_2()
        {
            string salt = "abcde";
            string key = "She was a fast machineShe kept her motor cleanShe was the best damn woman I had ever seenShe had the sightless eyesTelling me no liesKnockin' me out with those American thighsTaking more than her shareHad me fighting for airShe told me to come but I was already thereCause the walls start shakingThe earth was quakingMy mind was achingAnd we were making it and you";
            // expected value to hash is "abcdeyankeerose12345"
            string expected = "1ACB77919FF0C49A1459C025230E2544";
            string generatedString = _apiKeyService.HashAndSaltAPIKey(key, salt);

            if (generatedString.Length != 32)
            {
                Assert.Fail();
            }

            Assert.AreEqual(expected, generatedString);
        }
    }
}