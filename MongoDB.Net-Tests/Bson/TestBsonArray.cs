/*
 * User: scorder
 */

using System;
using System.IO;

using NUnit.Framework;

namespace MongoDB.Driver.Bson
{
    [TestFixture]
    public class TestBsonArray
    {
        [Test]
        public void TestType(){
            Assert.AreEqual(BsonDataType.Array, (BsonDataType)new BsonArray().TypeNum);
        }
        
        [Test]
        public void TestOnlyNumericKeys(){
            bool thrown = false;
            BsonArray ba = new BsonArray();
            ba.Add("1","A");
            Assert.IsNotNull(ba["1"]);
            try {
                ba.Add("A","A");
            } catch (ArgumentOutOfRangeException) {
                thrown = true;
            }
            if(thrown != true) Assert.Fail("Exception should have been thrown");
            
            thrown = false;
            try {
                ba["A"] = new BsonElement("A", new BsonString("A"));
            } catch (ArgumentOutOfRangeException) {
                thrown = true;
            }
            if(thrown != true) Assert.Fail("Exception should have been thrown");            
        }
        
        [Test]
        public void TestKeysCanBeInts(){
            BsonArray ba = new BsonArray();
            ba.Add(1,"A");
            Assert.AreEqual("A", ba[1].Val.ToNative());
        }
        
        [Test]
        public void TestKeyHoles(){
            BsonArray ba = new BsonArray();
            ba.Add(1,"A");
            ba.Add(3,"C");
            Assert.IsNull(ba[2]);
        }
        
        [Test]
        public void TestKeyOrdering(){
            BsonArray ba = new BsonArray();
            ba.Add(1,"A");
            ba.Add(2,"B");
            ba.Add(5,"E");
            ba.Add(3,"C");
            ba.Add(4,"D");
            
            int ikey = 1;
            foreach(string key in ba.Keys){
                Assert.AreEqual(ikey, int.Parse(key));
                ikey++;
            }
        }
		[Test]
		public void TestToNativeProducesArrayWithAllValuesTheSameType(){
			Document[] songs = new[] {
				new Document().Append("title", "Let The Music Set You Free").Append("length", "5:15"),
				new Document().Append("title", "Sally Likes to Run").Append("length", "4:06"),
				new Document().Append("title", "Deliveries After Dark").Append("length", "4:17"),
				new Document().Append("title", "Theme From The Godfather").Append("length", "3:06"),
				new Document().Append("title", "Grown Man Crying Blues").Append("length", "8:09"),
			};
			BsonArray ba = new BsonArray();
			ba.Add(1,"A");
			ba.Add(2,"B");
			ba.Add(3,"C");			
			Object obj = ba.ToNative();
			Assert.AreEqual(typeof(string[]),obj.GetType());
		}
		
		[Test]
		public void TestElementsSameType(){
			BsonArray ba = new BsonArray();
			ba.Add(1,"A");
			ba.Add(2,"B");
			ba.Add(3,"C");
			
			Assert.IsTrue(ba.ElementsSameType());
			
		}
    }
}
