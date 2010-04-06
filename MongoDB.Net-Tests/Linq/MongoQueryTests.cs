using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MongoDB.Driver.Linq;

using NUnit.Framework;

namespace MongoDB.Driver.Tests.Linq
{
    [TestFixture]
    public class MongoQueryTests
    {
        private class Person
        {
            public string FirstName { get; set; }

            public string LastName { get; set; }

            public int Age { get; set; }
        }

        private IMongoCollection<Person> collection;

        [SetUp]
        public void TestSetup()
        {
            collection = new Mongo().GetDatabase("tests").GetCollection<Person>("people");
        }

        [Test]
        public void WithoutConstraints()
        {
            var people = collection.Linq();

            var queryObject = ((IMongoQuery)people).GetQueryObject();

            Assert.IsNull(queryObject.Projection);
            Assert.AreEqual(0, queryObject.NumberToLimit);
            Assert.AreEqual(0, queryObject.NumberToSkip);
            Assert.AreEqual(0, queryObject.Order.Count);
            Assert.AreEqual(0, queryObject.Query.Count);
        }

        [Test]
        public void SingleEqualConstraint()
        {
            var people = collection.Linq().Where(p => p.FirstName == "Jack");

            var queryObject = ((IMongoQuery)people).GetQueryObject();

            Assert.IsNull(queryObject.Projection);
            Assert.AreEqual(0, queryObject.NumberToLimit);
            Assert.AreEqual(0, queryObject.NumberToSkip);
            Assert.AreEqual(0, queryObject.Order.Count);
            Assert.AreEqual(new Document("FirstName", "Jack"), queryObject.Query);
        }

        [Test]
        public void ConjuctionConstraint()
        {
            var people = collection.Linq().Where(p => p.Age > 21 && p.Age < 42);

            var queryObject = ((IMongoQuery)people).GetQueryObject();

            Assert.IsNull(queryObject.Projection);
            Assert.AreEqual(0, queryObject.NumberToLimit);
            Assert.AreEqual(0, queryObject.NumberToSkip);
            Assert.AreEqual(0, queryObject.Order.Count);
            Assert.AreEqual(new Document("Age", new Document().Merge(Op.GreaterThan(21)).Merge(Op.LessThan(42))), queryObject.Query);
        }

        [Test]
        public void Simple()
        {
            var people = from p in collection.Linq()
                         select p;

            var queryObject = ((IMongoQuery)people).GetQueryObject();
            Assert.IsNotNull(queryObject.Projection);
            Assert.AreEqual(0, queryObject.NumberToLimit);
            Assert.AreEqual(0, queryObject.NumberToSkip);
            Assert.AreEqual(0, queryObject.Order.Count);
            Assert.AreEqual(0, queryObject.Query.Count);
        }

        [Test]
        public void Projection()
        {
            var people = from p in collection.Linq()
                         select new { Name = p.FirstName + p.LastName };

            var queryObject = ((IMongoQuery)people).GetQueryObject();
            Assert.IsNotNull(queryObject.Projection);
            Assert.AreEqual(2, queryObject.Projection.Fields.Count());
            Assert.AreEqual(0, queryObject.NumberToLimit);
            Assert.AreEqual(0, queryObject.NumberToSkip);
            Assert.AreEqual(0, queryObject.Order.Count);
            Assert.AreEqual(0, queryObject.Query.Count);
        }

        [Test]
        public void ProjectionWithConstraints()
        {
            var people = from p in collection.Linq()
                         where p.Age > 21 && p.Age < 42
                         select new { Name = p.FirstName + p.LastName };

            var queryObject = ((IMongoQuery)people).GetQueryObject();
            Assert.IsNotNull(queryObject.Projection);
            Assert.AreEqual(2, queryObject.Projection.Fields.Count());
            Assert.AreEqual(0, queryObject.NumberToLimit);
            Assert.AreEqual(0, queryObject.NumberToSkip);
            Assert.AreEqual(0, queryObject.Order.Count);
            Assert.AreEqual(new Document("Age", new Document().Merge(Op.GreaterThan(21)).Merge(Op.LessThan(42))), queryObject.Query);
        }
    }
}