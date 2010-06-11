using System;
using NUnit.Framework;

namespace MongoDB.IntegrationTests
{
    [TestFixture]
    public class TestMapReduce : MongoTestBase
    {
        private IMongoCollection _collection;

        private const string MapFunc = "function(){\n" +
                                       "   this.tags.forEach(\n" +
                                       "       function(z){\n" +
                                       "           emit( z , { count : 1 } );\n" +
                                       "       });\n" +
                                       "};";

        private const string ReduceFunc = "function( key , values ){\n" +
                                          "    var total = 0;\n" +
                                          "    for ( var i=0; i<values.length; i++ )\n" +
                                          "        total += values[i].count;\n" +
                                          "    return { count : total };\n" +
                                          "};";

        public override string TestCollections
        {
            get { return "mr"; }
        }

        public override void OnInit()
        {
            _collection = DB["mr"];
            _collection.Insert(new Document().Add("_id", 1).Add("tags", new[] {"dog", "cat"}));
            _collection.Insert(new Document().Add("_id", 2).Add("tags", new[] {"dog"}));
            _collection.Insert(new Document().Add("_id", 3).Add("tags", new[] {"mouse", "cat", "dog"}));
            _collection.Insert(new Document().Add("_id", 4).Add("tags", new String[] {}));
        }

        [Test]
        public void TestBuilderSetsAllProperties()
        {
            var query = new Document().Add("x", 1);
            var scope = new Document().Add("y", 2);
            var sort = new Document().Add("z", 3);
            var mrb = _collection.MapReduce();
            mrb.Map(MapFunc)
                .Reduce(ReduceFunc)
                .KeepTemp(true)
                .Limit(5)
                .Out("outtest")
                .Query(query)
                .Scope(scope)
                .Sort(sort)
                .Verbose(false);

            var mr = mrb.Command;
            Assert.AreEqual(query.ToString(), mr.Query.ToString());
            Assert.AreEqual(scope.ToString(), mr.Scope.ToString());
            Assert.AreEqual(sort.ToString(), mr.Sort.ToString());
            Assert.AreEqual(true, mr.KeepTemp);
            Assert.AreEqual(5, mr.Limit);
            Assert.AreEqual("outtest", mr.Out);
            Assert.AreEqual(false, mr.Verbose);
        }

        [Test]
        public void TestCreateMapReduceWithStringFunctions()
        {
            var mr = _collection.MapReduce();
            mr.Map(MapFunc).Reduce(ReduceFunc);

            Assert.IsNotNull(mr.Command.Map);
            Assert.IsNotNull(mr.Command.Reduce);
        }

        [Test]
        public void TestExecuteSimple()
        {
            var mrb = _collection.MapReduce();
            var mr = mrb.Map(MapFunc).Reduce(ReduceFunc);

            mr.RetrieveData();

            Assert.IsNotNull(mr.Result);
            Assert.IsTrue(mr.Result.Ok);
            Assert.AreEqual(4, mr.Result.InputCount);
            Assert.AreEqual(6, mr.Result.EmitCount);
            Assert.AreEqual(3, mr.Result.OutputCount);
        }

        [Test]
        public void TestExecuteSimple2()
        {
            var mr = _collection.MapReduce();
            mr.Command.Map = new Code(MapFunc);
            mr.Command.Reduce = new Code(ReduceFunc);

            mr.RetrieveData();

            Assert.IsNotNull(mr.Result);
        }

        [Test]
        public void TestExecuteUsing()
        {
            String tempcollname;
            using(var mrb = _collection.MapReduce().Map(MapFunc).Reduce(ReduceFunc))
            {
                mrb.RetrieveData();
                Assert.IsNotNull(mrb.Result);
                Assert.IsTrue(mrb.Result.Ok);
                tempcollname = DB.Name + "." + mrb.Result.CollectionName;
                Assert.IsTrue(DB.GetCollectionNames().Contains(tempcollname));
            }
            Assert.IsFalse(DB.GetCollectionNames().Contains(tempcollname));
        }

        [Test]
        public void TestGetMapReduceObject()
        {
            var mr = _collection.MapReduce();
            Assert.IsNotNull(mr);
            Assert.AreEqual(_collection.Name, mr.Command.Name);
        }
    }
}