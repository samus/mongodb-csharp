using System;

using NUnit.Framework;

using MongoDB.Driver;
using MongoDB.Driver.GridFS;

namespace MongoDB.Driver.GridFS
{
    [TestFixture]
    public class GridFSTest{
        Mongo db = new Mongo();
        
        [Test]
        public void TestOpenNewGridFile()
        {
            GridFS gridFS = new GridFS(db["tests"]);
            using (GridFile gf = new GridFile(gridFS))
            {                
                gf.Open("newfile.txt");
                Console.WriteLine(gf.Id.ToString());
            }
        }
        
        [Test]
        public void TestFileDoesNotExist(){
            GridFS fs = new GridFS(db["tests"]);
            Assert.IsFalse(fs.Exists("non-existent filename"));
        }
        
        
        [TestFixtureSetUp]
        public void Init(){
            db.Connect();
            CleanDB(); //Run here instead of at the end so that the db can be examined after a run.
        }
        
        [TestFixtureTearDown]
        public void Dispose(){
            db.Disconnect();
        }
        
        protected void CleanDB(){
            //Any collections that we might want to delete before the tests run should be done here.
        }
    }
}
