using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NUnit.Framework;
using MongoDB.Driver;
using MongoDB.Driver.Configuration.Mapping.Auto;
using MongoDB.Driver.Configuration.Mapping;

namespace MongoDB.Driver.Serialization.Builders
{
    [TestFixture]
    public class PolymorphicObjectTests : SerializationTestBase
    {
        protected override MongoDB.Driver.Configuration.Mapping.IMappingStore MappingStore
        {
            get
            {
                var profile = new AutoMappingProfile();
                profile.IsSubClass = t => t.IsSubclassOf(typeof(BaseClass));
                var store = new AutoMappingStore(profile);
                //eagerly automap so they are known at deserialization time...
                store.GetClassMap(typeof(ClassA));
                store.GetClassMap(typeof(ClassB));
                store.GetClassMap(typeof(ClassD));
                return store;
            }
        }

        public abstract class BaseClass
        {
            public string A { get; set; }
        }

        public class ClassA : BaseClass
        {
            public string B { get; set; }
        }

        public class ClassB : BaseClass
        {
            public string C { get; set; }
        }

        public class ClassD : ClassA
        {
            public string E { get; set; }
        }

        [Test]
        public void CanDeserializeMiddleClassDirectly()
        {
            var doc = new Document("_t", "ClassB").Add("A", "a").Add("C", "c");
            var bson = Serialize(doc);
            var classB = Deserialize<ClassB>(bson);
            Assert.IsInstanceOfType(typeof(ClassB), classB);
            Assert.AreEqual("a", classB.A);
            Assert.AreEqual("c", classB.C);
        }

        [Test]
        public void CanDeserializeMiddleClassIndirectly()
        {
            var doc = new Document("_t", "ClassB").Add("A", "a").Add("C", "c");
            var bson = Serialize(doc);
            var classB = Deserialize<BaseClass>(bson);
            Assert.IsInstanceOfType(typeof(ClassB), classB);
            Assert.AreEqual("a", classB.A);
            Assert.AreEqual("c", ((ClassB)classB).C);
        }

        [Test]
        public void CanDeserializeLeafClassIndirectly()
        {
            var doc = new Document("_t", new [] { "ClassA", "ClassD" }).Add("A", "a").Add("B", "b").Add("E", "e");
            var bson = Serialize(doc);
            var classD = Deserialize<BaseClass>(bson);
            Assert.IsInstanceOfType(typeof(ClassD), classD);
            Assert.AreEqual("a", classD.A);
            Assert.AreEqual("b", ((ClassA)classD).B);
            Assert.AreEqual("e", ((ClassD)classD).E);
        }
    }
}