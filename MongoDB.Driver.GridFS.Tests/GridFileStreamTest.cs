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
        GridFile fs;

        String filesystem = "gfstream";

        [Test]
        public void TestWrite(){
            GridFileStream gfs = fs.Create("test.txt");
            Object id = gfs.GridFileInfo.Id;

            for(byte b = (byte)0; b < 128; b++){
                gfs.WriteByte(b);
            }
            gfs.Close();

            Assert.AreEqual(1, CountChunks(id));
            Document chunk = GrabChunk(id, 0);
            Binary bin = (Binary)chunk["data"];
            Assert.AreEqual(127, bin.Bytes[127]);
            Assert.AreEqual(0, bin.Bytes[0]);
        }

        [Test]
        public void TestWriteMultipleBytes(){
            GridFileStream gfs = fs.Create("multiplebytes.txt");
            Object id = gfs.GridFileInfo.Id;

            for(int x = 0; x < 256; x++){
                gfs.Write(BitConverter.GetBytes(x),0,4);
            }
            gfs.Close();

            Assert.AreEqual(1, CountChunks(id));
        }

        [Test]
        public void TestWriteTo3Chunks(){
            GridFileStream gfs = fs.Create("largewrite.txt");

            Object id = gfs.GridFileInfo.Id;
            int chunks = 3;
            int buffsize = 256 * 1024 * chunks;
            Byte[] buff = CreateBuffer(buffsize, (byte)6);
            gfs.Write(buff,0,buff.Length);
            Assert.AreEqual(buff.Length, gfs.Position);
            gfs.Close();
            Assert.AreEqual(chunks, CountChunks(id));
        }


        [Test]
        public void TestWriteMultipleBytesWithOffset(){
            String filename = "multioffset.txt";
            GridFileStream gfs = fs.Create(filename);
            Object id = gfs.GridFileInfo.Id;
            int chunks = 2;
            int buffsize = 256 * 1024 * chunks;
            byte[] buff = new byte[buffsize];

            int x = 1;
            for(int i = 4; i < buff.Length; i+=4){
                Array.Copy(BitConverter.GetBytes(x++),0,buff,i,4);
            }
            gfs.Write(buff,4,buff.Length - 4);
            gfs.Close();
            
            Assert.AreEqual(2, CountChunks(id));
            
            gfs = fs.OpenRead(filename);
            buff = new Byte[4];
            int read;
            int val;
            for(int i = 1; i <= buffsize/4; i++){
                if(i == 65536){
                    Console.WriteLine("break");
                }                
                read = gfs.Read(buff,0,4);
                val = BitConverter.ToInt32(buff, 0);
                Assert.AreEqual(4, read, "Not enough bytes were read");

                Assert.AreEqual(i,val, "value read back was not the same as written. Pos: " + gfs.Position);
            }

        }

        [Test]
        public void TestNonSequentialWriteToOneChunk(){
            string filename = "nonsequential1.txt";
            GridFileStream gfs = fs.Create(filename);
            Object id = gfs.GridFileInfo.Id;
            int chunksize = gfs.GridFileInfo.ChunkSize;

            gfs.Seek(chunksize/2, SeekOrigin.Begin);
            byte[] two = new byte[]{2};
            for(int i = chunksize; i > chunksize/2; i--){
                gfs.Write(two, 0, 1);
            }

            gfs.Seek(0, SeekOrigin.Begin);
            byte[] one = new byte[]{1};
            for(int i = 0; i < chunksize/2; i++){
                gfs.Write(one, 0, 1);
            }
            gfs.Close();

            Assert.AreEqual(1, CountChunks(id));
            Document chunk = GrabChunk(id, 0);
            Binary b = (Binary)chunk["data"];
            Assert.AreEqual(2, b.Bytes[chunksize-1]);
            Assert.AreEqual(2, b.Bytes[chunksize/2]);
            Assert.AreEqual(1, b.Bytes[chunksize/2 -1]);
            Assert.AreEqual(1, b.Bytes[0]);
        }

        [Test]
        public void TestNonSequentialWriteToPartialChunk(){
            string filename = "nonsequentialpartial.txt";
            GridFileStream gfs = fs.Create(filename);
            Object id = gfs.GridFileInfo.Id;
            int chunksize = gfs.GridFileInfo.ChunkSize;
            gfs.Write(BitConverter.GetBytes(0),0,4);
            Assert.AreEqual(4, gfs.Position);
            gfs.Write(BitConverter.GetBytes(7),0,4);
            Assert.AreEqual(8, gfs.Position);
            gfs.Seek(0, SeekOrigin.Begin);
            gfs.Write(BitConverter.GetBytes(15),0,4);
            gfs.Close();
            
            Document chunk = GrabChunk(id, 0);
            Binary b = (Binary)chunk["data"];
            Assert.AreEqual(8,b.Bytes.Length);
            
        }

        [Test]
        public void TestNonSequentialWriteToTwoChunks(){
            GridFileStream gfs = fs.Create("nonsequential2.txt");

            Object id = gfs.GridFileInfo.Id;
            int chunks = 3;
            int buffsize = 256 * 1024;
            for(int c = 0; c < chunks; c++){
            	Byte[] buff = CreateBuffer(buffsize, (byte)c);
                if(c == 2){
                	gfs.Seek(0, SeekOrigin.Begin); //On last chunk seek to start.
                }
                gfs.Write(buff,0,buff.Length);
            }
            Assert.AreEqual(buffsize, gfs.Position, "Position is incorrect");
            gfs.Close();
            Assert.AreEqual(chunks - 1, CountChunks(id));
            Document chunk = GrabChunk(id, 0);
            Binary b = (Binary)chunk["data"];
            Assert.AreEqual(2, b.Bytes[buffsize-1]);
            Assert.AreEqual(2, b.Bytes[0]);
            chunk = GrabChunk(id, 1);
            b = (Binary)chunk["data"];
            Assert.AreEqual(1, b.Bytes[buffsize-1]);
        }

        [Test]
        public void TestRead(){
            string filename = "readme.txt";
            GridFileStream gfs = fs.Create(filename);
            for(int i = 1; i <= 50; i++){
                gfs.Write(BitConverter.GetBytes(i), 0, 4);
            }
            gfs.Close();

            gfs = fs.OpenRead(filename);

            Byte[] buff = new Byte[4];
            int read;
            for(int i = 1; i <= 50; i++){
                read = gfs.Read(buff,0,4);
                Assert.AreEqual(4, read, "Not enough bytes were read");
                Assert.AreEqual(i,BitConverter.ToInt32(buff, 0), "value read back was not the same as written");
            }

        }

        #region File API compatibility

        [Test]
        public void TestSeekingBeyondEOF(){
            int buffsize = 256;
            FileStream gfs = File.Create("seektest.txt",buffsize);//,FileOptions.DeleteOnClose);
            int chunks = 3;
            //int buffsize = 256 * 1024;
            for(int c = 0; c < chunks; c++){
                Byte[] buff = new byte[buffsize];
                for(int i = 0; i < buff.Length; i++){
                    buff[i] = (byte)(c);
                }
                if(c == 2) gfs.Seek(0, SeekOrigin.Begin); //On last chunk seek to start.
                gfs.Write(buff,0,buff.Length);
                gfs.Seek(5,SeekOrigin.Current);
            }
            gfs.Seek(50,SeekOrigin.End);
            gfs.Write(new byte[]{5},0,1);


        }

        #endregion


        [TestFixtureSetUp]
        public void Init(){
            db.Connect();
            fs = new GridFile(db["tests"], filesystem);
            CleanDB(); //Run here instead of at the end so that the db can be examined after a run.
        }

        [TestFixtureTearDown]
        public void Dispose(){
            db.Disconnect();
        }

        protected void CleanDB(){
            //Any collections that we might want to delete before the tests run should be done here.
            DropGridFileSystem(filesystem);
        }

        protected void DropGridFileSystem(string filesystem){
            try{
                db["tests"].MetaData.DropCollection(filesystem + ".files");
                db["tests"].MetaData.DropCollection(filesystem + ".chunks");
            }catch(MongoCommandException){}//if it fails it is because the collection isn't there to start with.

        }

        protected long CountChunks(Object fileid){
            return db["tests"][filesystem + ".chunks"].Count(new Document().Append("files_id", fileid));
        }

        protected Document GrabChunk(Object fileid, int chunk){
            return db["tests"][filesystem + ".chunks"].FindOne(new Document().Append("files_id", fileid).Append("n", chunk));
        }
        
        protected byte[] CreateBuffer(int size, byte fill){
            Byte[] buff = new byte[size];
            for(int i = 0; i < buff.Length; i++){
                buff[i] = fill;
            }
            return buff;
        }

    }
}
