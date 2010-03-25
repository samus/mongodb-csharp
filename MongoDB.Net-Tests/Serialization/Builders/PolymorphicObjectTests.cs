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
                profile.IsSubClass = t => t == typeof(ClassA) || t == typeof(ClassB);
                var store = new AutoMappingStore(profile);
                //eagerly automap so they are known at deserialization time...
                store.GetClassMap(typeof(ClassA));
                store.GetClassMap(typeof(ClassB));
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

        [Test]
        public void CanDeserializeDirectly()
        {
            //{_t: "ClassB", A: "a", C: "c" }
            const string bson = "JgAAAAJfdAAHAAAAQ2xhc3NCAAJBAAIAAABhAAJDAAIAAABjAAA=";
            var classB = Deserialize<ClassB>(bson);
            Assert.IsInstanceOfType(typeof(ClassB), classB);
            Assert.AreEqual("a", classB.A);
            Assert.AreEqual("c", classB.C);
        }

        [Test]
        public void CanDeserializeIndirectly()
        {
            //{_t: "ClassB", A: "a", C: "c" }
            const string bson = "JgAAAAJfdAAHAAAAQ2xhc3NCAAJBAAIAAABhAAJDAAIAAABjAAA=";
            var classB = Deserialize<BaseClass>(bson);
            Assert.IsInstanceOfType(typeof(ClassB), classB);
            Assert.AreEqual("a", classB.A);
            Assert.AreEqual("c", ((ClassB)classB).C);
        }
    }
}