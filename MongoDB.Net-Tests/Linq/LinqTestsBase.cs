using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using MongoDB.Driver.Attributes;

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
            [MongoName("fn")]
            public string FirstName { get; set; }

            [MongoName("ln")]
            public string LastName { get; set; }

            [MongoName("age")]
            public int Age { get; set; }

            [MongoName("add")]
            public Address PrimaryAddress { get; set; }

            [MongoName("otherAdds")]
            public List<Address> Addresses { get; set; }

            [MongoName("emps")]
            public int[] EmployerIds { get; set; }
        }

        protected class Address
        {
            [MongoName("city")]
            public string City { get; set; }
        }
    }
}
