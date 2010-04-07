using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MongoDB.Driver.Linq;

using NUnit.Framework;

namespace MongoDB.Driver.Tests.Linq
{
    [TestFixture]
    public class MongoQueryTests : MongoTestBase
    {
        private class Person
        {
            public string FirstName { get; set; }

            public string LastName { get; set; }

            public int Age { get; set; }

            public Address Address { get; set; }
        }

        private class Address
        {
            public string City { get; set; }
        }

        private IMongoCollection<Person> collection;

        public override string TestCollections
        {
            get { return "people"; }
        }

        [SetUp]
        public void TestSetup()
        {
            collection = this.DB.GetCollection<Person>("people");
            collection.Delete(new { }, true);
            collection.Insert(new Person { FirstName = "Bob", LastName = "McBob", Age = 42, Address = new Address { City = "London" } }, true);
            collection.Insert(new Person { FirstName = "Jane", LastName = "McJane", Age = 35, Address = new Address { City = "Paris" } }, true);
            collection.Insert(new Person { FirstName = "Joe", LastName = "McJoe", Age = 21, Address = new Address { City = "Chicago" } }, true);
        }

        [Test]
        public void Simple()
        {
            var people = collection.Linq().ToList();

            Assert.AreEqual(3, people.Count());            
        }

        [Test]
        public void SimpleConstraint()
        {
            var people = collection.Linq().Where(p => p.FirstName == "Bob").ToList();

            Assert.AreEqual(1, people.Count());
        }

        [Test]
        public void NestedClassConstraint()
        {
            var people = collection.Linq().Where(p => p.Address.City == "London").ToList();

            Assert.AreEqual(1, people.Count());
        }

        [Test]
        public void ConjunctiveConstraint()
        {
            var people = collection.Linq().Where(p => p.Age > 21 && p.Age < 42).ToList();

            Assert.AreEqual(1, people.Count());
        }

        [Test]
        public void SimpleProjection()
        {
            var names = (from p in collection.Linq()
                         select new { Name = p.FirstName + " " + p.LastName })
                         .ToList();

            Assert.AreEqual(3, names.Count());
        }

        [Test]
        public void NestedClassProjection()
        {
            var cities = collection.Linq().Select(p => p.Address.City).ToList();

            Assert.AreEqual(3, cities.Count());
        }

        [Test]
        public void ProjectionWithConstraints()
        {
            var names = (from p in collection.Linq()
                         where p.Age > 21
                         select p.FirstName)
                         .ToList();

            Assert.AreEqual(2, names.Count());
        }

        [Test]
        public void ProjectionWithConstraintsInReverse()
        {
            var names = collection.Linq().Select(p => new { FirstName = p.FirstName, Age = p.Age }).Where(n => n.Age > 21).ToList();

            Assert.AreEqual(2, names.Count());
        }

        [Test]
        public void OddQuery()
        {
            var names = collection.Linq().Select(p => new { FirstName = p.FirstName, Age = p.Age }).Where(n => n.Age > 21).Select(t => t.FirstName).ToList();

            Assert.AreEqual(2, names.Count());
        }

    }
}