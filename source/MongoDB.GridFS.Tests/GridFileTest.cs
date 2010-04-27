using System;
using System.IO;

using NUnit.Framework;

using MongoDB;

namespace MongoDB.GridFS
{
    [TestFixture]
    public class GridFileTest : GridTestBase
    {
        public override string TestFileSystems {
            get {
                return "gfcopy,gfcreate,fs";
            }
        }

        [Test]
        public void TestFileDoesNotExist(){
            GridFile fs = new GridFile(DB);
            Assert.IsFalse(fs.Exists("non-existent filename"));
        }

        [Test]
        public void TestFileDoes(){
            GridFile fs = new GridFile(DB);
            fs.Create("exists.txt");
            Assert.IsTrue(fs.Exists("exists.txt"));
        }
        [Test]
        public void TestCopy(){
            GridFile fs = new GridFile(DB, "gfcopy");
            GridFileStream gfs = fs.Create("original.txt");
            gfs.WriteByte(1);
            gfs.Seek(1024 * 256 * 2, SeekOrigin.Begin);
            gfs.WriteByte(2);
            gfs.Close();
            fs.Copy("original.txt", "copy.txt");
            Assert.IsTrue(fs.Exists("original.txt"));
            Assert.IsTrue(fs.Exists("copy.txt"));
            //TODO Assert chunk data is the same too.
        }
        
        [Test]
        public void TestModeCreateNew(){
            Object id;
            string filename = "createnew.txt";
            GridFile gf = new GridFile(DB,"gfcreate");
            using(GridFileStream gfs = gf.Create(filename, FileMode.CreateNew)){
                id = gfs.GridFileInfo.Id;
                TextWriter tw = new StreamWriter(gfs);
                tw.WriteLine("test");
                tw.Close();
            }
            Assert.AreEqual(1, CountChunks("gfcreate", id));
        }        
    }
}
