using System;
using System.IO;

using NUnit.Framework;

namespace MongoDB.Driver.GridFS
{
    [TestFixture]
    public class GridFileInfoTest
    {
        Mongo db = new Mongo();
        [Test]
        public void TestCreateNonExisting(){
            String filename = "newfile.txt";
            GridFile gf = new GridFile(db["tests"],"gfcreate");
            GridFileInfo gfi = new GridFileInfo(db["tests"],"gfcreate", filename);
            
            Assert.AreEqual(filename, gfi.FileName);
            GridFileStream gfs = gfi.Create();
            Assert.AreEqual(filename, gfi.FileName, "Filename got erased?");
            Assert.IsTrue(gf.Exists(gfi.FileName));
        }
        
        [Test]
        public void TestCreateExisting(){
            String filename = "existing.txt";
            GridFile gf = new GridFile(db["tests"],"gfcreate");
            GridFileInfo gfi = new GridFileInfo(db["tests"],"gfcreate", filename);
            GridFileStream gfs = gfi.Create();
            bool thrown = false;
            try{
                gfi = new GridFileInfo(db["tests"],"create", filename);
                gfi.Create();
            }catch(IOException){
                thrown = true;
            }
            Assert.IsTrue(thrown, "Shouldn't be able to create the same file twice.");
        }
        
        [Test]
        public void TestDelete(){
            String filename = "gfi-delete.txt";
            GridFile gf = new GridFile(db["tests"],"gfdelete");
            GridFileInfo gfi = new GridFileInfo(db["tests"],"gfdelete", filename);
            GridFileStream gfs = gfi.Create();  //TODO Expand Test to make sure that chunks for the file got deleted too.
            gfi.Delete();
            Assert.IsFalse(gf.Exists(filename), "File should have been deleted.");
        }
        
        [Test]
        public void TestMoveTo(){
            String filename = "gfi-move.txt";
            String filename2 = "gfi-move.txt2";
            GridFile gf = new GridFile(db["tests"],"gfmove");
            GridFileInfo gfi = new GridFileInfo(db["tests"],"gfmove", filename);
            gfi.Create();
            gfi.MoveTo(filename2);
            Assert.IsFalse(gf.Exists(filename), "File should have been moved.");
            Assert.IsTrue(gf.Exists(filename2), "File wasn't");
        }

        [Test]
        public void TestOpenNonExistentFails(){
            string filename = "gfi-opennothere.txt";
            GridFile gf = new GridFile(db["tests"], "gfopen");
            GridFileInfo gfi = new GridFileInfo(db["tests"], "gfopen", filename);
            bool thrown = false;
            try{
                GridFileStream gfs = gfi.OpenRead();
            }catch(DirectoryNotFoundException dnfe){
                Assert.AreEqual(filename, dnfe.Message);
                thrown = true;
            }
            Assert.IsTrue(thrown);
        }

        [Test]
        public void TestOpen(){
            string filename = "gfi-open.txt";
            GridFile gf = new GridFile(db["tests"], "gfopen");
            GridFileStream gfs = gf.Create(filename);
            gfs.Close();

            gfs = gf.OpenRead(filename);
            Assert.IsNotNull(gfs);

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
            DropGridFileSystem("gfcreate");
            DropGridFileSystem("gfdelete");
            DropGridFileSystem("gfmove");
            DropGridFileSystem("gfopen");
        }
        
        protected void DropGridFileSystem(string filesystem){
            try{
                db["tests"].MetaData.DropCollection(filesystem + ".files");
                db["tests"].MetaData.DropCollection(filesystem + ".chunks");
            }catch(MongoCommandException){}//if it fails it is because the collection isn't there to start with.
            
        }
    }
}
