using NUnit.Framework;

namespace MongoDB.IntegrationTests.Linq
{
    public class LinqTestsBase : MongoTestBase
    {
        public override string TestCollections
        {
            get { return "people"; }
        }

        protected IMongoCollection<Person> Collection;
        protected IMongoCollection DocumentCollection;

        [SetUp]
        public virtual void TestSetup()
        {
            Collection = TestsDatabase.GetCollection<Person>("people");
            DocumentCollection = TestsDatabase.GetCollection("people");
        }
    }
}
