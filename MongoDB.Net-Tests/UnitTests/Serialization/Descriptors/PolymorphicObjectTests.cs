using NUnit.Framework;
using MongoDB.Driver.Configuration.Mapping.Auto;
using MongoDB.Driver.Configuration.Mapping;

namespace MongoDB.Driver.UnitTests.Serialization.Descriptors
{
    [TestFixture]
    public class PolymorphicObjectTests : SerializationTestBase
    {
        protected override IMappingStore MappingStore
        {
            get
            {
                var profile = new AutoMappingProfile();
                profile.IsSubClassDelegate = t => t == typeof(ClassA) || t == typeof(ClassB);
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
            var classB = new ClassB() { A = "a", C = "c" };
            string bson = Serialize<ClassB>(classB);
            Assert.AreEqual("JgAAAAJfdAAHAAAAQ2xhc3NCAAJBAAIAAABhAAJDAAIAAABjAAA=", bson);
        }

        [Test]
        public void CanDeserializeDirectlyWithAnonymousType()
        {
            string bson = Serialize<ClassB>(new { A = "a", C = "c" });
            Assert.AreEqual("JgAAAAJfdAAHAAAAQ2xhc3NCAAJBAAIAAABhAAJDAAIAAABjAAA=", bson);
        }

        [Test]
        public void CanDeserializeIndirectly()
        {
            var baseClass = new ClassB() { A = "a", C = "c" };
            string bson = Serialize<BaseClass>(baseClass);
            Assert.AreEqual("JgAAAAJfdAAHAAAAQ2xhc3NCAAJBAAIAAABhAAJDAAIAAABjAAA=", bson);
        }
    }
}