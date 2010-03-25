using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MongoDB.Driver;

using NUnit.Framework;

namespace MongoDB.Driver.Serialization.Descriptors
{
    [TestFixture]
    public class OperatorTests : SerializationTestBase
    {
        public class OperatorProperty
        {
            public int A { get; set; }
        }

        [Test]
        public void CanSerializeWithStandardOperator()
        {
            var bson = Serialize<OperatorProperty>(new { A = new Document("$gt", 12) });

            Assert.AreEqual("FgAAAANBAA4AAAAQJGd0AAwAAAAAAA==", bson);
        }

        [Test]
        public void CanSerializeWithMetaOperator()
        {
            var bson = Serialize<OperatorProperty>(new { A = new Document("$not", new Document("$gt", 12)) });

            Assert.AreEqual("IQAAAANBABkAAAADJG5vdAAOAAAAECRndAAMAAAAAAAA", bson);
        }

        [Test]
        public void CanSerializeWithComplexOperators()
        {
            var bson = Serialize<OperatorProperty>(new { A = new Document("$gt", 12).Add("$not", new Document("$gt", 24)) });

            Assert.AreEqual("KgAAAANBACIAAAAQJGd0AAwAAAADJG5vdAAOAAAAECRndAAYAAAAAAAA", bson);
        }
    }
}