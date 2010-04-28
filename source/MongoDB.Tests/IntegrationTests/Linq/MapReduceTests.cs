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
        public void NoGrouping()
        {
            var ageRange = Enumerable.ToList(from p in collection.Linq()
                                             group p by 1 into g
                                             select new
                                             {
                                                 Min = g.Min(x => x.Age),
                                                 Max = g.Max(x => x.Age)
                                             });

            Assert.AreEqual(1, ageRange.Count);
            Assert.AreEqual(21, ageRange.Single().Min);
            Assert.AreEqual(42, ageRange.Single().Max);
        }

        [Test]
        public void SimpleGrouping()
        {
            var ageRange = Enumerable.ToList(from p in collection.Linq()
                                             group p by p.FirstName into g
                                             select new
                                             {
                                                 Min = g.Min(x => x.Age),
                                                 Max = g.Max(x => x.Age)
                                             });

            Assert.AreEqual(3, ageRange.Count);
        }
    }
}