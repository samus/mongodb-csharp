using System;
using System.IO;

using NUnit.Framework;

using MongoDB.Driver;

namespace MongoDB.Driver.GridFS
{
    [TestFixture]
    public class GridFileStreamTest
    {
        Mongo db = new Mongo();
        [Test]
        public void TestWrite(){
            GridFile fs = new GridFile(db["tests"], "gfstream");
            GridFileStream gfs = fs.Create("test.txt");
            Object id = gfs.GridFileInfo.Id;

            for(byte b = (byte)0; b < 128; b++){
                gfs.WriteByte(b);    
            }
            gfs.Close();
            
            Assert.AreEqual(1, db["tests"]["gfstream.chunks"].Count(new Document().Append("files_id", id)));
        }
        
        [Test]
        public void TestWriteMultipleBytes(){
            GridFile fs = new GridFile(db["tests"], "gfstream");
            GridFileStream gfs = fs.Create("multiplebytes.txt");
            Object id = gfs.GridFileInfo.Id;

            for(int x = 0; x < 256; x++){
                gfs.Write(BitConverter.GetBytes(x),0,4);
            }
            gfs.Close();
            
            Assert.AreEqual(1, db["tests"]["gfstream.chunks"].Count(new Document().Append("files_id", id)));
        }        
        
        [Test]
        public void TestLargeWrite(){
            GridFile fs = new GridFile(db["tests"], "gfstream");
            //GridFile fs = new GridFile(db["tests"]);
            GridFileStream gfs = fs.Create("largewrite.txt");
            Object id = gfs.GridFileInfo.Id;
            int chunks = 3;
            Byte[] buff = new byte[(256 * 1024) * chunks]; //intentionally bigger than default buffer size.
            for(int i = 0; i < buff.Length; i++){
                buff[i] = (byte)(i % 128);
            }
            gfs.Write(buff,0,buff.Length);
            Assert.AreEqual(buff.Length, gfs.Position);
            gfs.Close();
            Assert.AreEqual(chunks, db["tests"]["gfstream.chunks"].Count(new Document().Append("files_id", id)));
            //Assert.AreEqual(chunks, db["tests"]["fs.chunks"].Count(new Document().Append("files_id", id)));
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
            DropGridFileSystem("gfstream");
        }
        
        protected void DropGridFileSystem(string filesystem){
            try{
                db["tests"].MetaData.DropCollection(filesystem + ".files");
                db["tests"].MetaData.DropCollection(filesystem + ".chunks");
            }catch(MongoCommandException){}//if it fails it is because the collection isn't there to start with.
            
        }
        
    }
}
