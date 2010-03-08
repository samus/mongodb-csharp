
using System;
using NUnit.Framework;

namespace MongoDB.Driver
{
    [TestFixture()]        
    public class TestMapReduce : MongoTestBase
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
        public void TestGetMapReduceObject(){
            MapReduce mr = mrcol.MapReduce();
            Assert.IsNotNull(mr);
            Assert.AreEqual(mrcol.Name, mr.Name);
        }
        
        [Test()]
        public void TestExecuteSimple(){
            MapReduce mr = mrcol.MapReduce();
            mr.Map = new Code(mapfunction);
            mr.Reduce = new Code(reducefunction);
            mr.Execute();
            Assert.IsNotNull(mr.Result);
        }
        
    }

}
