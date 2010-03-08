using System;
using System.Collections.Generic;
using System.IO;

using MongoDB.Driver.Bson;

using NUnit.Framework;

namespace MongoDB.Driver.Bson
{
    [TestFixture]
    public class TestRoundTrips
    {

        [Test]
        public void TestDBRef(){
            Document source = new Document();
            source.Append("x",1).Append("ref",new DBRef("refs","ref1"));

            Document copy = WriteAndRead(source);

            Assert.IsTrue(copy.Contains("ref"));
            Assert.IsTrue(copy["ref"].GetType() == typeof(DBRef));

            DBRef sref = (DBRef)source["ref"];
            DBRef cref = (DBRef)copy["ref"];

            Assert.AreEqual(sref.Id, cref.Id);

        }

        [Test]
        public void TestDateLocal(){
            DateTime now = DateTime.Now;

            Document source = new Document();
            source.Append("d",now);

            Document copy = WriteAndRead(source);
            
            DateTime then = (DateTime)copy["d"];           
            then = then.ToLocalTime();
            
            Assert.AreEqual(now.Hour,then.Hour, "Date did not round trip right.");

        }
        
        [Test]
        public void TestDateUTC(){
            DateTime now = DateTime.UtcNow;
            
            Document source = new Document();
            source.Append("d",now);
            
            Document copy = WriteAndRead(source);           
            DateTime then = (DateTime)copy["d"];           
            
            Assert.AreEqual(now.Hour,then.Hour, "Date did not round trip right.");

        }

        [Test]
        public void TestGUID(){
            Guid expected = Guid.NewGuid();

            Document source = new Document();
            source.Append("uuid", expected);

            Guid read = (Guid)(WriteAndRead(source)["uuid"]);

            Assert.AreEqual(expected, read, "UUID did not round trip right.");
        }
        
        [Test]
        public void TestMultiDimensionalArray(){
            int[][] arr = new int[3][];
            for(int a = 0; a < arr.Length; a++){
                int x = a + 1;
                arr[a] = new int[]{x * 1, x * 2, x * 3};
            }
            
            Document expected = new Document(){{"arr", arr}};
            Document read = WriteAndRead(expected);
               
            Assert.AreEqual(expected.ToString(), read.ToString());
        }
        
        [Test]
        public void TestMixedArrayContents(){
            Object[] arr = new Object[]{new string[]{"one", "two"},
                                        new string[]{"three", "four"}, 
                                        new Document(){{"id", "six"}}};
            Document expected = new Document(){{"arr", arr}};
            Document read = WriteAndRead(expected);
            
            string json = @"{ ""arr"": [ [ ""one"", ""two"" ], [ ""three"", ""four"" ], { ""id"": ""six"" } ] }";
            Assert.AreEqual(json, expected.ToString());
            
            Assert.IsTrue(read["arr"] is IEnumerable<Object>, "Mixed array wasn't returned as IEnumerable<Object>");
            
            Assert.AreEqual(json, read.ToString());
        }
        
        [Test]
        public void TestSingleContentTypeArray(){
            string[] arr = new string[]{"one", "two", "three", "four"};

            Document expected = new Document(){{"arr", arr}};
            Document read = WriteAndRead(expected);
            
            string json = @"{ ""arr"": [ ""one"", ""two"", ""three"", ""four"" ] }";
            Assert.AreEqual(json, expected.ToString());
            
            Assert.IsTrue(read["arr"] is IEnumerable<string>, "Array wasn't returned as IEnumerable<string>");
            
            Assert.AreEqual(json, read.ToString());
            
        }
        
        [Test]
        public void TestEmptyArray(){
            Object[] arr = new Object[0];
            Document expected = new Document(){{"arr", arr}};
            Document read = WriteAndRead(expected);
            
            string json = @"{ ""arr"": [  ] }";
            Assert.AreEqual(json, expected.ToString());            
            Assert.IsTrue(read["arr"] is IEnumerable<Object>, "Empty array wasn't returned as IEnumerable<Object>");
            Assert.AreEqual(json, read.ToString());
        }
        
        protected Document WriteAndRead(Document source){
            MemoryStream ms = new MemoryStream();
            BsonWriter writer = new BsonWriter(ms);

            writer.Write(source);
            writer.Flush();
            ms.Seek(0, SeekOrigin.Begin);

            BsonReader reader = new BsonReader(ms);
            return reader.Read();
        }   
    }
}
