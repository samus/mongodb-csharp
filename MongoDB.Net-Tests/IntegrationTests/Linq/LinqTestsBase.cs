using System.Collections.Generic;
using NUnit.Framework;
using MongoDB.Driver.Attributes;

namespace MongoDB.Driver.IntegrationTests.Linq
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
            [MongoAlias("fn")]
            public string FirstName { get; set; }

            [MongoAlias("ln")]
            public string LastName { get; set; }

            [MongoAlias("age")]
            public int Age { get; set; }

            [MongoAlias("add")]
            public Address PrimaryAddress { get; set; }

            [MongoAlias("otherAdds")]
            public List<Address> Addresses { get; set; }

            [MongoAlias("emps")]
            public int[] EmployerIds { get; set; }
        }

        protected class Address
        {
            [MongoAlias("city")]
            public string City { get; set; }
        }
    }
}
