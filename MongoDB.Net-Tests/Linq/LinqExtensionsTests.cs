using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace MongoDB.Driver.Linq
{
    [TestFixture]
    public class LinqExtensionsTests : MongoTestBase
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
        public void Find()
        {
            var people = personCollection.Find(x => x.Age > 21).Documents;

            Assert.AreEqual(2, people.Count());
        }

    }
}