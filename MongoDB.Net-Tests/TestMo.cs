using NUnit.Framework;

namespace MongoDB.Driver
{
    [TestFixture]
    public class TestMo
    {
        [Test]
        public void TestSimpleInc(){
            var mo = Mo.Inc("A", 10);

            var expected = new Document("$inc", new Document("A", 10));

            Assert.AreEqual(expected, mo);
        }

        [Test]
        public void TestMultipleInc()
        {
            var mo = Mo.Inc(new Document("A",1).Add("B",2));

            var expected = new Document("$inc", new Document("A", 1).Add("B",2));

            Assert.AreEqual(expected, mo);
        }

        [Test]
        public void TestMultipleModifiers()
        {
            var mo = Mo.Inc("A", 1) & Mo.Inc("B", 2);

            var expected = new Document("$inc", new Document("A", 1).Add("B", 2));

            Assert.AreEqual(expected, mo);
        }
    }
}