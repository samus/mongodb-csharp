using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using MongoDB.Linq;
using NUnit.Framework;

namespace MongoDB.IntegrationTests.Linq
{
    [TestFixture]
    public class MongoQueryProviderTests : LinqTestsBase
    {
        [Test]
        public void WithoutConstraints()
        {
            var people = collection.Linq();

            var queryObject = ((IMongoQueryable)people).GetQueryObject();

            Assert.AreEqual(0, queryObject.NumberToLimit);
            Assert.AreEqual(0, queryObject.NumberToSkip);
            Assert.IsNull(queryObject.Query);
        }

        [Test]
        public void SingleEqualConstraint()
        {
            var people = collection.Linq().Where(p => "Jack" == p.FirstName);

            var queryObject = ((IMongoQueryable)people).GetQueryObject();

            Assert.AreEqual(0, queryObject.NumberToLimit);
            Assert.AreEqual(0, queryObject.NumberToSkip);
            Assert.AreEqual(new Document("FirstName", "Jack"), queryObject.Query);
        }

        [Test]
        public void ConjuctionConstraint()
        {
            var people = collection.Linq().Where(p => p.Age > 21 && p.Age < 42);

            var queryObject = ((IMongoQueryable)people).GetQueryObject();

            Assert.AreEqual(0, queryObject.NumberToLimit);
            Assert.AreEqual(0, queryObject.NumberToSkip);
            Assert.AreEqual(new Document("Age", new Document().Merge(Op.GreaterThan(21)).Merge(Op.LessThan(42))), queryObject.Query);
        }

        [Test]
        public void NestedClassConstraint()
        {
            var people = collection.Linq().Where(p => p.PrimaryAddress.City == "my city");

            var queryObject = ((IMongoQueryable)people).GetQueryObject();

            Assert.AreEqual(0, queryObject.NumberToLimit);
            Assert.AreEqual(0, queryObject.NumberToSkip);
            Assert.AreEqual(new Document("PrimaryAddress.City", "my city"), queryObject.Query);
        }

        [Test]
        public void Projection()
        {
            var people = from p in collection.Linq()
                         select new { Name = p.FirstName + p.LastName };

            var queryObject = ((IMongoQueryable)people).GetQueryObject();
            Assert.AreEqual(2, queryObject.Fields.Count());
            Assert.AreEqual(0, queryObject.NumberToLimit);
            Assert.AreEqual(0, queryObject.NumberToSkip);
            Assert.IsNull(queryObject.Query);
        }

        [Test]
        public void ProjectionWithConstraints()
        {
            var people = from p in collection.Linq()
                         where p.Age > 21 && p.Age < 42
                         select new { Name = p.FirstName + p.LastName };

            var queryObject = ((IMongoQueryable)people).GetQueryObject();
            Assert.AreEqual(2, queryObject.Fields.Count());
            Assert.AreEqual(0, queryObject.NumberToLimit);
            Assert.AreEqual(0, queryObject.NumberToSkip);
            Assert.AreEqual(new Document("Age", new Document().Merge(Op.GreaterThan(21)).Merge(Op.LessThan(42))), queryObject.Query);
        }

        [Test]
        public void ConstraintsAgainstLocalVariable()
        {
            int age = 21;
            var people = collection.Linq().Where(p => p.Age > age);

            var queryObject = ((IMongoQueryable)people).GetQueryObject();
            Assert.AreEqual(0, queryObject.NumberToLimit);
            Assert.AreEqual(0, queryObject.NumberToSkip);
            Assert.AreEqual(new Document("Age", Op.GreaterThan(age)), queryObject.Query);
        }

        [Test]
        public void ConstraintsAgainstLocalReferenceMember()
        {
            var local = new { Test = new { Age = 21 } };
            var people = collection.Linq().Where(p => p.Age > local.Test.Age);

            var queryObject = ((IMongoQueryable)people).GetQueryObject();
            Assert.AreEqual(0, queryObject.NumberToLimit);
            Assert.AreEqual(0, queryObject.NumberToSkip);
            Assert.AreEqual(new Document("Age", Op.GreaterThan(local.Test.Age)), queryObject.Query);
        }

        [Test]
        public void OrderBy()
        {
            var people = collection.Linq().OrderBy(x => x.Age).ThenByDescending(x => x.LastName);

            var queryObject = ((IMongoQueryable)people).GetQueryObject();
            Assert.AreEqual(0, queryObject.NumberToLimit);
            Assert.AreEqual(0, queryObject.NumberToSkip);
            Assert.AreEqual(new Document("Age", 1).Add("LastName", -1), queryObject.Sort);
        }

        [Test]
        public void SkipAndTake()
        {
            var people = collection.Linq().Skip(2).Take(1);

            var queryObject = ((IMongoQueryable)people).GetQueryObject();
            Assert.AreEqual(1, queryObject.NumberToLimit);
            Assert.AreEqual(2, queryObject.NumberToSkip);
        }

        [Test]
        public void DocumentQuery()
        {
            var people = from p in documentCollection.Linq()
                         where p.Key("Age") > 21
                         select (string)p["FirstName"];

            var queryObject = ((IMongoQueryable)people).GetQueryObject();
            Assert.AreEqual(new Document("FirstName", 1), queryObject.Fields);
            Assert.AreEqual(0, queryObject.NumberToLimit);
            Assert.AreEqual(0, queryObject.NumberToSkip);
            Assert.AreEqual(new Document("Age", Op.GreaterThan(21)), queryObject.Query);
        }

        [Test]
        public void LocalList_Contains()
        {
            var names = new List<string>() { "Jack", "Bob" };
            var people = collection.Linq().Where(x => names.Contains(x.FirstName));

            var queryObject = ((IMongoQueryable)people).GetQueryObject();
            Assert.AreEqual(0, queryObject.Fields.Count);
            Assert.AreEqual(0, queryObject.NumberToLimit);
            Assert.AreEqual(0, queryObject.NumberToSkip);
            Assert.AreEqual(new Document("FirstName", Op.In("Jack", "Bob")), queryObject.Query);
        }

        [Test]
        public void LocalEnumerable_Contains()
        {
            var names = new[] { "Jack", "Bob" };
            var people = collection.Linq().Where(x => names.Contains(x.FirstName));

            var queryObject = ((IMongoQueryable)people).GetQueryObject();
            Assert.AreEqual(0, queryObject.Fields.Count);
            Assert.AreEqual(0, queryObject.NumberToLimit);
            Assert.AreEqual(0, queryObject.NumberToSkip);
            Assert.AreEqual(new Document("FirstName", Op.In("Jack", "Bob")), queryObject.Query);
        }

        [Test]
        public void String_StartsWith()
        {
            var people = from p in collection.Linq()
                         where p.FirstName.StartsWith("J")
                         select p;

            var queryObject = ((IMongoQueryable)people).GetQueryObject();
            Assert.AreEqual(0, queryObject.Fields.Count);
            Assert.AreEqual(0, queryObject.NumberToLimit);
            Assert.AreEqual(0, queryObject.NumberToSkip);
            Assert.AreEqual(new Document("FirstName", new MongoRegex("^J")), queryObject.Query);
        }

        [Test]
        public void String_EndsWith()
        {
            var people = from p in collection.Linq()
                         where p.FirstName.EndsWith("e")
                         select p;

            var queryObject = ((IMongoQueryable)people).GetQueryObject();
            Assert.AreEqual(0, queryObject.Fields.Count);
            Assert.AreEqual(0, queryObject.NumberToLimit);
            Assert.AreEqual(0, queryObject.NumberToSkip);
            Assert.AreEqual(new Document("FirstName", new MongoRegex("e$")), queryObject.Query);
        }

        [Test]
        public void String_Contains()
        {
            var people = from p in collection.Linq()
                         where p.FirstName.Contains("o")
                         select p;

            var queryObject = ((IMongoQueryable)people).GetQueryObject();
            Assert.AreEqual(0, queryObject.Fields.Count);
            Assert.AreEqual(0, queryObject.NumberToLimit);
            Assert.AreEqual(0, queryObject.NumberToSkip);
            Assert.AreEqual(new Document("FirstName", new MongoRegex("o")), queryObject.Query);
        }

        [Test]
        public void Regex_IsMatch()
        {
            var people = from p in collection.Linq()
                         where Regex.IsMatch(p.FirstName, "Joe")
                         select p;

            var queryObject = ((IMongoQueryable)people).GetQueryObject();
            Assert.AreEqual(0, queryObject.Fields.Count);
            Assert.AreEqual(0, queryObject.NumberToLimit);
            Assert.AreEqual(0, queryObject.NumberToSkip);
            Assert.AreEqual(new Document("FirstName", new MongoRegex("Joe")), queryObject.Query);
        }

        [Test]
        public void NestedArray_Length()
        {
            var people = from p in collection.Linq()
                         where p.EmployerIds.Length == 1
                         select p;

            var queryObject = ((IMongoQueryable)people).GetQueryObject();
            Assert.AreEqual(0, queryObject.Fields.Count);
            Assert.AreEqual(0, queryObject.NumberToLimit);
            Assert.AreEqual(0, queryObject.NumberToSkip);
            Assert.AreEqual(new Document("EmployerIds", Op.Size(1)), queryObject.Query);
        }

        [Test]
        public void NestedCollection_Count()
        {
            var people = from p in collection.Linq()
                         where p.Addresses.Count == 1
                         select p;

            var queryObject = ((IMongoQueryable)people).GetQueryObject();
            Assert.AreEqual(0, queryObject.Fields.Count);
            Assert.AreEqual(0, queryObject.NumberToLimit);
            Assert.AreEqual(0, queryObject.NumberToSkip);
            Assert.AreEqual(new Document("Addresses", Op.Size(1)), queryObject.Query);
        }

        [Test]
        public void Nested_Queryable_Count()
        {
            var people = collection.Linq().Where(x => x.Addresses.Count() == 1);

            var queryObject = ((IMongoQueryable)people).GetQueryObject();
            Assert.AreEqual(0, queryObject.Fields.Count);
            Assert.AreEqual(0, queryObject.NumberToLimit);
            Assert.AreEqual(0, queryObject.NumberToSkip);
            Assert.AreEqual(new Document("Addresses", Op.Size(1)), queryObject.Query);
        }

        [Test]
        public void Nested_Queryable_ElementAt()
        {
            var people = collection.Linq().Where(x => x.Addresses.ElementAt(1).City == "Tokyo");

            var queryObject = ((IMongoQueryable)people).GetQueryObject();
            Assert.AreEqual(0, queryObject.Fields.Count);
            Assert.AreEqual(0, queryObject.NumberToLimit);
            Assert.AreEqual(0, queryObject.NumberToSkip);
            Assert.AreEqual(new Document("Addresses.1.City", "Tokyo"), queryObject.Query);
        }

        [Test]
        public void NestedArray_indexer()
        {
            var people = collection.Linq().Where(x => x.EmployerIds[0] == 1);

            var queryObject = ((IMongoQueryable)people).GetQueryObject();
            Assert.AreEqual(0, queryObject.Fields.Count);
            Assert.AreEqual(0, queryObject.NumberToLimit);
            Assert.AreEqual(0, queryObject.NumberToSkip);
            Assert.AreEqual(new Document("EmployerIds.0", 1), queryObject.Query);
        }

        [Test]
        public void NestedList_indexer()
        {
            var people = collection.Linq().Where(x => x.Addresses[1].City == "Tokyo");

            var queryObject = ((IMongoQueryable)people).GetQueryObject();
            Assert.AreEqual(0, queryObject.Fields.Count);
            Assert.AreEqual(0, queryObject.NumberToLimit);
            Assert.AreEqual(0, queryObject.NumberToSkip);
            Assert.AreEqual(new Document("Addresses.1.City", "Tokyo"), queryObject.Query);
        }

        [Test]
        public void NestedQueryable_Contains()
        {
            var people = collection.Linq().Where(x => x.EmployerIds.Contains(1));

            var queryObject = ((IMongoQueryable)people).GetQueryObject();
            Assert.AreEqual(0, queryObject.Fields.Count);
            Assert.AreEqual(0, queryObject.NumberToLimit);
            Assert.AreEqual(0, queryObject.NumberToSkip);
            Assert.AreEqual(new Document("EmployerIds", 1), queryObject.Query);
        }

        [Test]
        public void NestedQueryable_Any()
        {
            var people = collection.Linq().Where(x => x.Addresses.Any(a => a.City == "London"));

            var queryObject = ((IMongoQueryable)people).GetQueryObject();
            Assert.AreEqual(0, queryObject.Fields.Count);
            Assert.AreEqual(0, queryObject.NumberToLimit);
            Assert.AreEqual(0, queryObject.NumberToSkip);
            Assert.AreEqual(new Document("Addresses", new Document("$elemMatch", new Document("City", "London"))), queryObject.Query);
        }

        [Test]
        [Ignore("Something is interesting about document comparison that causes this to fail.")]
        public void Disjunction()
        {
            var people = collection.Linq().Where(x => x.Age == 21 || x.Age == 35);

            var queryObject = ((IMongoQueryable)people).GetQueryObject();
            Assert.AreEqual(0, queryObject.Fields.Count);
            Assert.AreEqual(0, queryObject.NumberToLimit);
            Assert.AreEqual(0, queryObject.NumberToSkip);
            Assert.AreEqual(new Document("$where", new Code("((this.Age === 21) || (this.Age === 35))")), queryObject.Query);
        }

        [Test]
        public void Chained()
        {
            var people = collection.Linq()
                .Select(x => new { Name = x.FirstName + x.LastName, Age = x.Age })
                .Where(x => x.Age > 21)
                .Select(x => x.Name);

            var queryObject = ((IMongoQueryable)people).GetQueryObject();
            Assert.AreEqual(2, queryObject.Fields.Count);
            Assert.AreEqual(0, queryObject.NumberToLimit);
            Assert.AreEqual(0, queryObject.NumberToSkip);
            Assert.AreEqual(new Document("Age", Op.GreaterThan(21)), queryObject.Query);
        }
    }
}