using NUnit.Framework;
using System.Collections.Generic;
using MongoDB.Driver.Attributes;

namespace MongoDB.Driver.Serialization.Descriptors
{
    [TestFixture]
    public class WhereTests : SerializationTestBase
    {
        public class WhereClass
        {
            [MongoName("a")]
            public List<WhereChildA> A { get; set; }

            [MongoName("c")]
            public WhereChildC C { get; set; }
        }

        public class WhereChildA
        {
            [MongoName("b")]
            public int B { get; set; }
        }

        public class WhereChildC
        {
            [MongoName("d")]
            public int D { get; set; }
        }

        [Test]
        public void CanSerializeWithChild()
        {
            var expected = Serialize(Op.Where("this.c.d > 10"));
            var bson = Serialize<WhereClass>(Op.Where("this.C.D > 10"));
            Assert.AreEqual(expected, bson);
        }

        [Test]
        public void CanSerializeWithChildIndexer()
        {
            var expected = Serialize(Op.Where("this.a[0].b > 10"));
            var bson = Serialize<WhereClass>(Op.Where("this.A[0].B > 10"));
            Assert.AreEqual(expected, bson);
        }

        [Test]
        public void CanSerializeWithNonMember()
        {
            var expected = Serialize(Op.Where("this.a.length > 10"));
            var bson = Serialize<WhereClass>(Op.Where("this.A.length > 10"));
            Assert.AreEqual(expected, bson);
        }

        [Test]
        public void CanSerializeWithMethod()
        {
            var expected = Serialize(Op.Where("this.a.awesome().someProperty > 10"));
            var bson = Serialize<WhereClass>(Op.Where("this.A.awesome().someProperty > 10"));
            Assert.AreEqual(expected, bson);
        }

        [Test]
        public void CanSerializeComplex()
        {
            var expected = Serialize(Op.Where("this.a[4].b > this.c.d && this.c.d == 2"));
            var bson = Serialize<WhereClass>(Op.Where("this.A[4].B > this.C.D && this.C.D == 2"));
            Assert.AreEqual(expected, bson);
        }
    }
}