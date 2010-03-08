
using System;
using System.Collections.Generic;

using NUnit.Framework;

namespace MongoDB.Driver
{
    [TestFixture()]
    public class TestMapReduceBuilder : MongoTestBase
    {
        IMongoCollection mrcol;
        string mapfunction = "function(){\n" +
                            "   this.tags.forEach(\n" +
                            "       function(z){\n" +
                            "           emit( z , { count : 1 } );\n" +
                            "       });\n" +
                            "};";
        string reducefunction = "function( key , values ){\n" +
                                "    var total = 0;\n" +
                                "    for ( var i=0; i<values.length; i++ )\n" +
                                "        total += values[i].count;\n" +
                                "    return { count : total };\n" +
                                "};";
        
        public override string TestCollections {
            get {
                return "mr";
            }
        }

        public override void OnInit (){
            mrcol = DB["mr"];
            mrcol.Insert(new Document().Append("_id", 1).Append("tags", new String[]{"dog", "cat"}));
            mrcol.Insert(new Document().Append("_id", 2).Append("tags", new String[]{"dog"}));
            mrcol.Insert(new Document().Append("_id", 3).Append("tags", new String[]{"mouse", "cat", "dog"}));
            mrcol.Insert(new Document().Append("_id", 4).Append("tags", new String[]{}));
        }
        
        [Test()]
        public void TestCreateMapReduceWithStringFunctions(){
            MapReduce mr = mrcol.MapReduce();
            MapReduceBuilder mrb = new MapReduceBuilder(mr);
            mrb.Map(mapfunction).Reduce(reducefunction);
            
            Assert.IsNotNull(mr.Map);
            Assert.IsNotNull(mr.Reduce);    
        }
        [Test()]
        public void TestBuilderSetsAllProperties(){
            Document query = new Document().Append("x",1);
            Document scope = new Document().Append("y",2);
            Document sort = new Document().Append("z",3);
            MapReduceBuilder mrb = mrcol.MapReduceBuilder();
            mrb.Map(mapfunction)
                    .Reduce(reducefunction)
                    .KeepTemp(true)
                    .Limit(5)
                    .Out("outtest")
                    .Query(query)
                    .Scope(scope)
                    .Sort(sort)
                    .Verbose(false);
            
            MapReduce mr = mrb.MapReduce;
            Assert.AreEqual(query.ToString(), mr.Query.ToString());
            Assert.AreEqual(scope.ToString(), mr.Scope.ToString());
            Assert.AreEqual(sort.ToString(), mr.Sort.ToString());
            Assert.AreEqual(true, mr.KeepTemp);
            Assert.AreEqual(5, mr.Limit);
            Assert.AreEqual("outtest", mr.Out);
            Assert.AreEqual(false, mr.Verbose);
        }
        
        [Test()]
        public void TestExecuteSimple(){
            MapReduceBuilder mrb = mrcol.MapReduceBuilder();
            MapReduce mr = mrb.Map(mapfunction).Reduce(reducefunction).Execute();
            Assert.IsNotNull(mr.Result);
            Assert.IsTrue(mr.Result.Ok);
            Assert.AreEqual(4, mr.Result.InputCount);
            Assert.AreEqual(6, mr.Result.EmitCount);
            Assert.AreEqual(3, mr.Result.OutputCount);
        }
        
        [Test()]
        public void TestExecuteUsing(){
            String tempcollname = null;
            using(MapReduceBuilder mrb = mrcol.MapReduceBuilder().Map(mapfunction).Reduce(reducefunction)){
                MapReduce mr = mrb.Execute();
                Assert.IsNotNull(mr.Result);
                Assert.IsTrue(mr.Result.Ok);
                tempcollname = DB.Name + "." + mr.Result.CollectionName;
                Assert.IsTrue(DB.GetCollectionNames().Contains(tempcollname));
            }
            Assert.IsFalse(DB.GetCollectionNames().Contains(tempcollname));
        }        
    }
}
