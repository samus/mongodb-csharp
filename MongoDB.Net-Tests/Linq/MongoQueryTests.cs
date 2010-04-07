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
            collection.Insert(new Person { FirstName = "Bob", LastName = "McBob", Age = 42 }, true);
            collection.Insert(new Person { FirstName = "Jane", LastName = "McJane", Age = 35 }, true);
            collection.Insert(new Person { FirstName = "Joe", LastName = "McJoe", Age = 21 }, true);
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
        public void ProjectionWithConstraints()
        {
            var names = (from p in collection.Linq()
                         where p.Age > 21
                         select p.FirstName)
                         .ToList();

            Assert.AreEqual(2, names.Count());
        }

    }
}