using System;
using System.Diagnostics;
using System.Linq;
using MongoDB.Driver;
using Moq;
using NUnit.Framework;

namespace MongoDB.Linq.Tests {
    // ReSharper disable InconsistentNaming
    [TestFixture]
    public class TestMongoDocumentQuerySyntax {

        private IMongoQuery queryable;
        private Mock<IMongoCollection<Document>> collectionMock;
        private Mock<ICursor<Document>> cursorMock;

        [SetUp]
        public void Setup() {
            Debug.WriteLine("initializing queryable");
            collectionMock = new Mock<IMongoCollection<Document>>();
            cursorMock = new Mock<ICursor<Document>>();
            collectionMock.Setup(c => c.Find(It.IsAny<Document>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<Document>())).Returns(cursorMock.Object);
            queryable = new MongoQuery(new MongoQueryProvider(collectionMock.Object));
        }

        [Test]
        public void Can_use_in_query() {
            var q = (IMongoQuery)(from d in queryable where d.Key("foo").In("bar", "baz") select d);
            Assert.AreEqual(
                new Document().Add("foo", new Document().Add("$in", new[]{ "bar", "baz" })),
                q.Query);
        }

        [Test]
        public void Can_use_in_query_with_array_ref() {
            var a = new[] { "bar", "baz" };
            var q = (IMongoQuery)(from d in queryable where d.Key("foo").In(a) select d);
            Assert.AreEqual(
                new Document().Add("foo", new Document().Add("$in", new[] { "bar", "baz" })),
                q.Query);
        }

        [Test]
        public void Can_use_not_in_query() {
            var q = (IMongoQuery)(from d in queryable where d.Key("foo").NotIn("bar", "baz") select d);
            Assert.AreEqual(
                new Document().Add("foo", new Document().Add("$nin", new[] { "bar", "baz" })),
                q.Query);
        }

        [Test]
        public void Can_use_Equals() {
            var q = (IMongoQuery)(from d in queryable where d.Key("foo").Equals("bar") select d);
            Assert.AreEqual(new Document().Add("foo", "bar").ToString(), q.Query.ToString());
        }

        #region string operator overloads
        [Test]
        public void Can_use_equality_op_on_string() {
            var q = (IMongoQuery)(from d in queryable where d.Key("foo") == "bar" select d);
            Assert.AreEqual(new Document().Add("foo", "bar").ToString(), q.Query.ToString());
        }

        [Test]
        public void Can_use_inequality_op_on_string() {
            var q = (IMongoQuery)(from d in queryable where d.Key("foo") != "bar" select d);
            Assert.AreEqual(new Document().Add("foo", new Document().Add("$ne","bar")).ToString(), q.Query.ToString());
        }

        [Test]
        public void Can_use_equality_op_on_string_reversed() {
            var q = (IMongoQuery)(from d in queryable where "bar" == d.Key("foo") select d);
            Assert.AreEqual(new Document().Add("foo", "bar").ToString(), q.Query.ToString());
        }

        [Test]
        public void Can_use_inequality_op_on_string_reversed() {
            var q = (IMongoQuery)(from d in queryable where "bar" != d.Key("foo") select d);
            Assert.AreEqual(new Document().Add("foo", new Document().Add("$ne","bar")).ToString(), q.Query.ToString());
        }
        #endregion

        #region int operator overloads
        [Test]
        public void Can_use_equality_op_on_int() {
            var q = (IMongoQuery)(from d in queryable where d.Key("foo") == 10 select d);
            Assert.AreEqual(new Document().Add("foo", 10).ToString(), q.Query.ToString());
        }

        [Test]
        public void Can_use_inequality_op_on_int() {
            var q = (IMongoQuery)(from d in queryable where d.Key("foo") != 10 select d);
            Assert.AreEqual(new Document().Add("foo", new Document().Add("$ne", 10)).ToString(), q.Query.ToString());
        }

        [Test]
        public void Can_use_greater_than_op_on_int() {
            var q = (IMongoQuery)(from d in queryable where d.Key("foo") > 10 select d);
            Assert.AreEqual(new Document().Add("foo", new Document().Add("$gt", 10)).ToString(), q.Query.ToString());
        }

        [Test]
        public void Can_use_greater_than_or_equal_op_on_int() {
            var q = (IMongoQuery)(from d in queryable where d.Key("foo") >= 10 select d);
            Assert.AreEqual(new Document().Add("foo", new Document().Add("$gte", 10)).ToString(), q.Query.ToString());
        }

        [Test]
        public void Can_use_less_than_op_on_int() {
            var q = (IMongoQuery)(from d in queryable where d.Key("foo") < 10 select d);
            Assert.AreEqual(new Document().Add("foo", new Document().Add("$lt", 10)).ToString(), q.Query.ToString());
        }

        [Test]
        public void Can_use_less_than_or_equal_op_on_int() {
            var q = (IMongoQuery)(from d in queryable where d.Key("foo") <= 10 select d);
            Assert.AreEqual(new Document().Add("foo", new Document().Add("$lte", 10)).ToString(), q.Query.ToString());
        }

        [Test]
        public void Can_use_equality_op_on_int_reversed() {
            var q = (IMongoQuery)(from d in queryable where 10 == d.Key("foo") select d);
            Assert.AreEqual(new Document().Add("foo", 10).ToString(), q.Query.ToString());
        }

        [Test]
        public void Can_use_inequality_op_on_int_reversed() {
            var q = (IMongoQuery)(from d in queryable where 10 != d.Key("foo") select d);
            Assert.AreEqual(new Document().Add("foo", new Document().Add("$ne", 10)).ToString(), q.Query.ToString());
        }

        [Test]
        public void Can_use_greater_than_op_on_int_reversed() {
            var q = (IMongoQuery)(from d in queryable where 10 > d.Key("foo") select d);
            Assert.AreEqual(new Document().Add("foo", new Document().Add("$lt", 10)).ToString(), q.Query.ToString());
        }

        [Test]
        public void Can_use_greater_than_or_equal_op_on_int_reversed() {
            var q = (IMongoQuery)(from d in queryable where 10 >= d.Key("foo") select d);
            Assert.AreEqual(new Document().Add("foo", new Document().Add("$lte", 10)).ToString(), q.Query.ToString());
        }

        [Test]
        public void Can_use_less_than_op_on_int_reversed() {
            var q = (IMongoQuery)(from d in queryable where 10 < d.Key("foo") select d);
            Assert.AreEqual(new Document().Add("foo", new Document().Add("$gt", 10)).ToString(), q.Query.ToString());
        }

        [Test]
        public void Can_use_less_than_or_equal_op_on_int_reversed() {
            var q = (IMongoQuery)(from d in queryable where 10 <= d.Key("foo") select d);
            Assert.AreEqual(new Document().Add("foo", new Document().Add("$gte", 10)).ToString(), q.Query.ToString());
        }
        #endregion

        #region double operator overloads
        [Test]
        public void Can_use_equality_op_on_double() {
            var q = (IMongoQuery)(from d in queryable where d.Key("foo") == 10.1 select d);
            Assert.AreEqual(new Document().Add("foo", 10.1).ToString(), q.Query.ToString());
        }

        [Test]
        public void Can_use_inequality_op_on_double() {
            var q = (IMongoQuery)(from d in queryable where d.Key("foo") != 10.1 select d);
            Assert.AreEqual(new Document().Add("foo", new Document().Add("$ne", 10.1)).ToString(), q.Query.ToString());
        }

        [Test]
        public void Can_use_greater_than_op_on_double() {
            var q = (IMongoQuery)(from d in queryable where d.Key("foo") > 10.1 select d);
            Assert.AreEqual(new Document().Add("foo", new Document().Add("$gt", 10.1)).ToString(), q.Query.ToString());
        }

        [Test]
        public void Can_use_greater_than_or_equal_op_on_double() {
            var q = (IMongoQuery)(from d in queryable where d.Key("foo") >= 10.1 select d);
            Assert.AreEqual(new Document().Add("foo", new Document().Add("$gte", 10.1)).ToString(), q.Query.ToString());
        }

        [Test]
        public void Can_use_less_than_op_on_double() {
            var q = (IMongoQuery)(from d in queryable where d.Key("foo") < 10.1 select d);
            Assert.AreEqual(new Document().Add("foo", new Document().Add("$lt", 10.1)).ToString(), q.Query.ToString());
        }

        [Test]
        public void Can_use_less_than_or_equal_op_on_double() {
            var q = (IMongoQuery)(from d in queryable where d.Key("foo") <= 10.1 select d);
            Assert.AreEqual(new Document().Add("foo", new Document().Add("$lte", 10.1)).ToString(), q.Query.ToString());
        }

        [Test]
        public void Can_use_equality_op_on_double_reversed() {
            var q = (IMongoQuery)(from d in queryable where 10.1 == d.Key("foo") select d);
            Assert.AreEqual(new Document().Add("foo", 10.1).ToString(), q.Query.ToString());
        }

        [Test]
        public void Can_use_inequality_op_on_double_reversed() {
            var q = (IMongoQuery)(from d in queryable where 10.1 != d.Key("foo") select d);
            Assert.AreEqual(new Document().Add("foo", new Document().Add("$ne", 10.1)).ToString(), q.Query.ToString());
        }

        [Test]
        public void Can_use_greater_than_op_on_double_reversed() {
            var q = (IMongoQuery)(from d in queryable where 10.1 > d.Key("foo") select d);
            Assert.AreEqual(new Document().Add("foo", new Document().Add("$lt", 10.1)).ToString(), q.Query.ToString());
        }

        [Test]
        public void Can_use_greater_than_or_equal_op_on_double_reversed() {
            var q = (IMongoQuery)(from d in queryable where 10.1 >= d.Key("foo") select d);
            Assert.AreEqual(new Document().Add("foo", new Document().Add("$lte", 10.1)).ToString(), q.Query.ToString());
        }

        [Test]
        public void Can_use_less_than_op_on_double_reversed() {
            var q = (IMongoQuery)(from d in queryable where 10.1 < d.Key("foo") select d);
            Assert.AreEqual(new Document().Add("foo", new Document().Add("$gt", 10.1)).ToString(), q.Query.ToString());
        }

        [Test]
        public void Can_use_less_than_or_equal_op_on_double_reversed() {
            var q = (IMongoQuery)(from d in queryable where 10.1 <= d.Key("foo") select d);
            Assert.AreEqual(new Document().Add("foo", new Document().Add("$gte", 10.1)).ToString(), q.Query.ToString());
        }
        #endregion

        #region double operator overloads
        [Test]
        public void Can_use_equality_op_on_DateTime() {
            var dt = DateTime.Parse("2009-10-10T07:00:00.0000000Z");
            var q = (IMongoQuery)(from d in queryable where d.Key("foo") == dt select d);
            Assert.AreEqual(new Document().Add("foo",dt).ToString(), q.Query.ToString());
        }

        [Test]
        public void Can_use_inequality_op_on_DateTime() {
            var dt = DateTime.Parse("2009-10-10T07:00:00.0000000Z");
            var q = (IMongoQuery)(from d in queryable where d.Key("foo") != dt select d);
            Assert.AreEqual(new Document().Add("foo", new Document().Add("$ne", dt)).ToString(), q.Query.ToString());
        }

        [Test]
        public void Can_use_greater_than_op_on_DateTime() {
            var dt = DateTime.Parse("2009-10-10T07:00:00.0000000Z");
            var q = (IMongoQuery)(from d in queryable where d.Key("foo") > dt select d);
            Assert.AreEqual(new Document().Add("foo", new Document().Add("$gt", dt)).ToString(), q.Query.ToString());
        }

        [Test]
        public void Can_use_greater_than_or_equal_op_on_DateTime() {
            var dt = DateTime.Parse("2009-10-10T07:00:00.0000000Z");
            var q = (IMongoQuery)(from d in queryable where d.Key("foo") >= dt select d);
            Assert.AreEqual(new Document().Add("foo", new Document().Add("$gte", dt)).ToString(), q.Query.ToString());
        }

        [Test]
        public void Can_use_less_than_op_on_DateTime() {
            var dt = DateTime.Parse("2009-10-10T07:00:00.0000000Z");
            var q = (IMongoQuery)(from d in queryable where d.Key("foo") < dt select d);
            Assert.AreEqual(new Document().Add("foo", new Document().Add("$lt", dt)).ToString(), q.Query.ToString());
        }

        [Test]
        public void Can_use_less_than_or_equal_op_on_DateTime() {
            var dt = DateTime.Parse("2009-10-10T07:00:00.0000000Z");
            var q = (IMongoQuery)(from d in queryable where d.Key("foo") <= dt select d);
            Assert.AreEqual(new Document().Add("foo", new Document().Add("$lte", dt)).ToString(), q.Query.ToString());
        }

        [Test]
        public void Can_use_equality_op_on_DateTime_reversed() {
            var dt = DateTime.Parse("2009-10-10T07:00:00.0000000Z");
            var q = (IMongoQuery)(from d in queryable where dt == d.Key("foo") select d);
            Assert.AreEqual(new Document().Add("foo", dt).ToString(), q.Query.ToString());
        }

        [Test]
        public void Can_use_inequality_op_on_DateTime_reversed() {
            var dt = DateTime.Parse("2009-10-10T07:00:00.0000000Z");
            var q = (IMongoQuery)(from d in queryable where dt != d.Key("foo") select d);
            Assert.AreEqual(new Document().Add("foo", new Document().Add("$ne", dt)).ToString(), q.Query.ToString());
        }

        [Test]
        public void Can_use_greater_than_op_on_DateTime_reversed() {
            var dt = DateTime.Parse("2009-10-10T07:00:00.0000000Z");
            var q = (IMongoQuery)(from d in queryable where dt > d.Key("foo") select d);
            Assert.AreEqual(new Document().Add("foo", new Document().Add("$lt", dt)).ToString(), q.Query.ToString());
        }

        [Test]
        public void Can_use_greater_than_or_equal_op_on_DateTime_reversed() {
            var dt = DateTime.Parse("2009-10-10T07:00:00.0000000Z");
            var q = (IMongoQuery)(from d in queryable where dt >= d.Key("foo") select d);
            Assert.AreEqual(new Document().Add("foo", new Document().Add("$lte", dt)).ToString(), q.Query.ToString());
        }

        [Test]
        public void Can_use_less_than_op_on_DateTime_reversed() {
            var dt = DateTime.Parse("2009-10-10T07:00:00.0000000Z");
            var q = (IMongoQuery)(from d in queryable where dt < d.Key("foo") select d);
            Assert.AreEqual(new Document().Add("foo", new Document().Add("$gt", dt)).ToString(), q.Query.ToString());
        }

        [Test]
        public void Can_use_less_than_or_equal_op_on_DateTime_reversed() {
            var dt = DateTime.Parse("2009-10-10T07:00:00.0000000Z");
            var q = (IMongoQuery)(from d in queryable where dt <= d.Key("foo") select d);
            Assert.AreEqual(new Document().Add("foo", new Document().Add("$gte", dt)).ToString(), q.Query.ToString());
        }
        #endregion
    }
    // ReSharper restore InconsistentNaming
}
