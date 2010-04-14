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

        private class Organization
        {
            public string Name { get; set; }

            public Address Address { get; set; }
        }

        private IMongoCollection<Person> personCollection;
        private IMongoCollection<Organization> orgCollection;

        public override string TestCollections
        {
            get { return "people"; }
        }

        [SetUp]
        public void TestSetup()
        {
            personCollection = this.DB.GetCollection<Person>("people");
            personCollection.Delete(new { }, true);
            personCollection.Insert(new Person { FirstName = "Bob", LastName = "McBob", Age = 42, Address = new Address { City = "London" } }, true);
            personCollection.Insert(new Person { FirstName = "Jane", LastName = "McJane", Age = 35, Address = new Address { City = "Paris" } }, true);
            personCollection.Insert(new Person { FirstName = "Joe", LastName = "McJoe", Age = 21, Address = new Address { City = "Chicago" } }, true);

            orgCollection = this.DB.GetCollection<Organization>("orgs");
            orgCollection.Delete(new { }, true);
            orgCollection.Insert(new Organization { Name = "The Muffler Shanty", Address = new Address { City = "London" } }, true);
        }

        [Test]
        public void Simple()
        {
            var people = personCollection.Linq().ToList();

            Assert.AreEqual(3, people.Count());            
        }

        [Test]
        public void SimpleConstraint()
        {
            var people = personCollection.Linq().Where(p => p.FirstName == "Bob").ToList();

            Assert.AreEqual(1, people.Count());
        }

        [Test]
        public void NestedClassConstraint()
        {
            var people = personCollection.Linq().Where(p => p.Address.City == "London").ToList();

            Assert.AreEqual(1, people.Count());
        }

        [Test]
        public void ConjunctiveConstraint()
        {
            var people = personCollection.Linq().Where(p => p.Age > 21 && p.Age < 42).ToList();

            Assert.AreEqual(1, people.Count());
        }

        [Test]
        public void SimpleProjection()
        {
            var names = (from p in personCollection.Linq()
                         select new { Name = p.FirstName + " " + p.LastName })
                         .ToList();

            Assert.AreEqual(3, names.Count());
        }

        [Test]
        public void NestedClassProjection()
        {
            var cities = personCollection.Linq().Select(p => p.Address.City).ToList();

            Assert.AreEqual(3, cities.Count());
        }

        [Test]
        public void ProjectionWithConstraints()
        {
            var names = (from p in personCollection.Linq()
                         where p.Age > 21
                         select p.FirstName)
                         .ToList();

            Assert.AreEqual(2, names.Count());
        }

        [Test]
        public void ProjectionWithConstraintsInReverse()
        {
            var names = personCollection.Linq().Select(p => new { FirstName = p.FirstName, Age = p.Age }).Where(n => n.Age > 21).ToList();

            Assert.AreEqual(2, names.Count());
        }

        [Test]
        public void OrderBy()
        {
            var names = personCollection.Linq().Select(p => new { FirstName = p.FirstName, Age = p.Age }).Where(n => n.Age > 21).OrderBy(x => x.Age).ToList();

            Assert.AreEqual(names[0].FirstName, "Jane");
            Assert.AreEqual(names[1].FirstName, "Bob");
        }

        [Test]
        public void OddQuery()
        {
            var names = personCollection.Linq().Select(p => new { FirstName = p.FirstName, Age = p.Age }).Where(n => n.Age > 21).Select(t => t.FirstName).ToList();

            Assert.AreEqual(2, names.Count());
        }

        [Test]
        public void NestedQuery()
        {
            var query = from p in personCollection.Linq()
                        select new
                        {
                            Name = p.FirstName + " " + p.LastName,
                            SameCityOrgs = from o in orgCollection.Linq()
                                           where o.Address.City == p.Address.City
                                           select o.Name
                        };

            var results = query.ToList();
        }

        [Test]
        public void DocumentQuery()
        {
            var query = from p in DB.GetCollection("people").Linq()
                        where p.Key("Age") > 21
                        select (string)p["FirstName"];

            var names = query.ToList();

            Assert.AreEqual(2, names.Count);
        }

    }
}