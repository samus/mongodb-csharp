using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using MongoDB.Linq;

using NUnit.Framework;

namespace MongoDB.IntegrationTests.Linq
{
    [TestFixture]
    public class MapReduceTests : LinqTestsBase
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
                    EmployerIds = new[] { 1, 2 }
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
                    EmployerIds = new[] { 3 }
                }, true);
        }

        [Test]
        public void Off_of_select()
        {
            var minAge = collection.Linq().Select(x => x.Age).Min();

            Assert.AreEqual(21, minAge);
        }

        [Test]
        public void Off_of_root()
        {
            var minAge = collection.Linq().Min(x => x.Age);

            Assert.AreEqual(21, minAge);
        }

        [Test]
        public void NoGrouping()
        {
            var ageRange = Enumerable.ToList(from p in collection.Linq()
                                             where p.Age > 21
                                             group p by 1 into g
                                             select new
                                             {
                                                 Min = g.Min(x => x.Age),
                                                 Max = g.Max(x => x.Age),
                                                 Count = g.Count(),
                                                 Sum = g.Sum(x => x.Age)
                                             });

            Assert.AreEqual(1, ageRange.Count);
            Assert.AreEqual(35, ageRange.Single().Min);
            Assert.AreEqual(42, ageRange.Single().Max);
            Assert.AreEqual(2, ageRange.Single().Count);
            Assert.AreEqual(77, ageRange.Single().Sum);
        }

        [Test]
        public void Expression_Grouping()
        {
            var ageRange = Enumerable.ToList(from p in collection.Linq()
                                             group p by p.Age % 2 into g
                                             select new
                                             {
                                                 IsEven = g.Key == 0,
                                                 Min = g.Min(x => x.Age),
                                                 Max = g.Max(x => x.Age),
                                                 Count = g.Count(),
                                                 Sum = g.Sum(x => x.Age)
                                             });

            Assert.AreEqual(2, ageRange.Count);
            Assert.AreEqual(1, ageRange[0].Count);
            Assert.AreEqual(42, ageRange[0].Max);
            Assert.AreEqual(42, ageRange[0].Min);
            Assert.AreEqual(42, ageRange[0].Sum);
            Assert.AreEqual(2, ageRange[1].Count);
            Assert.AreEqual(35, ageRange[1].Max);
            Assert.AreEqual(21, ageRange[1].Min);
            Assert.AreEqual(56, ageRange[1].Sum);
        }

        [Test]
        public void Complex()
        {
            var ageRange = Enumerable.ToList(from p in collection.Linq()
                                             where p.Age > 21
                                             group p by new { FirstName = p.FirstName, LastName = p.LastName } into g
                                             select new
                                             {
                                                 Name = g.Key.FirstName + " " + g.Key.LastName,
                                                 Min = g.Min(x => x.Age) + 100,
                                                 Max = g.Max(x => x.Age) + 100
                                             });

            Assert.AreEqual(2, ageRange.Count);
            Assert.AreEqual("Bob McBob", ageRange[0].Name);
            Assert.AreEqual(142, ageRange[0].Max);
            Assert.AreEqual(142, ageRange[0].Min);
            Assert.AreEqual("Jane McJane", ageRange[1].Name);
            Assert.AreEqual(135, ageRange[1].Max);
            Assert.AreEqual(135, ageRange[1].Min);
        }
    }
}