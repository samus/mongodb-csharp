using System;
using System.Collections.Generic;
using System.IO;

using NUnit.Framework;

using MongoDB.Driver;

namespace MongoDB.GridFS
{
    [TestFixture]
    public class GridFileInfoTest : GridTestBase
    {

        public override string TestFileSystems {
            get {
                return "gfcreate,gfdelete,gfmove,gfopen,gfexists,gfinfo";
            }
        }
        
        
        [Test]
        public void TestCreateNonExisting(){
            String filename = "newfile.txt";
            GridFile gf = new GridFile(DB,"gfcreate");
            GridFileInfo gfi = new GridFileInfo(DB,"gfcreate", filename);
            
            Assert.AreEqual(filename, gfi.FileName);
            GridFileStream gfs = gfi.Create();
            Assert.AreEqual(filename, gfi.FileName, "Filename got erased?");
            Assert.IsTrue(gf.Exists(gfi.FileName));
        }
        
        [Test]
        public void TestCreateExisting(){
            String filename = "existing.txt";
            GridFile gf = new GridFile(DB,"gfcreate");
            GridFileInfo gfi = new GridFileInfo(DB,"gfcreate", filename);
            GridFileStream gfs = gfi.Create();
            gfs.Close();
            
            bool thrown = false;
            try{
                gfi = new GridFileInfo(DB,"gfcreate", filename);
                gfi.Create();
            }catch(IOException){
                thrown = true;
            }
            Assert.IsTrue(thrown, "Shouldn't be able to create the same file twice.");
        }
        
        [Test]
        public void TestModeCreateNew(){
            Object id;
            string filename = "createnew.txt";
            GridFileInfo gfi = new GridFileInfo(DB,"gfcreate", filename);
            using(GridFileStream gfs = gfi.Create(FileMode.CreateNew)){
                id = gfs.GridFileInfo.Id;
                TextWriter tw = new StreamWriter(gfs);
                tw.WriteLine("test");
                tw.Close();
            }
            Assert.AreEqual(1, CountChunks("gfcreate", id));
        }
        
        [Test]
        public void TestDelete(){
            String filename = "gfi-delete.txt";
            GridFile gf = new GridFile(DB,"gfdelete");
            GridFileInfo gfi = new GridFileInfo(DB,"gfdelete", filename);
            GridFileStream gfs = gfi.Create();  //TODO Expand Test to make sure that chunks for the file got deleted too.
            gfi.Delete();
            Assert.IsFalse(gf.Exists(filename), "File should have been deleted.");
        }
        
        [Test]
        public void TestMoveTo(){
            String filename = "gfi-move.txt";
            String filename2 = "gfi-move.txt2";
            GridFile gf = new GridFile(DB,"gfmove");
            GridFileInfo gfi = new GridFileInfo(DB,"gfmove", filename);
            gfi.Create();
            gfi.MoveTo(filename2);
            Assert.IsFalse(gf.Exists(filename), "File should have been moved.");
            Assert.IsTrue(gf.Exists(filename2), "File wasn't");
            Assert.AreEqual(filename2, gfi.FileName, "Filename wasn't set in GridFileInfo");
        }
        
        [Test]
        public void TestFileExists(){
            string filename = "gfi-exists.txt";
            GridFileInfo gfi = new GridFileInfo(DB, "gfexists", filename);
            Assert.IsFalse(gfi.Exists);
            GridFileStream gfs = gfi.Create();
            Assert.IsTrue(gfi.Exists);
        }
        
        [Test]
        public void TestOpenNonExistentFails(){
            string filename = "gfi-opennothere.txt";
            GridFile gf = new GridFile(DB, "gfopen");
            GridFileInfo gfi = new GridFileInfo(DB, "gfopen", filename);
            bool thrown = false;
            try{
                GridFileStream gfs = gfi.OpenRead();
            }catch(DirectoryNotFoundException dnfe){
                Assert.AreEqual(gf.Name + Path.VolumeSeparatorChar + filename, dnfe.Message);
                thrown = true;
            }
            Assert.IsTrue(thrown);
        }

        [Test]
        public void TestOpenReadOnly(){
            string filename = "gfi-open.txt";
            GridFile gf = new GridFile(DB, "gfopen");
            GridFileStream gfs = gf.Create(filename);
            gfs.Close();

            gfs = gf.OpenRead(filename);
            Assert.IsNotNull(gfs);
            bool thrown = false;
            try{
                gfs.Write(new byte[]{0},0,1);
            }catch(System.NotSupportedException){
                thrown = true;
            }catch(Exception ex){
                Assert.Fail("Wrong exception thrown " + ex.GetType().Name);
            }
            Assert.IsTrue(thrown, "NotSupportedException not thrown");
        }
        
        [Test]
        public void TestUpdateInfo(){
            string filename = "gfi-meta.txt";
            string fs = "gfinfo";
            
            Object id;
            GridFileInfo gfi = new GridFileInfo(DB,fs, filename);
            using(GridFileStream gfs = gfi.Create(FileMode.CreateNew)){
                id = gfs.GridFileInfo.Id;
                gfi.ContentType = "text/sam";
                Assert.AreEqual(gfi.ContentType, gfs.GridFileInfo.ContentType, "gridfileinfos don't point to the same object");
                TextWriter tw = new StreamWriter(gfs);
                tw.WriteLine("test");
                tw.Close();
            }
            gfi.Aliases = new List<String>(){"file1"};
            GridFileInfo gfi2 = new GridFileInfo(DB,fs, filename);
            Assert.IsTrue(gfi2.Exists, "Couldn't find " + filename);
            Assert.AreEqual("text/sam", gfi2.ContentType);
            Assert.AreNotEqual(gfi2.Aliases, gfi.Aliases, "Aliases shouldn't have been updated in the DB yet.");
            gfi.UpdateInfo();
            gfi2.Refresh();
            Assert.AreEqual(gfi2.Aliases, gfi.Aliases);
        }        
    }
}
