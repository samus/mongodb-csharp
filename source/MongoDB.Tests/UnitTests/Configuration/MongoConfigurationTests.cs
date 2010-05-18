using System;
using MongoDB.Configuration;
using NUnit.Framework;

namespace MongoDB.UnitTests.Configuration
{
    [TestFixture]
    public class MongoConfigurationTests
    {
        [Test]
        public void IsModifiableByDefault()
        {
            var config = new MongoConfiguration();
            Assert.IsTrue(config.IsModifiable);
        }

        [Test]
        public void IsNotModifiableAfterValidate()
        {
            var config = new MongoConfiguration();
            config.ValidateAndSeal();
            Assert.IsFalse(config.IsModifiable);            
        }

        [Test]    
        [ExpectedException(typeof(InvalidOperationException))]
        public void CanNotChangeConnectionStringAfterValidate()
        {
            var config = new MongoConfiguration();
            config.ValidateAndSeal();
            config.ConnectionString = "";
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void CanNotChangeMappingStoreAfterValidate()
        {
            var config = new MongoConfiguration();
            config.ValidateAndSeal();
            config.MappingStore = null;
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void CanNotChangeReadLocalTimeAfterValidate()
        {
            var config = new MongoConfiguration();
            config.ValidateAndSeal();
            config.ReadLocalTime = true;
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void CanNotChangeSerializationFactoryAfterValidate()
        {
            var config = new MongoConfiguration();
            config.ValidateAndSeal();
            config.SerializationFactory = null;
        }
    }
}