using System;
using System.Collections.Generic;
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

        public class SetPrivatePropertys
        {
            private double Property { get; set; }

            public double GetProperty() { return Property; }
        }

        [Test]
        public void CanNotSetPrivatePropertys()
        {
            var bson = Serialize(new Document("Property", 4));

            var prop = Deserialize<SetPrivatePropertys>(bson);

            Assert.IsNotNull(prop);
            Assert.AreEqual(0, prop.GetProperty());
        }

        public class NullableProperty
        {
            public double? Value { get; set; }
        }

        [Test]
        public void CanSetNullOnNullablPropertys()
        {
            var bson = Serialize(new Document("Value", null));

            var obj = Deserialize<NullableProperty>(bson);

            Assert.IsNotNull(obj);
            Assert.IsNull(obj.Value);
        }

        [Test]
        public void CanSetValueOnNullablPropertys()
        {
            var bson = Serialize(new Document("Value", 10));

            var obj = Deserialize<NullableProperty>(bson);

            Assert.IsNotNull(obj);
            Assert.AreEqual(10,obj.Value);
        }

        public class GenericDictionary
        {
            public Dictionary<string, int> Property { get; set; }
        }

        [Test]
        public void CanSerializeGenericDictionary()
        {
            var expectedBson = Serialize<Document>(new Document("Property", new Document() { { "key1", 10 }, { "key2", 20 } }));
            var obj = new GenericDictionary { Property = new Dictionary<string, int> { { "key1", 10 }, { "key2", 20 } } };
            var bson = Serialize<GenericDictionary>(obj);
            Assert.AreEqual(expectedBson, bson);
        }

        [Test]
        public void CanDeserializeGenericDictionary()
        {
            var bson = Serialize<Document>(new Document("Property", new Document() { { "key1", 10 }, { "key2", 20 } }));
            var prop = Deserialize<GenericDictionary>(bson);

            Assert.IsNotNull(prop);
            Assert.IsNotNull(prop.Property);
            Assert.AreEqual(2,prop.Property.Count);
            Assert.Contains(new KeyValuePair<string, int>("key1", 10), prop.Property);
            Assert.Contains(new KeyValuePair<string, int>("key2", 20), prop.Property);
        }

        public class GenericStringDictionaryWithComplexType
        {
            public Dictionary<string, GenericDictionaryComplexType> Dict { get; set; }
        }


        public class GenericDictionaryComplexType
        {
            public string Name { get; set; }
        }

        [Test]
        public void CanSerializeStringGenericDictionaryWithComplexType()
        {
            var expectedBson = Serialize<Document>(new Document("Dict", new Document { { "key1", new Document("Name", "a") }, { "key2", new Document("Name", "b") } }));
            var obj = new GenericStringDictionaryWithComplexType { Dict = new Dictionary<string, GenericDictionaryComplexType> { { "key1", new GenericDictionaryComplexType { Name = "a" } }, { "key2", new GenericDictionaryComplexType { Name = "b" } } } };
            var bson = Serialize<GenericStringDictionaryWithComplexType>(obj);
            Assert.AreEqual(expectedBson, bson);
        }

        [Test]
        public void CanDeserializeStringGenericDictionaryWithComplexType()
        {
            var bson = Serialize<Document>(new Document("Dict", new Document { { "key1", new Document("Name", "a") }, { "key2", new Document("Name", "b") } }));
            var prop = Deserialize<GenericStringDictionaryWithComplexType>(bson);

            Assert.IsNotNull(prop);
            Assert.IsNotNull(prop.Dict);
            Assert.AreEqual(2, prop.Dict.Count);
            Assert.IsTrue(prop.Dict["key1"].Name == "a");
            Assert.IsTrue(prop.Dict["key2"].Name == "b");
        }

        public class GenericIntDictionaryWithComplexType
        {
            public Dictionary<int, GenericDictionaryComplexType> Dict { get; set; }
        }

        [Test]
        public void CanSerializeIntGenericDictionaryWithComplexType()
        {
            var expectedBson = Serialize<Document>(new Document("Dict", new Document { { "1", new Document("Name", "a") }, { "2", new Document("Name", "b") } }));
            var obj = new GenericIntDictionaryWithComplexType { Dict = new Dictionary<int, GenericDictionaryComplexType> { { 1, new GenericDictionaryComplexType { Name = "a" } }, { 2, new GenericDictionaryComplexType { Name = "b" } } } };
            var bson = Serialize<GenericIntDictionaryWithComplexType>(obj);
            Assert.AreEqual(expectedBson, bson);
        }

        [Test]
        public void CanDeserializeIntGenericDictionaryWithComplexType()
        {
            var bson = Serialize<Document>(new Document("Dict", new Document { { "1", new Document("Name", "a") }, { "2", new Document("Name", "b") } }));
            var prop = Deserialize<GenericIntDictionaryWithComplexType>(bson);

            Assert.IsNotNull(prop);
            Assert.IsNotNull(prop.Dict);
            Assert.AreEqual(2, prop.Dict.Count);
            Assert.IsTrue(prop.Dict[1].Name == "a");
            Assert.IsTrue(prop.Dict[2].Name == "b");
        }

        public class SortedListDictionary
        {
            public SortedList<string, int> Property { get; set; }
        }

        [Test]
        public void CanSerializeSortedListDictionary()
        {
            var expectedBson = Serialize<Document>(new Document("Property", new Document { { "key1", 10 }, { "key2", 20 } }));
            var obj = new SortedListDictionary { Property = new SortedList<string, int> { { "key1", 10 }, { "key2", 20 } } };
            var bson = Serialize<SortedListDictionary>(obj);
            Assert.AreEqual(expectedBson, bson);
        }

        [Test]
        public void CanDeserializeSortedListDictionary()
        {
            var bson = Serialize<Document>(new Document("Property", new Document { { "key1", 10 }, { "key2", 20 } }));
            var prop = Deserialize<SortedListDictionary>(bson);

            Assert.IsNotNull(prop);
            Assert.IsNotNull(prop.Property);
            Assert.AreEqual(2, prop.Property.Count);
            Assert.Contains(new KeyValuePair<string, int>("key1", 10), prop.Property);
            Assert.Contains(new KeyValuePair<string, int>("key2", 20), prop.Property);
        }
        
        public class HashSetHelper
        {
            public HashSet<string> Property { get; set; }
        }

        [Test]
        public void CanSerializeAndDeserializeHashSet()
        {
            var obj = new HashSetHelper {Property = new HashSet<string> {"test1", "test2"}};
            var bson = Serialize<HashSetHelper>(obj);
            var prop = Deserialize<HashSetHelper>(bson);

            Assert.IsNotNull(prop);
            Assert.IsNotNull(prop.Property);
            Assert.AreEqual(2, prop.Property.Count);

            Assert.IsTrue(prop.Property.Contains("test1"));
            Assert.IsTrue(prop.Property.Contains("test2"));
        }

        public class EnumHelper
        {
            public enum Test
            {
                A=1,
                B=2
            }

            public List<Test> Tests { get; set; }
        }

        [Test]
        public void CanSerializerAndDesializeEnumLists()
        {
            var helper = new EnumHelper {Tests = new List<EnumHelper.Test> {EnumHelper.Test.A}};
            var bson = Serialize<EnumHelper>(helper);
            var deserialize = Deserialize<EnumHelper>(bson);

            Assert.IsNotNull(deserialize);
            Assert.IsNotNull(deserialize.Tests);
            Assert.Contains(EnumHelper.Test.A, deserialize.Tests);
        }

        public class ByteArrayHelper
        {
            public byte[] Property { get; set; }
        }

        [Test]
        public void CanWriteByteArrayPropertyFromBinary()
        {
            var bson = Serialize(new Document("Property", new Binary(new byte[] {1, 2, 3, 4})));

            var helper = Deserialize<ByteArrayHelper>(bson);

            Assert.IsNotNull(helper);
            Assert.AreEqual(4, helper.Property.Length);
            Assert.AreEqual(1, helper.Property[0]);
            Assert.AreEqual(2, helper.Property[1]);
            Assert.AreEqual(3, helper.Property[2]);
            Assert.AreEqual(4, helper.Property[3]);
        }

        public class EmbeddedDocumentHelper
        {
            public Document Document { get; set; }
        }

        [Test]
        public void CanReadEmbeddedDocument()
        {
            var bson = Serialize(new Document("Document", new Document("Embedded",new Document("value", 10))));

            var helper = Deserialize<EmbeddedDocumentHelper>(bson);

            Assert.IsNotNull(helper);
            Assert.IsNotNull(helper.Document);
            Assert.AreEqual(1, helper.Document.Count);

            var embedded = helper.Document["Embedded"] as Document;
            Assert.IsNotNull(embedded);
            Assert.AreEqual(1, embedded.Count);
            Assert.AreEqual(10, embedded["value"]);
        }
    }
}