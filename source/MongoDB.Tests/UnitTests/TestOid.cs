using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using NUnit.Framework;

namespace MongoDB.UnitTests
{
    [TestFixture]
    public class TestOid
    {
        [Test]
        public void TestCtor()
        {
            var thrown = false;
            try
            {
                new Oid("4a7067c30a57000000008ecb");
            }
            catch(ArgumentException)
            {
                thrown = true;
            }
            Assert.IsFalse(thrown, "ID should be fine.");
        }

        [Test]
        public void TestDate()
        {
            const string hex = "4B458B95D114BE541B000000";
            var oid = new Oid(hex);
            //Expected: 2010-01-07 02:24:56.633
            var expected = new DateTime(2010, 1, 7, 7, 21, 57, DateTimeKind.Utc);
            Assert.AreEqual(expected, oid.Created);
        }

        [Test]
        public void TestDecode()
        {
            const string hex = "4a7067c30a57000000008ecb";
            var oid = new Oid(hex);

            Assert.AreEqual(hex, oid.ToString());
        }

        [Test]
        public void TestEquals()
        {
            const string hex = "4a7067c30a57000000008ecb";
            Assert.AreEqual(new Oid(hex), new Oid(hex));
        }

        [Test]
        public void TestFormatJ()
        {
            const string hex = "4a7067c30a57000000008ecb";
            var oid = new Oid(hex);

            Assert.AreEqual("\"" + hex + "\"", oid.ToString("J"));
        }

        [Test]
        public void TestIDCharacters()
        {
            var thrown = false;
            try
            {
                new Oid("BADBOYc30a57000000008ecb");
            }
            catch(Exception)
            {
                thrown = true;
            }
            Assert.IsTrue(thrown, "No invalid characters exception thrown");
        }

        [Test]
        public void TestIDLength()
        {
            var thrown = false;
            try
            {
                new Oid("BAD0");
            }
            catch(Exception)
            {
                thrown = true;
            }
            Assert.IsTrue(thrown, "No length exception thrown");
        }

        [Test]
        public void TestNewOidFromToString()
        {
            const string hex = "4B458B95D114BE541B000000";
            var firstOid = new Oid(hex);
            var secondOid = new Oid(firstOid.ToString());

            Assert.AreEqual(firstOid.ToString(), secondOid.ToString());
        }

        [Test]
        public void TestNotEquals()
        {
            const string hex = "4a7067c30a57000000008ecb";
            const string hex2 = "4a7067c30a57000000008ecc";
            Assert.AreNotEqual(new Oid(hex), new Oid(hex2));
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestNullValue()
        {
            new Oid(String.Empty);
        }

        [Test]
        public void TestOidCanBeSerialized()
        {
            var serializer = new BinaryFormatter();

            var oidSource = Oid.NewOid();
            Oid oidDesc;
            using(var mem = new MemoryStream())
            {
                serializer.Serialize(mem, oidSource);
                mem.Position = 0;
                oidDesc = (Oid)serializer.Deserialize(mem);
            }

            Assert.AreEqual(oidSource, oidDesc);
        }

        [Test]
        public void TestOidComparisons()
        {
            var lower = new Oid("4a7067c30a57000000008ecb");
            var higher = new Oid("5a7067c30a57000000008ecb");

            Assert.AreEqual(1, lower.CompareTo(null));
            Assert.AreEqual(1, higher.CompareTo(lower));

            Assert.IsTrue(lower < higher);
            Assert.IsTrue(higher > lower);
        }

        [Test]
        public void TestOidEquality()
        {
            var val = new Oid("4a7067c30a57000000008ecb");
            var other = new Oid("4a7067c30a57000000008ecb");

            Assert.IsTrue(val.Equals(other), "Equals(Oid) did not work");
            Assert.IsTrue(val == other, "== operator did not work");
            Assert.IsTrue(val == val, "Objects should be equal to itself.");
        }

        [Test]
        public void TestOidEqualityToNull()
        {
            var val = Oid.NewOid();
            Oid other = null;
            Oid other2 = null;
            Assert.AreNotEqual(val, other);
            Assert.AreNotEqual(other, val);
            Assert.IsTrue(other == other2);
        }

        [Test]
        public void TestOidFromBytes()
        {
            var bytes = new byte[] {1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12};
            var hex = "0102030405060708090a0b0c";

            var bval = new Oid(bytes);
            var sval = new Oid(hex);
            Assert.AreEqual(bval, sval);
        }

        [Test]
        public void TestOidInEquality()
        {
            var val = new Oid("4a7067c30a57000000008ecb");
            var other = new Oid("5a7067c30a57000000008ecb");
            Oid nilo = null;

            Assert.IsFalse(val == null);
            Assert.IsFalse(nilo == val);
            Assert.IsFalse(val == nilo);
            Assert.IsFalse(val == other);
            Assert.IsFalse(val.Equals(other));
            Assert.IsTrue(val != null);
            Assert.IsTrue(val != other);
        }

        [Test]
        public void TestQuoteCharacters()
        {
            var val = new Oid(@"""4a7067c30a57000000008ecb""");
            try
            {
                new Oid(val.ToString());
            }
            catch(ArgumentException)
            {
                Assert.Fail("Creating an Oid from the json representation should not fail.");
            }
        }

        [Test]
        public void TestToByteArray()
        {
            var bytes = new byte[] {1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12};

            var bval = new Oid(bytes);
            var bytes2 = bval.ToByteArray();

            Assert.IsNotNull(bytes2);
            Assert.AreEqual(12, bytes2.Length);
            Assert.AreEqual(bytes, bytes2);
        }
    }
}