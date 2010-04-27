using NUnit.Framework;
using MongoDB.Driver.Bson;

namespace MongoDB.Driver
{
    [TestFixture]
    public class TestOp
    {
        [Test]
        public void ConjunctionsWith2Operators()
        {
            var op = Op.GreaterThan(10) & Op.LessThan(20);

            Assert.AreEqual(10, op["$gt"]);
            Assert.AreEqual(20, op["$lt"]);
        }

        [Test]
        public void ConjunctionsWith3Operators()
        {
            var op = Op.GreaterThan(10) & Op.LessThan(20) & Op.Mod(3, 1);

            Assert.AreEqual(10, op["$gt"]);
            Assert.AreEqual(20, op["$lt"]);
            Assert.AreEqual(new [] { 3, 1 }, op["$mod"]);
        }

        [Test]
        public void Negation()
        {
            var op = !Op.GreaterThan(10);

            var negated = (Op)op["$not"];

            Assert.AreEqual(10, negated["$gt"]);
        }

        [Test]
        public void GreaterThan()
        {
            var op = Op.GreaterThan(10);

            Assert.AreEqual(10, op["$gt"]);
        }

        [Test]
        public void GreaterThanOrEqual()
        {
            var op = Op.GreaterThanOrEqual(10);

            Assert.AreEqual(10, op["$gte"]);
        }

        [Test]
        public void LessThan()
        {
            var op = Op.LessThan(10);

            Assert.AreEqual(10, op["$lt"]);
        }

        [Test]
        public void LessThanOrEqual()
        {
            var op = Op.LessThanOrEqual(10);

            Assert.AreEqual(10, op["$lte"]);
        }

        [Test]
        public void NotEqual()
        {
            var op = Op.NotEqual(10);

            Assert.AreEqual(10, op["$ne"]);
        }

        [Test]
        public void In()
        {
            var op = Op.In(10,11,12);

            Assert.AreEqual(new[] { 10, 11, 12 }, op["$in"]);
        }

        [Test]
        public void NotIn()
        {
            var op = Op.NotIn(10, 11, 12);

            Assert.AreEqual(new[] { 10, 11, 12 }, op["$nin"]);
        }

        [Test]
        public void All()
        {
            var op = Op.All(10, 11, 12);

            Assert.AreEqual(new[] { 10, 11, 12 }, op["$all"]);
        }

        [Test]
        public void Mod()
        {
            var op = Op.Mod(10, 1);

            Assert.AreEqual(new[] { 10, 1 }, op["$mod"]);
        }

        [Test]
        public void Size()
        {
            var op = Op.Size(10);

            Assert.AreEqual(10, op["$size"]);
        }

        [Test]
        public void Exists()
        {
            var op = Op.Exists();

            Assert.AreEqual(true, op["$exists"]);
        }

        [Test]
        public void NotExists()
        {
            var op = Op.NotExists();

            Assert.AreEqual(false, op["$exists"]);
        }

        [Test]
        public void Type()
        {
            var op = Op.Type(BsonDataType.Boolean);

            Assert.AreEqual((int)BsonDataType.Boolean, op["$type"]);
        }

        [Test]
        public void Where()
        {
            var op = Op.Where("return this.a == 3 || this.b == 4;");

            Assert.AreEqual("return this.a == 3 || this.b == 4;",op["$where"]);
        }
    }
}