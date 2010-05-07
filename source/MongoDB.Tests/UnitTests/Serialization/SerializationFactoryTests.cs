using System;
using MongoDB.Configuration;
using MongoDB.Serialization;
using NUnit.Framework;

namespace MongoDB.UnitTests.Serialization
{
    [TestFixture]
    public class SerializationFactoryTests : SerializationTestBase
    {
        [Test]
        public void GetBsonReaderSettingsDefaults()
        {
            var factory = new SerializationFactory();
            var readerSettings = factory.GetBsonReaderSettings(typeof(int));
            Assert.AreEqual(readerSettings.ReadLocalTime,true);
            Assert.IsNotNull(readerSettings.Builder);
        }

        [Test]
        public void ReadLocalTimeCanBeChangedByConfig()
        {
            var factory = new SerializationFactory(new MongoConfiguration {ReadLocalTime = false});
            var readerSettings = factory.GetBsonReaderSettings(typeof(int));
            Assert.AreEqual(readerSettings.ReadLocalTime, false);
        }

        public class ProtectedConstructor
        {
            protected ProtectedConstructor(){}
        }

        [Test]
        public void CanCreateObjectFromProtectedConstructor()
        {
            var obj = Deserialize<ProtectedConstructor>(EmptyDocumentBson);

            Assert.IsNotNull(obj);
        }

        public class PrivateConstructor
        {
            private PrivateConstructor() { }
        }

        [Test]
        [ExpectedException(typeof(MissingMethodException))]
        public void CanNotCreateObjectFromPrivateConstructor()
        {
            var obj = Deserialize<PrivateConstructor>(EmptyDocumentBson);

            Assert.IsNotNull(obj);
        }

        public class SetProtectedPropertys
        {
            protected double Property { get; set; }

            public double GetProperty() {return Property; }
        }

        [Test]
        public void CanSetProtectedProperty()
        {
            var bson = Serialize(new Document("Property", 4));

            var prop = Deserialize<SetProtectedPropertys>(bson);

            Assert.IsNotNull(prop);
            Assert.AreEqual(4, prop.GetProperty());
        }
    }
}