﻿using System.Collections.Generic;
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
        public void Boolean1()
        {
            var people = Collection.Linq().Where(x => x.PrimaryAddress.IsInternational);

            var queryObject = ((IMongoQueryable)people).GetQueryObject();
            Assert.AreEqual(new Document("PrimaryAddress.IsInternational", true), queryObject.Query);
        }

        [Test]
        public void Boolean_Inverse()
        {
            var people = Collection.Linq().Where(x => !x.PrimaryAddress.IsInternational);

            var queryObject = ((IMongoQueryable)people).GetQueryObject();
            Assert.AreEqual(new Document("PrimaryAddress.IsInternational", false), queryObject.Query);
        }

        [Test]
        public void Boolean_In_Conjunction()
        {
            var people = Collection.Linq().Where(x => x.PrimaryAddress.IsInternational && x.Age > 21);

            var queryObject = ((IMongoQueryable)people).GetQueryObject();
            Assert.AreEqual(new Document("PrimaryAddress.IsInternational", true).Add("Age", Op.GreaterThan(21)), queryObject.Query);
        }

        [Test]
        public void Chained()
        {
            var people = Collection.Linq()
                .Select(x => new {Name = x.FirstName + x.LastName, fluffy = x.Age})
                .Where(x => x.fluffy > 21)
                .Select(x => x.Name);

            var queryObject = ((IMongoQueryable)people).GetQueryObject();
            Assert.AreEqual(2, queryObject.Fields.Count);
            Assert.AreEqual(0, queryObject.NumberToLimit);
            Assert.AreEqual(0, queryObject.NumberToSkip);
            Assert.AreEqual(new Document("Age", Op.GreaterThan(21)), queryObject.Query);
        }

        [Test]
        public void ConjuctionConstraint()
        {
            var people = Collection.Linq().Where(p => p.Age > 21 && p.Age < 42);

            var queryObject = ((IMongoQueryable)people).GetQueryObject();

            Assert.AreEqual(0, queryObject.NumberToLimit);
            Assert.AreEqual(0, queryObject.NumberToSkip);
            Assert.AreEqual(new Document("Age", new Document().Merge(Op.GreaterThan(21)).Merge(Op.LessThan(42))), queryObject.Query);
        }

        [Test]
        public void ConstraintsAgainstLocalReferenceMember()
        {
            var local = new {Test = new {Age = 21}};
            var people = Collection.Linq().Where(p => p.Age > local.Test.Age);

            var queryObject = ((IMongoQueryable)people).GetQueryObject();
            Assert.AreEqual(0, queryObject.NumberToLimit);
            Assert.AreEqual(0, queryObject.NumberToSkip);
            Assert.AreEqual(new Document("Age", Op.GreaterThan(local.Test.Age)), queryObject.Query);
        }

        [Test]
        public void ConstraintsAgainstLocalVariable()
        {
            var age = 21;
            var people = Collection.Linq().Where(p => p.Age > age);

            var queryObject = ((IMongoQueryable)people).GetQueryObject();
            Assert.AreEqual(0, queryObject.NumberToLimit);
            Assert.AreEqual(0, queryObject.NumberToSkip);
            Assert.AreEqual(new Document("Age", Op.GreaterThan(age)), queryObject.Query);
        }

        [Test]
        [Ignore("Something is interesting about document comparison that causes this to fail.")]
        public void Disjunction()
        {
            var people = Collection.Linq().Where(x => x.Age == 21 || x.Age == 35);

            var queryObject = ((IMongoQueryable)people).GetQueryObject();
            Assert.AreEqual(0, queryObject.Fields.Count);
            Assert.AreEqual(0, queryObject.NumberToLimit);
            Assert.AreEqual(0, queryObject.NumberToSkip);
            Assert.AreEqual(new Document("$where", new Code("((this.Age === 21) || (this.Age === 35))")), queryObject.Query);
        }

        [Test]
        public void DocumentQuery()
        {
            var people = from p in DocumentCollection.Linq()
                         where p.Key("Age") > 21
                         select (string)p["FirstName"];

            var queryObject = ((IMongoQueryable)people).GetQueryObject();
            Assert.AreEqual(new Document("FirstName", 1), queryObject.Fields);
            Assert.AreEqual(0, queryObject.NumberToLimit);
            Assert.AreEqual(0, queryObject.NumberToSkip);
            Assert.AreEqual(new Document("Age", Op.GreaterThan(21)), queryObject.Query);
        }

        [Test]
        public void Enum()
        {
            var people = Collection.Linq().Where(x => x.PrimaryAddress.AddressType == AddressType.Company);

            var queryObject = ((IMongoQueryable)people).GetQueryObject();
            Assert.AreEqual(0, queryObject.Fields.Count);
            Assert.AreEqual(0, queryObject.NumberToLimit);
            Assert.AreEqual(0, queryObject.NumberToSkip);
            Assert.AreEqual(new Document("PrimaryAddress.AddressType", (int)AddressType.Company), queryObject.Query);
        }

        [Test]
        public void LocalEnumerable_Contains()
        {
            var names = new[] {"Jack", "Bob"};
            var people = Collection.Linq().Where(x => names.Contains(x.FirstName));

            var queryObject = ((IMongoQueryable)people).GetQueryObject();
            Assert.AreEqual(0, queryObject.Fields.Count);
            Assert.AreEqual(0, queryObject.NumberToLimit);
            Assert.AreEqual(0, queryObject.NumberToSkip);
            Assert.AreEqual(new Document("FirstName", Op.In("Jack", "Bob")), queryObject.Query);
        }

        [Test]
        public void LocalList_Contains()
        {
            var names = new List<string> {"Jack", "Bob"};
            var people = Collection.Linq().Where(x => names.Contains(x.FirstName));

            var queryObject = ((IMongoQueryable)people).GetQueryObject();
            Assert.AreEqual(0, queryObject.Fields.Count);
            Assert.AreEqual(0, queryObject.NumberToLimit);
            Assert.AreEqual(0, queryObject.NumberToSkip);
            Assert.AreEqual(new Document("FirstName", Op.In("Jack", "Bob")), queryObject.Query);
        }

        [Test]
        public void NestedArray_Contains()
        {
            var people = from p in Collection.Linq()
                         where p.EmployerIds.Contains(1)
                         select p;

            var queryObject = ((IMongoQueryable)people).GetQueryObject();
            Assert.AreEqual(0, queryObject.Fields.Count);
            Assert.AreEqual(0, queryObject.NumberToLimit);
            Assert.AreEqual(0, queryObject.NumberToSkip);
            Assert.AreEqual(new Document("EmployerIds", 1), queryObject.Query);
        }

        [Test]
        public void NestedArray_Length()
        {
            var people = from p in Collection.Linq()
                         where p.EmployerIds.Length == 1
                         select p;

            var queryObject = ((IMongoQueryable)people).GetQueryObject();
            Assert.AreEqual(0, queryObject.Fields.Count);
            Assert.AreEqual(0, queryObject.NumberToLimit);
            Assert.AreEqual(0, queryObject.NumberToSkip);
            Assert.AreEqual(new Document("EmployerIds", Op.Size(1)), queryObject.Query);
        }

        [Test]
        public void NestedArray_indexer()
        {
            var people = Collection.Linq().Where(x => x.EmployerIds[0] == 1);

            var queryObject = ((IMongoQueryable)people).GetQueryObject();
            Assert.AreEqual(0, queryObject.Fields.Count);
            Assert.AreEqual(0, queryObject.NumberToLimit);
            Assert.AreEqual(0, queryObject.NumberToSkip);
            Assert.AreEqual(new Document("EmployerIds.0", 1), queryObject.Query);
        }

        [Test]
        public void NestedClassConstraint()
        {
            var people = Collection.Linq().Where(p => p.PrimaryAddress.City == "my city");

            var queryObject = ((IMongoQueryable)people).GetQueryObject();

            Assert.AreEqual(0, queryObject.NumberToLimit);
            Assert.AreEqual(0, queryObject.NumberToSkip);
            Assert.AreEqual(new Document("PrimaryAddress.City", "my city"), queryObject.Query);
        }

        [Test]
        public void NestedCollection_Count()
        {
            var people = from p in Collection.Linq()
                         where p.Addresses.Count == 1
                         select p;

            var queryObject = ((IMongoQueryable)people).GetQueryObject();
            Assert.AreEqual(0, queryObject.Fields.Count);
            Assert.AreEqual(0, queryObject.NumberToLimit);
            Assert.AreEqual(0, queryObject.NumberToSkip);
            Assert.AreEqual(new Document("Addresses", Op.Size(1)), queryObject.Query);
        }

        [Test]
        public void NestedList_indexer()
        {
            var people = Collection.Linq().Where(x => x.Addresses[1].City == "Tokyo");

            var queryObject = ((IMongoQueryable)people).GetQueryObject();
            Assert.AreEqual(0, queryObject.Fields.Count);
            Assert.AreEqual(0, queryObject.NumberToLimit);
            Assert.AreEqual(0, queryObject.NumberToSkip);
            Assert.AreEqual(new Document("Addresses.1.City", "Tokyo"), queryObject.Query);
        }

        [Test]
        public void NestedQueryable_Any()
        {
            var people = Collection.Linq().Where(x => x.Addresses.Any(a => a.City == "London"));

            var queryObject = ((IMongoQueryable)people).GetQueryObject();
            Assert.AreEqual(0, queryObject.Fields.Count);
            Assert.AreEqual(0, queryObject.NumberToLimit);
            Assert.AreEqual(0, queryObject.NumberToSkip);
            Assert.AreEqual(new Document("Addresses", new Document("$elemMatch", new Document("City", "London"))), queryObject.Query);
        }

        [Test]
        public void NestedQueryable_Contains()
        {
            var people = Collection.Linq().Where(x => x.EmployerIds.Contains(1));

            var queryObject = ((IMongoQueryable)people).GetQueryObject();
            Assert.AreEqual(0, queryObject.Fields.Count);
            Assert.AreEqual(0, queryObject.NumberToLimit);
            Assert.AreEqual(0, queryObject.NumberToSkip);
            Assert.AreEqual(new Document("EmployerIds", 1), queryObject.Query);
        }

        [Test]
        public void Nested_Queryable_Count()
        {
            var people = Collection.Linq().Where(x => x.Addresses.Count() == 1);

            var queryObject = ((IMongoQueryable)people).GetQueryObject();
            Assert.AreEqual(0, queryObject.Fields.Count);
            Assert.AreEqual(0, queryObject.NumberToLimit);
            Assert.AreEqual(0, queryObject.NumberToSkip);
            Assert.AreEqual(new Document("Addresses", Op.Size(1)), queryObject.Query);
        }

        [Test]
        public void Nested_Queryable_ElementAt()
        {
            var people = Collection.Linq().Where(x => x.Addresses.ElementAt(1).City == "Tokyo");

            var queryObject = ((IMongoQueryable)people).GetQueryObject();
            Assert.AreEqual(0, queryObject.Fields.Count);
            Assert.AreEqual(0, queryObject.NumberToLimit);
            Assert.AreEqual(0, queryObject.NumberToSkip);
            Assert.AreEqual(new Document("Addresses.1.City", "Tokyo"), queryObject.Query);
        }

        [Test]
        public void NotNullCheck()
        {
            var people = Collection.Linq().Where(x => x.MidName != null);

            var queryObject = ((IMongoQueryable)people).GetQueryObject();
            Assert.AreEqual(0, queryObject.Fields.Count);
            Assert.AreEqual(0, queryObject.NumberToLimit);
            Assert.AreEqual(0, queryObject.NumberToSkip);
            Assert.AreEqual(new Document("MidName", Op.NotEqual<string>(null)), queryObject.Query);
        }

        [Test]
        public void NullCheck()
        {
            var people = Collection.Linq().Where(x => x.MidName == null);

            var queryObject = ((IMongoQueryable)people).GetQueryObject();
            Assert.AreEqual(0, queryObject.Fields.Count);
            Assert.AreEqual(0, queryObject.NumberToLimit);
            Assert.AreEqual(0, queryObject.NumberToSkip);
            Assert.AreEqual(new Document("MidName", null), queryObject.Query);
        }

        [Test]
        public void OrderBy()
        {
            var people = Collection.Linq().OrderBy(x => x.Age).ThenByDescending(x => x.LastName);

            var queryObject = ((IMongoQueryable)people).GetQueryObject();
            Assert.AreEqual(0, queryObject.NumberToLimit);
            Assert.AreEqual(0, queryObject.NumberToSkip);
            Assert.AreEqual(new Document("Age", 1).Add("LastName", -1), queryObject.Sort);
        }

        [Test]
        public void Projection()
        {
            var people = from p in Collection.Linq()
                         select new {Name = p.FirstName + p.LastName};

            var queryObject = ((IMongoQueryable)people).GetQueryObject();
            Assert.AreEqual(2, queryObject.Fields.Count());
            Assert.AreEqual(0, queryObject.NumberToLimit);
            Assert.AreEqual(0, queryObject.NumberToSkip);
            Assert.AreEqual(0, queryObject.Query.Count);
        }

        [Test]
        public void ProjectionWithConstraints()
        {
            var people = from p in Collection.Linq()
                         where p.Age > 21 && p.Age < 42
                         select new {Name = p.FirstName + p.LastName};

            var queryObject = ((IMongoQueryable)people).GetQueryObject();
            Assert.AreEqual(2, queryObject.Fields.Count());
            Assert.AreEqual(0, queryObject.NumberToLimit);
            Assert.AreEqual(0, queryObject.NumberToSkip);
            Assert.AreEqual(new Document("Age", new Document().Merge(Op.GreaterThan(21)).Merge(Op.LessThan(42))), queryObject.Query);
        }

        [Test]
        public void ProjectionWithLocalCreation_ChildobjectShouldNotBeNull()
        {
            var people = Collection.Linq()
                .Select(p => new PersonWrapper(p, p.FirstName));

            var queryObject = ((IMongoQueryable)people).GetQueryObject();
            Assert.AreEqual(0, queryObject.Fields.Count());
            Assert.AreEqual(0, queryObject.NumberToLimit);
            Assert.AreEqual(0, queryObject.NumberToSkip);
            Assert.AreEqual(0, queryObject.Query.Count);

        }

        [Test]
        public void Regex_IsMatch()
        {
            var people = from p in Collection.Linq()
                         where Regex.IsMatch(p.FirstName, "Joe")
                         select p;

            var queryObject = ((IMongoQueryable)people).GetQueryObject();
            Assert.AreEqual(0, queryObject.Fields.Count);
            Assert.AreEqual(0, queryObject.NumberToLimit);
            Assert.AreEqual(0, queryObject.NumberToSkip);
            Assert.AreEqual(new Document("FirstName", new MongoRegex("Joe")), queryObject.Query);
        }

        [Test]
        public void Regex_IsMatch_CaseInsensitive()
        {
            var people = from p in Collection.Linq()
                         where Regex.IsMatch(p.FirstName, "Joe", RegexOptions.IgnoreCase)
                         select p;

            var queryObject = ((IMongoQueryable)people).GetQueryObject();
            Assert.AreEqual(0, queryObject.Fields.Count);
            Assert.AreEqual(0, queryObject.NumberToLimit);
            Assert.AreEqual(0, queryObject.NumberToSkip);
            Assert.AreEqual(new Document("FirstName", new MongoRegex("Joe", MongoRegexOption.IgnoreCase)), queryObject.Query);
        }

        [Test]
        public void SingleEqualConstraint()
        {
            var people = Collection.Linq().Where(p => "Jack" == p.FirstName);

            var queryObject = ((IMongoQueryable)people).GetQueryObject();

            Assert.AreEqual(0, queryObject.NumberToLimit);
            Assert.AreEqual(0, queryObject.NumberToSkip);
            Assert.AreEqual(new Document("FirstName", "Jack"), queryObject.Query);
        }

        [Test]
        public void SkipAndTake()
        {
            var people = Collection.Linq().Skip(2).Take(1);

            var queryObject = ((IMongoQueryable)people).GetQueryObject();
            Assert.AreEqual(1, queryObject.NumberToLimit);
            Assert.AreEqual(2, queryObject.NumberToSkip);
        }

        [Test]
        public void String_Contains()
        {
            var people = from p in Collection.Linq()
                         where p.FirstName.Contains("o")
                         select p;

            var queryObject = ((IMongoQueryable)people).GetQueryObject();
            Assert.AreEqual(0, queryObject.Fields.Count);
            Assert.AreEqual(0, queryObject.NumberToLimit);
            Assert.AreEqual(0, queryObject.NumberToSkip);
            Assert.AreEqual(new Document("FirstName", new MongoRegex("o")), queryObject.Query);
        }

        [Test]
        public void String_EndsWith()
        {
            var people = from p in Collection.Linq()
                         where p.FirstName.EndsWith("e")
                         select p;

            var queryObject = ((IMongoQueryable)people).GetQueryObject();
            Assert.AreEqual(0, queryObject.Fields.Count);
            Assert.AreEqual(0, queryObject.NumberToLimit);
            Assert.AreEqual(0, queryObject.NumberToSkip);
            Assert.AreEqual(new Document("FirstName", new MongoRegex("e$")), queryObject.Query);
        }

        [Test]
        public void String_StartsWith()
        {
            var people = from p in Collection.Linq()
                         where p.FirstName.StartsWith("J")
                         select p;

            var queryObject = ((IMongoQueryable)people).GetQueryObject();
            Assert.AreEqual(0, queryObject.Fields.Count);
            Assert.AreEqual(0, queryObject.NumberToLimit);
            Assert.AreEqual(0, queryObject.NumberToSkip);
            Assert.AreEqual(new Document("FirstName", new MongoRegex("^J")), queryObject.Query);
        }

        [Test]
        public void WithoutConstraints()
        {
            var people = Collection.Linq();

            var queryObject = ((IMongoQueryable)people).GetQueryObject();

            Assert.AreEqual(0, queryObject.NumberToLimit);
            Assert.AreEqual(0, queryObject.NumberToSkip);
            Assert.AreEqual(0, queryObject.Query.Count);
        }
    }
}