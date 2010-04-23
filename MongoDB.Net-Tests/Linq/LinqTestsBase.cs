using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace MongoDB.Driver.Tests.Linq
{
    public class LinqTestsBase : MongoTestBase
    {
        public override string TestCollections
        {
            get { return "people"; }
        }

        protected IMongoCollection<Person> collection;
        protected IMongoCollection documentCollection;

        [SetUp]
        public virtual void TestSetup()
        {
            collection = DB.GetCollection<Person>("people");
            documentCollection = DB.GetCollection("people");
        }

        protected class Person
        {
            public string FirstName { get; set; }

            public string LastName { get; set; }

            public int Age { get; set; }

            public Address PrimaryAddress { get; set; }

            public List<Address> Addresses { get; set; }

            public int[] EmployerIds { get; set; }

        }

        protected class Address
        {
            public string City { get; set; }
        }
    }
}
