using System;
using System.Collections.Generic;

using NUnit.Framework;
using MongoDB.Driver.Bson;

namespace MongoDB.Driver
{
    [TestFixture]
    public class TestCollectionMetaData : MongoTestBase
    {
        MongoDatabase adminDb;

        public override string TestCollections {
            get {
                return "indextests,rename,renamed";
            }
        }
        
        public override void OnInit (){
            IMongoCollection its = DB["indextests"];
            its.Insert(createDoc("S","A","Anderson","OH"));
            its.Insert(createDoc("T","B","Delhi","OH"));
            its.Insert(createDoc("F","B","Cincinnati","OH"));
            its.Insert(createDoc("U","D","Newtown","OH"));
            its.Insert(createDoc("J","E","Newport","KY"));

            adminDb = DB.GetSisterDatabase("admin");
            //adminDb.MetaData.AddUser(adminuser, adminpass);
        }

        public override void OnDispose (){
            //adminDb.MetaData.RemoveUser(adminuser);
        }

        [Test]
        public void TestGetOptions(){
            CollectionMetadata cmd = DB["reads"].MetaData;
            Document options = cmd.Options;
            Assert.IsNotNull(options);
        }

        [Test]
        public void TestGetIndexes(){
            CollectionMetadata cmd = DB["indextests"].MetaData;
            Dictionary<string, Document> indexes = cmd.Indexes;

            Assert.IsNotNull(indexes);
            Assert.IsTrue(indexes.Count > 0, "Should have been at least one index found.");
            foreach(string key in indexes.Keys){
                System.Console.WriteLine(String.Format("Key: {0} Value: {1}", key, indexes[key]));
            }
        }

        [Test]
        public void TestCreateIndex(){
            CollectionMetadata cmd = DB["indextests"].MetaData;
            cmd.CreateIndex("lastnames", new Document().Add("lname", IndexOrder.Ascending), false);
            Dictionary<string, Document> indexes = cmd.Indexes;
            Assert.IsNotNull(indexes["lastnames"]);
        }

        [Test]
        public void TestCreateIndexNoNames(){
            CollectionMetadata cmd = DB["indextests"].MetaData;
            cmd.CreateIndex(new Document().Add("lname", IndexOrder.Ascending).Add("fname", IndexOrder.Ascending), true);
            Dictionary<string, Document> indexes = cmd.Indexes;
            Assert.IsNotNull(indexes["_lname_fname_unique_"]);
        }

        [Test]
        public void TestDropIndex(){
            CollectionMetadata cmd = DB["indextests"].MetaData;
            cmd.CreateIndex("firstnames", new Document().Add("fname", IndexOrder.Ascending), false);
            Dictionary<string, Document> indexes = cmd.Indexes;
            Assert.IsNotNull(indexes["firstnames"]);
            cmd.DropIndex("firstnames");
            Assert.IsFalse(cmd.Indexes.ContainsKey("firstnames"));
        }

        [Test]
        public void TestRename(){
            DB["rename"].Insert(new Document(){{"test", "rename"}});
            Assert.AreEqual(1, DB["rename"].Count());
            CollectionMetadata cmd = DB["rename"].MetaData;
            cmd.Rename("renamed");
            Assert.IsFalse(DB.GetCollectionNames().Contains(DB.Name + ".rename"), "Shouldn't have found collection");
            Assert.IsTrue(DB.GetCollectionNames().Contains(DB.Name + ".renamed"),"Should have found collection");
            Assert.AreEqual(1, DB["renamed"].Count());
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
