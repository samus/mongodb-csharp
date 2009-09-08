/*
 * User: scorder
 * Date: 7/17/2009
 */
using System;
using System.IO;

using NUnit.Framework;

namespace MongoDB.Driver.Bson
{

    [TestFixture]
    public class TestBsonReaderWriter
    {
        MemoryStream mem;
        BsonReader reader;
        BsonWriter writer;
        
        
        //Not a true unit test but it will work for being lazy right now.
        [Test]
        public void TestReadString(){
            InitStreams();
            writer.Write("test");
            FlushAndGotoBegin();
            
            Assert.AreEqual("test", reader.ReadString(5));      
            
            long start = mem.Position;
            String[] values = {"one", "two", "three"};
            foreach(string val in values){
                writer.Write(val);
            }
            writer.Flush();
            mem.Seek(start, SeekOrigin.Begin);
            
            foreach(string val in values){
                Assert.AreEqual(val,reader.ReadString(val.Length + 1));
            }
        }
        
        [Test]
        public void TestReadStringNoLength(){       
            InitStreams();
            writer.Write("test");
            FlushAndGotoBegin();
            
            Assert.AreEqual("test", reader.ReadString());       
        }

        [Test]
        public void TestBoolean(){
            InitStreams();
            writer.Write(true);
            FlushAndGotoBegin();
            
            Assert.IsTrue(reader.ReadBoolean());
        }
        
        [Test]
        public void TestInteger(){
            InitStreams();
            writer.Write(1);
            FlushAndGotoBegin();
            
            Assert.AreEqual(1,reader.ReadInt32());
        }
        
        [Test]
        public void TestByte(){
            byte v = (byte)1;
            InitStreams();
            writer.Write(v);
            FlushAndGotoBegin();
            
            Assert.AreEqual(v,reader.ReadByte());
        }       

        protected void InitStreams(){
            mem = new MemoryStream();
            reader = new BsonReader(mem);
            writer = new BsonWriter(mem);                   
        }
        
        protected void FlushAndGotoBegin(){
            writer.Flush();
            mem.Seek(0,SeekOrigin.Begin);           
        }
    }
}
