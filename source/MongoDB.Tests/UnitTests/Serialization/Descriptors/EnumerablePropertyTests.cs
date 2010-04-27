using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;

namespace MongoDB.UnitTests.Serialization.Descriptors
{
    [TestFixture]
    public class EnumerablePropertyTests : SerializationTestBase
    {
        public class Enumerable
        {
            public IEnumerable A { get; set; }
        }

        [Test]
        public void CanDeserializeAnSimpleArrayAsIEnumerable()
        {
            var e = new Enumerable();
            e.A = new ArrayList {  1, 2 };
            string bson = Serialize<Enumerable>(e);
            Assert.AreEqual("GwAAAARBABMAAAAQMAABAAAAEDEAAgAAAAAA", bson);
        }

        [Test]
        public void CanDeserializeAnSimpleArrayAsIEnumerableUsingAnonymousType()
        {
            string bson = Serialize<Enumerable>(new { A = new[] { 1, 2 } });
            Assert.AreEqual("GwAAAARBABMAAAAQMAABAAAAEDEAAgAAAAAA", bson);
        }

        public class EnumerableOfIntegers
        {
            public IEnumerable<int> A { get; set; }
        }

        [Test]
        public void CanDeserializeAnSimpleArrayAsIEnumerableOfInt()
        {
            var e = new EnumerableOfIntegers();
            e.A = new List<int> { 1, 2 };
            string bson = Serialize<EnumerableOfIntegers>(e);
            Assert.AreEqual("GwAAAARBABMAAAAQMAABAAAAEDEAAgAAAAAA", bson);
        }

        [Test]
        public void CanDeserializeAnSimpleArrayAsIEnumerableOfIntUsingAnonymousType()
        {
            string bson = Serialize<EnumerableOfIntegers>(new { A = new[] { 1, 2 } });
            Assert.AreEqual("GwAAAARBABMAAAAQMAABAAAAEDEAAgAAAAAA", bson);
        }
    }
}