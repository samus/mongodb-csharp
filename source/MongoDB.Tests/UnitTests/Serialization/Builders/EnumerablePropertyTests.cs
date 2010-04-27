using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;

namespace MongoDB.UnitTests.Serialization.Builders
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
            //{ A: [1, 2] }
            const string bson = "GwAAAARBABMAAAAQMAABAAAAEDEAAgAAAAAA";
            var simpleArray = Deserialize<Enumerable>(bson);
            Assert.IsNotNull(simpleArray);
            Assert.IsNotNull(simpleArray.A);
            var list = new List<object>();
            foreach (var value in simpleArray.A)
                list.Add(value);
            Assert.AreEqual(2, list.Count);
            Assert.Contains(1, list);
            Assert.Contains(2, list);
        }

        public class EnumerableOfIntegers
        {
            public IEnumerable<int> A { get; set; }
        }

        [Test]
        public void CanDeserializeAnSimpleArrayAsIEnumerableOfInt()
        {
            //{ A: [1, 2] }
            const string bson = "GwAAAARBABMAAAAQMAABAAAAEDEAAgAAAAAA";
            var simpleArray = Deserialize<EnumerableOfIntegers>(bson);
            Assert.IsNotNull(simpleArray);
            Assert.IsNotNull(simpleArray.A);
            var list = new List<int>(simpleArray.A);
            Assert.AreEqual(2, list.Count);
            Assert.Contains(1, list);
            Assert.Contains(2, list);
        }

        [Test]
        public void CanTransfromASimpleArrayToItsMostEqualTypeInADocument(){
            var bson = Serialize(new Document().Add("A", new[] {"text"}));

            var simpleArray = Deserialize<Document>(bson);

            Assert.AreEqual(1, simpleArray.Count);

            var array = simpleArray["A"];

            Assert.IsNotNull(array);
            Assert.IsInstanceOfType(typeof(IList<string>),array);

            var stringArray = (IList<string>)array;
            Assert.AreEqual(1,stringArray.Count);
            Assert.Contains("text",(ICollection)stringArray);
        } 
    }
}
