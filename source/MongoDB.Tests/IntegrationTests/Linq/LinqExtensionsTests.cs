using System.Linq;
using MongoDB.Linq;
using NUnit.Framework;

namespace MongoDB.IntegrationTests.Linq
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
            personCollection = this.TestsDatabase.GetCollection<Person>("people");
            personCollection.Delete(new { }, true);
            personCollection.Insert(new Person { FirstName = "Bob", LastName = "McBob", Age = 42, Address = new Address { City = "London" } }, true);
            personCollection.Insert(new Person { FirstName = "Jane", LastName = "McJane", Age = 35, Address = new Address { City = "Paris" } }, true);
            personCollection.Insert(new Person { FirstName = "Joe", LastName = "McJoe", Age = 21, Address = new Address { City = "Chicago" } }, true);

            orgCollection = this.TestsDatabase.GetCollection<Organization>("orgs");
            orgCollection.Delete(new { }, true);
            orgCollection.Insert(new Organization { Name = "The Muffler Shanty", Address = new Address { City = "London" } }, true);
        }

        [Test]
        public void Delete()
        {
            personCollection.Delete(p => true);

            Assert.AreEqual(0, personCollection.Count());
        }

        [Test]
        public void Find()
        {
            var people = personCollection.Find(x => x.Age > 21).Documents;

            Assert.AreEqual(2, people.Count());
        }

    }
}