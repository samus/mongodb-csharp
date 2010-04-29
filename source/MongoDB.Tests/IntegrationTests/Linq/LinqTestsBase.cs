using System.Collections.Generic;
using MongoDB.Attributes;
using NUnit.Framework;

namespace MongoDB.IntegrationTests.Linq
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
    }
}
