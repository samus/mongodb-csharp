using System.Collections.Generic;
using NUnit.Framework;

namespace MongoDB.Driver.Serialization.Builders
{
    [TestFixture]
    public class ExtendedPropertiesTests : SerializationTestBase
    {
        public class IDictionaryProperty
        {
            public IDictionary<string, object> ExtendedProperties { get; private set; }
        }

        [Test]
        public void CanDeserializePropertiesWithoutMapsUsingAnIDictionary()
        {
            //{ A: { B: "b" } }
            const string bson = "FgAAAANBAA4AAAACQgACAAAAYgAAAA==";
            var prop = Deserialize<IDictionaryProperty>(bson);
            Assert.IsNotNull(prop);
            Assert.IsNotNull(prop.ExtendedProperties["A"]);
            Assert.AreEqual("b", ((Document)prop.ExtendedProperties["A"])["B"]);
        }

        public class DictionaryProperty
        {
            public Dictionary<string, object> ExtendedProperties { get; private set; }
        }

        [Test]
        public void CanDeserializePropertiesWithoutMapsUsingADictionary()
        {
            //{ A: { B: "b" } }
            const string bson = "FgAAAANBAA4AAAACQgACAAAAYgAAAA==";
            var prop = Deserialize<DictionaryProperty>(bson);
            Assert.IsNotNull(prop);
            Assert.IsNotNull(prop.ExtendedProperties["A"]);
            Assert.AreEqual("b", ((Document)prop.ExtendedProperties["A"])["B"]);
        }

        public class DocumentProperty
        {
            public Document ExtendedProperties { get; private set; }
        }

        [Test]
        public void CanDeserializePropertiesWithoutMapsUsingADocument()
        {
            //{ A: { B: "b" } }
            const string bson = "FgAAAANBAA4AAAACQgACAAAAYgAAAA==";
            var prop = Deserialize<DocumentProperty>(bson);
            Assert.IsNotNull(prop);
            Assert.IsNotNull(prop.ExtendedProperties["A"]);
            Assert.AreEqual("b", ((Document)prop.ExtendedProperties["A"])["B"]);
        }
    }
}