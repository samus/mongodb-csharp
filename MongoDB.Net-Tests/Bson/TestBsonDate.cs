/*
 * User: scorder
 */

using System;
using NUnit.Framework;

namespace MongoDB.Driver.Bson
{
    [TestFixture]
    public class TestBsonDate
    {
        [Test]
        public void TestPosixEpochConvertsTo1_1_1970(){
            long epoch = 0;
            BsonDate bepoch = new BsonDate(epoch);
            Assert.AreEqual(0, bepoch.Val);
        }
        [Test]
        public void TestNet1_1_1970IsZero(){
            DateTime nepoch = new DateTime(1970,1,1,0,0,0,DateTimeKind.Utc);
            BsonDate bd = new BsonDate(nepoch);
            Assert.AreEqual(0, bd.Val);
        }
        
        [Test]
        public void TestConversionOfNow(){
            BsonDate bd1 = new BsonDate(DateTime.Now);
            BsonDate bd2 = new BsonDate(bd1.Val);
            Assert.AreEqual(bd1.Val, bd2.Val, "Dates weren't the same value.");
        
        }
        [Test]
        public void TestToNative(){
            DateTime now = DateTime.Now.ToUniversalTime();
            BsonDate bd = new BsonDate(now);
            DateTime bnow = (DateTime)bd.ToNative();
            Assert.AreEqual(now.Date,bnow.Date, "Native conversion of date failed.");
            
            //.Net uses fractional milliseconds so there is a precision loss.
            //Just test the hour, minute, second, and milliseconds
            Assert.AreEqual(now.Hour, bnow.Hour, "Time differed");
            Assert.AreEqual(now.Minute, bnow.Minute, "Time differed");
            Assert.AreEqual(now.Second, bnow.Second, "Time differed");
            Assert.AreEqual(now.Millisecond, bnow.Millisecond, "Time differed");
        }
    }
}
