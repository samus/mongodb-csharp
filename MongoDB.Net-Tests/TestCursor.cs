/*
 * User: scorder
 */

using System;
using NUnit.Framework;

using MongoDB.Driver;
using MongoDB.Driver.IO;
using MongoDB.Driver.Bson;

namespace MongoDB.Driver
{
    [TestFixture]
    public class TestCursor
    {
        Mongo db = new Mongo();
        
        [TestFixtureSetUp]
        public void Setup(){
            db.Connect();
        }
        
        [Test]
        public void TestCanReadSmall()
        {
            Cursor c = db["tests"]["smallreads"].FindAll();
            
            Assert.IsNotNull(c,"Cursor shouldn't be null");
            int reads = 0;
            foreach(Document doc in c.Documents){
                reads++;
            }
            Assert.IsTrue(reads > 0, "No documents were returned.");
            Assert.AreEqual(4, reads, "More than 4 documents in the small reads dataset");
        }
        [Test]
        public void TestCanReadMore(){
            Cursor c = db["tests"]["reads"].FindAll();
            
            Assert.IsNotNull(c,"Cursor shouldn't be null");
            int reads = 0;
            int idchanges = 0;
            long id = 0;
            foreach(Document doc in c.Documents){
                reads++;
                if(c.Id != id){
                    idchanges++;
                    id = c.Id;
                }
            }
            Assert.IsTrue(reads > 0, "No documents were returned.");
            Assert.IsTrue(idchanges > 0,String.Format("ReadMore message never sent. {0} changes seen", idchanges));
            Assert.AreEqual(9999,reads, "Not all documents returned.");
            System.Console.Out.Write(String.Format("{0} records read", reads));
            

        }
        [Test]
        public void TestCanReadAndKillCursor()
        {
            Cursor c = db["tests"]["reads"].FindAll();
            
            Assert.IsNotNull(c,"Cursor shouldn't be null");
            foreach(Document doc in c.Documents){
                break;
            }
            c.Dispose();
            Assert.AreEqual(0,c.Id);
        }
        
        [Test]
        public void TestCanLimit()
        {
            Cursor c = db["tests"]["reads"].FindAll();
            c.Limit = 5;
            
            Assert.IsNotNull(c,"Cursor shouldn't be null");
            int reads = 0;
            int idchanges = 0;
            long id = 0;
            foreach(Document doc in c.Documents){
                reads++;
            }
            Assert.IsTrue(reads > 0, "No documents were returned.");
            Assert.AreEqual(5, reads);

        }
//      [Test]
//      public void TestMethod()
//      {
//          Assert.Fail("Write Test");
//      }
    }
}
