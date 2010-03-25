using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NUnit.Framework;


namespace MongoDB.Driver.Serialization.Descriptors
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
    }
}