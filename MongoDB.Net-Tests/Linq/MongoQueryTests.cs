using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MongoDB.Driver.Linq;

using NUnit.Framework;
using System.Text.RegularExpressions;

namespace MongoDB.Driver.Tests.Linq
{
    [TestFixture]
    public class MongoQueryTests : LinqTestsBase
    {
        public override void TestSetup()
        {
            base.TestSetup();

            collection.Delete(new { }, true);
            collection.Insert(
                new Person
                {
                    FirstName = "Bob",
                    LastName = "McBob",
                    Age = 42,
                    PrimaryAddress = new Address { City = "London" },
                    Addresses = new List<Address> 
                    {
                        new Address { City = "London" },
                        new Address { City = "Tokyo" }, 
                        new Address { City = "Seattle" } 
                    },
                    EmployerIds = new [] { 1, 2}
                }, true);

            collection.Insert(
                new Person
                {
                    FirstName = "Jane",
                    LastName = "McJane",
                    Age = 35,
                    PrimaryAddress = new Address { City = "Paris" },
                    Addresses = new List<Address> 
                    {
                        new Address { City = "Paris" }
                    },
                    EmployerIds = new[] { 1 }

                }, true);

            collection.Insert(
                new Person
                {
                    FirstName = "Joe",
                    LastName = "McJoe",
                    Age = 21,
                    PrimaryAddress = new Address { City = "Chicago" },
                    Addresses = new List<Address> 
                    {
                        new Address { City = "Chicago" },
                        new Address { City = "London" }
                    },
                    EmployerIds = new [] { 3 }
                }, true);
        }

        [Test]
        public void WithoutConstraints()
        {
            var people = collection.Linq().ToList();

            Assert.AreEqual(3, people.Count);
        }

        [Test]
        public void SingleEqualConstraint()
        {
            var people = collection.Linq().Where(p => "Joe" == p.FirstName).ToList();

            Assert.AreEqual(1, people.Count);
        }

        [Test]
        public void ConjuctionConstraint()
        {
            var people = collection.Linq().Where(p => p.Age > 21 && p.Age < 42).ToList();

            Assert.AreEqual(1, people.Count);
        }

        [Test]
        public void NestedClassConstraint()
        {
            var people = collection.Linq().Where(p => p.PrimaryAddress.City == "London").ToList();

            Assert.AreEqual(1, people.Count);
        }

        [Test]
        public void Projection()
        {
            var people = (from p in collection.Linq()
                          select new { Name = p.FirstName + p.LastName }).ToList();

            Assert.AreEqual(3, people.Count);
        }

        [Test]
        public void ProjectionWithConstraints()
        {
            var people = (from p in collection.Linq()
                          where p.Age > 21 && p.Age < 42
                          select new { Name = p.FirstName + p.LastName }).ToList();

            Assert.AreEqual(1, people.Count);
        }

        [Test]
        public void ConstraintsAgainstLocalVariable()
        {
            int age = 21;
            var people = collection.Linq().Where(p => p.Age > age).ToList();

            Assert.AreEqual(2, people.Count);
        }

        [Test]
        public void ConstraintsAgainstLocalReferenceMember()
        {
            var local = new { Test = new { Age = 21 } };
            var people = collection.Linq().Where(p => p.Age > local.Test.Age).ToList();

            Assert.AreEqual(2, people.Count);
        }

        [Test]
        public void OrderBy()
        {
            var people = collection.Linq().OrderBy(x => x.Age).ThenByDescending(x => x.LastName).ToList();

            Assert.AreEqual("Joe", people[0].FirstName);
            Assert.AreEqual("Jane", people[1].FirstName);
            Assert.AreEqual("Bob", people[2].FirstName);
        }

        [Test]
        public void SkipAndTake()
        {
            var people = collection.Linq().OrderBy(x => x.Age).Skip(2).Take(1).ToList();

            Assert.AreEqual("Bob", people[0].FirstName);
        }

        [Test]
        public void First()
        {
            var person = collection.Linq().OrderBy(x => x.Age).First();

            Assert.AreEqual("Joe", person.FirstName);
        }

        [Test]
        public void Single()
        {
            var person = collection.Linq().Where(x => x.Age == 21).Single();

            Assert.AreEqual("Joe", person.FirstName);
        }

        [Test]
        public void Count()
        {
            var count = collection.Linq().Count();

            Assert.AreEqual(3, count);
        }

        [Test]
        public void Count_without_predicate()
        {
            var count = collection.Linq().Where(x => x.Age > 21).Count();

            Assert.AreEqual(2, count);
        }

        [Test]
        public void Count_with_predicate()
        {
            var count = collection.Linq().Count(x => x.Age > 21);

            Assert.AreEqual(2, count);
        }

        [Test]
        public void DocumentQuery()
        {
            var people = (from p in documentCollection.Linq()
                          where p.Key("Age") > 21
                          select (string)p["FirstName"]).ToList();

            Assert.AreEqual(2, people.Count);
        }

        [Test]
        public void LocalList_Contains()
        {
            var names = new List<string>() { "Joe", "Bob" };
            var people = collection.Linq().Where(x => names.Contains(x.FirstName)).ToList();

            Assert.AreEqual(2, people.Count);
        }

        [Test]
        public void LocalEnumerable_Contains()
        {
            var names = new[] { "Joe", "Bob" };
            var people = collection.Linq().Where(x => names.Contains(x.FirstName)).ToList();

            Assert.AreEqual(2, people.Count);
        }

        [Test]
        public void String_StartsWith()
        {
            var people = (from p in collection.Linq()
                          where p.FirstName.StartsWith("J")
                          select p).ToList();

            Assert.AreEqual(2, people.Count);
        }

        [Test]
        public void String_EndsWith()
        {
            var people = (from p in collection.Linq()
                          where p.FirstName.EndsWith("e")
                          select p).ToList();

            Assert.AreEqual(2, people.Count);
        }

        [Test]
        public void String_Contains()
        {
            var people = (from p in collection.Linq()
                          where p.FirstName.Contains("o")
                          select p).ToList();

            Assert.AreEqual(2, people.Count);
        }

        [Test]
        public void Regex_IsMatch()
        {
            var people = (from p in collection.Linq()
                          where Regex.IsMatch(p.FirstName, "Joe")
                          select p).ToList();

            Assert.AreEqual(1, people.Count);
        }

        [Test]
        public void NestedArray_Length()
        {
            var people = (from p in collection.Linq()
                          where p.EmployerIds.Length == 1
                          select p).ToList();

            Assert.AreEqual(2, people.Count);
        }

        [Test]
        public void NestedCollection_Count()
        {
            var people = (from p in collection.Linq()
                          where p.Addresses.Count == 1
                          select p).ToList();

            Assert.AreEqual(1, people.Count);
        }

        [Test]
        public void Nested_Queryable_Count()
        {
            var people = collection.Linq().Where(x => x.Addresses.Count() == 1).ToList();

            Assert.AreEqual(1, people.Count);
        }

        [Test(Description = "This will fail < 1.4")]
        public void Nested_Queryable_ElementAt()
        {
            var people = collection.Linq().Where(x => x.Addresses.ElementAt(1).City == "Tokyo").ToList();

            Assert.AreEqual(1, people.Count);
        }

        [Test(Description = "This will fail < 1.4")]
        public void NestedArray_indexer()
        {
            var people = collection.Linq().Where(x => x.EmployerIds[0] == 1).ToList();

            Assert.AreEqual(2, people.Count);
        }

        [Test(Description = "This will fail < 1.4")]
        public void NestedList_indexer()
        {
            var people = collection.Linq().Where(x => x.Addresses[1].City == "Tokyo").ToList();

            Assert.AreEqual(1, people.Count);
        }

        [Test]
        public void NestedQueryable_Contains()
        {
            var people = collection.Linq().Where(x => x.EmployerIds.Contains(1)).ToList();

            Assert.AreEqual(2, people.Count);
        }

        [Test]
        public void NestedQueryable_Any()
        {
            var people = collection.Linq().Where(x => x.Addresses.Any(a => a.City == "London")).ToList();

            Assert.AreEqual(2, people.Count);
        }
    }
}