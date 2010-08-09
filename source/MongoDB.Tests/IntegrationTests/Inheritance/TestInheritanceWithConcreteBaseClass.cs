using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace MongoDB.IntegrationTests.Inheritance
{
    [TestFixture]
    public class TestInheritanceWithConcreteBaseClass : MongoTestBase
    {
        abstract class Animal
        {
            public Oid Id { get; set; }

            public int Age { get; set; }
        }

        class Bear : Animal
        { }

        abstract class Cat : Animal
        { }

        class Tiger : Cat
        { }

        class Lion : Cat
        { }

        public override string TestCollections
        {
            get { return "Animal"; }
        }

        protected override Configuration.MongoConfigurationBuilder GetConfiguration()
        {
            var builder = base.GetConfiguration();
            builder.Mapping(mapping =>
            {
                mapping.DefaultProfile(profile =>
                {
                    profile.SubClassesAre(x => x.IsSubclassOf(typeof(Animal)));
                });

                mapping.Map<Bear>();
                mapping.Map<Cat>();
                mapping.Map<Tiger>();
                mapping.Map<Lion>();
            });

            return builder;
        }

        [Test]
        public void Should_persist_discriminator_using_base_class_collection()
        {
            var animalCollection = DB.GetCollection<Animal>();
            animalCollection.Save(new Bear() { Age = 20 });
            animalCollection.Save(new Tiger() { Age = 19 });

            var docCollection = DB.GetCollection<Document>("Animal");

            var docs = docCollection.FindAll().Sort("Age", IndexOrder.Ascending).Documents.ToList();

            Assert.AreEqual(new[] { "Cat", "Tiger" }, (List<string>)docs[0]["_t"]);
            Assert.AreEqual("Bear", (string)docs[1]["_t"]);
        }

        [Test]
        public void Should_persist_discriminator_using_concrete_class_collection()
        {
            var animalCollection = DB.GetCollection<Cat>();
            animalCollection.Save(new Lion() { Age = 20 });
            animalCollection.Save(new Tiger() { Age = 19 });

            var docCollection = DB.GetCollection<Document>("Animal");

            var docs = docCollection.FindAll().Sort("Age", IndexOrder.Ascending).Documents.ToList();

            Assert.AreEqual(new[] { "Cat", "Tiger" }, (List<string>)docs[0]["_t"]);
            Assert.AreEqual(new[] { "Cat", "Lion" }, (List<string>)docs[1]["_t"]);
        }

        [Test]
        public void Should_fetch_with_base_class_collection()
        {
            var animalCollection = DB.GetCollection<Animal>();
            animalCollection.Save(new Bear() { Age = 20 });
            animalCollection.Save(new Tiger() { Age = 19 });

            var animals = animalCollection.FindAll().Sort("Age", IndexOrder.Ascending).Documents.ToList();

            Assert.AreEqual(2, animals.Count);
            Assert.IsInstanceOfType(typeof(Tiger), animals[0]);
            Assert.AreEqual(19, animals[0].Age);
            Assert.IsInstanceOfType(typeof(Bear), animals[1]);
            Assert.AreEqual(20, animals[1].Age);
        }

        [Test]
        public void Should_fetch_with_base_class_collection_through_linq()
        {
            var animalCollection = DB.GetCollection<Animal>();
            animalCollection.Save(new Bear() { Age = 20 });
            animalCollection.Save(new Tiger() { Age = 19 });

            var animals = (from a in animalCollection.Linq()
                           orderby a.Age ascending
                           select a).ToList();

            Assert.AreEqual(2, animals.Count);
            Assert.IsInstanceOfType(typeof(Tiger), animals[0]);
            Assert.AreEqual(19, animals[0].Age);
            Assert.IsInstanceOfType(typeof(Bear), animals[1]);
            Assert.AreEqual(20, animals[1].Age);
        }

        [Test]
        public void Should_fetch_with_concrete_class_collection()
        {
            var animalCollection = DB.GetCollection<Animal>();
            animalCollection.Save(new Bear() { Age = 20 });
            animalCollection.Save(new Tiger() { Age = 19 });

            var catCollection = DB.GetCollection<Cat>();

            var cats = catCollection.FindAll().Sort("Age", IndexOrder.Ascending).Documents.ToList();

            Assert.AreEqual(1, cats.Count);
            Assert.IsInstanceOfType(typeof(Tiger), cats[0]);
            Assert.AreEqual(19, cats[0].Age);
        }

        [Test]
        public void Should_fetch_with_concrete_class_collection_through_linq()
        {
            var animalCollection = DB.GetCollection<Animal>();
            animalCollection.Save(new Bear() { Age = 20 });
            animalCollection.Save(new Tiger() { Age = 19 });

            var catCollection = DB.GetCollection<Cat>();

            var animals = (from a in catCollection.Linq()
                           orderby a.Age ascending
                           select a).ToList();

            Assert.AreEqual(1, animals.Count);
            Assert.IsInstanceOfType(typeof(Tiger), animals[0]);
            Assert.AreEqual(19, animals[0].Age);
        }

        [Test]
        public void Should_get_correct_count_with_base_class_collection()
        {
            var animalCollection = DB.GetCollection<Animal>();
            animalCollection.Save(new Bear() { Age = 20 });
            animalCollection.Save(new Tiger() { Age = 19 });

            Assert.AreEqual(2, animalCollection.Count());
        }

        [Test]
        public void Should_get_correct_count_with_base_class_collection_using_linq()
        {
            var animalCollection = DB.GetCollection<Animal>();
            animalCollection.Save(new Bear() { Age = 20 });
            animalCollection.Save(new Tiger() { Age = 19 });

            Assert.AreEqual(2, animalCollection.Linq().Count());
        }

        [Test]
        public void Should_get_correct_count_with_concrete_class_collection()
        {
            var animalCollection = DB.GetCollection<Animal>();
            animalCollection.Save(new Bear() { Age = 20 });
            animalCollection.Save(new Tiger() { Age = 19 });

            var catCollection = DB.GetCollection<Cat>();

            Assert.AreEqual(1, catCollection.Count());
        }

        [Test]
        public void Should_get_correct_count_with_concrete_class_collection_using_linq()
        {
            var animalCollection = DB.GetCollection<Animal>();
            animalCollection.Save(new Bear() { Age = 20 });
            animalCollection.Save(new Tiger() { Age = 19 });

            var catCollection = DB.GetCollection<Cat>();

            Assert.AreEqual(1, catCollection.Linq().Count());
        }
    }
}