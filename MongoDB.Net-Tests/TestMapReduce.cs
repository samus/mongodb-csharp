
using System;
using NUnit.Framework;

namespace MongoDB.Driver
{
    [TestFixture()]        
    public class TestMapReduce
    {
        Mongo db = new Mongo();
        Database tests;
        Collection mrcol;
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

    
        [TestFixtureSetUp]
        public void Init(){
            db.Connect();
            tests = db["tests"];
            mrcol = (Collection)tests["mr"];
            
            CleanDB();
            SetupCollection();
        }
        
        [TestFixtureTearDown]
        public void Dispose(){
            //cleanDB();
            db.Disconnect();
        }
        
        protected void CleanDB(){
            db["tests"]["$cmd"].FindOne(new Document().Append("drop","mr"));
        }
        
        protected void SetupCollection(){
            mrcol.Insert(new Document().Append("_id", 1).Append("tags", new String[]{"dog", "cat"}));
            mrcol.Insert(new Document().Append("_id", 2).Append("tags", new String[]{"dog"}));
            mrcol.Insert(new Document().Append("_id", 3).Append("tags", new String[]{"mouse", "cat", "dog"}));
            mrcol.Insert(new Document().Append("_id", 4).Append("tags", new String[]{}));
        }
    }

}
