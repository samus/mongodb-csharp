using System;
using System.Collections.Generic;

using NUnit.Framework;
using MongoDB.Driver.Bson;

namespace MongoDB.Driver
{
    [TestFixture]
    public class TestCollectionMetaData : MongoTestBase
    {
        Mongo db = new Mongo();
        public override string TestCollections {
            get {
                return "indextests";
            }
        }

        [Test]
        public void TestGetOptions(){
            CollectionMetaData cmd = DB["reads"].MetaData;
            Document options = cmd.Options;
            Assert.IsNotNull(options);            
        }
        
        [Test]
        public void TestGetIndexes(){
            CollectionMetaData cmd = DB["indextests"].MetaData;
            Dictionary<string, Document> indexes = cmd.Indexes;
            
            Assert.IsNotNull(indexes);      
            Assert.IsTrue(indexes.Count > 0, "Should have been at least one index found.");
            foreach(string key in indexes.Keys){
                System.Console.WriteLine(String.Format("Key: {0} Value: {1}", key, indexes[key]));
            }
        }
        
        [Test]
        public void TestCreateIndex(){
            CollectionMetaData cmd = DB["indextests"].MetaData;
            cmd.CreateIndex("lastnames", new Document().Append("lname", IndexOrder.Ascending), false);
            Dictionary<string, Document> indexes = cmd.Indexes;
            Assert.IsNotNull(indexes["lastnames"]);
        }

        [Test]
        public void TestCreateIndexNoNames(){
            CollectionMetaData cmd = DB["indextests"].MetaData;
            cmd.CreateIndex(new Document().Append("lname", IndexOrder.Ascending).Append("fname",IndexOrder.Ascending), true);
            Dictionary<string, Document> indexes = cmd.Indexes;
            Assert.IsNotNull(indexes["_lname_fname_unique_"]);
        }
        
        [Test]
        public void TestDropIndex(){
            CollectionMetaData cmd = DB["indextests"].MetaData;
            cmd.CreateIndex("firstnames", new Document().Append("fname", IndexOrder.Ascending), false);
            Dictionary<string, Document> indexes = cmd.Indexes;
            Assert.IsNotNull(indexes["firstnames"]);
            cmd.DropIndex("firstnames");
            Assert.IsFalse(cmd.Indexes.ContainsKey("firstnames"));
        }

        public override void OnInit (){
            IMongoCollection its = DB["indextests"];
            Document doc = new Document();
            its.Insert(createDoc("S","A","Anderson","OH"));
            its.Insert(createDoc("T","B","Delhi","OH"));
            its.Insert(createDoc("F","B","Cincinnati","OH"));
            its.Insert(createDoc("U","D","Newtown","OH"));
            its.Insert(createDoc("J","E","Newport","KY"));            
        }

        
        protected Document createDoc(string fname, string lname, string city, string state){
            Document doc = new Document();
            doc["fname"] = fname;
            doc["lname"] = lname;
            doc["city"] = city;
            doc["state"] = state;
            return doc;
        }
        
    }   
}
