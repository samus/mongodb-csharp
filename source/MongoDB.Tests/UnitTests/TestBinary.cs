using NUnit.Framework;
using System.Linq;

namespace MongoDB.UnitTests
{
    [TestFixture]
    public class TestBinary
    {
        [Test]
        public void CanCreateBinary(){
            var binary = new Binary();
            Assert.IsNull(binary.Bytes);
            Assert.AreEqual(Binary.TypeCode.Unknown, binary.Subtype);
        }

        [Test]
        public void CanCreateBinaryFromNull(){
            var binary = new Binary(null);
            Assert.IsNull(binary.Bytes);
            Assert.AreEqual(Binary.TypeCode.General, binary.Subtype);
        }

        [Test]
        public void CanCreateBinaryFromBytes(){
            var bytes = new byte[] { 10 };
            var binary = new Binary(bytes);
            Assert.AreEqual(bytes,binary.Bytes);
            Assert.AreEqual(Binary.TypeCode.General, binary.Subtype);
        }

        [Test]
        public void CanCreateBinaryFromBytesAndSubtype(){
            var bytes = new byte[] {10};
            var binary = new Binary(bytes,Binary.TypeCode.UserDefined);
            Assert.AreEqual(bytes, binary.Bytes);
            Assert.AreEqual(Binary.TypeCode.UserDefined, binary.Subtype);
        }

        [Test]
        public void CanImplicitConvertedToBytes(){
            var bytes = new byte[]{10,12};
            var binary = new Binary(bytes);
            var converted = (byte[])binary;
            Assert.IsNotNull(converted);
            Assert.AreEqual(bytes, converted);
        }

        [Test]
        public void CanImplicitConvertedFromBytes(){
            var bytes = new byte[] {10, 12};
            var binary = (Binary)bytes;
            Assert.IsNotNull(binary);
            Assert.AreEqual(bytes,binary.Bytes);
        }

        [Test]
        public void CanBeCloned(){
            var binarySource = new Binary(new byte[] {10, 20}, Binary.TypeCode.UserDefined);
            var binaryDest = binarySource.Clone() as Binary;
            Assert.IsNotNull(binaryDest);
            Assert.AreEqual(binarySource.Bytes,binaryDest.Bytes);
            Assert.AreEqual(binarySource.Subtype,binaryDest.Subtype);
        }

        [Test]
        public void CanBeEnumerated()
        {
            var binary = new Binary(new byte[] { 10, 20 });

            var array = binary.ToArray();
            Assert.AreEqual(2,array.Length);
            Assert.AreEqual(10, array[0]);
            Assert.AreEqual(20, array[1]);
        }
    }
}
