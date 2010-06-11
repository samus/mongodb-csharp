using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;
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
            Assert.AreEqual(BinarySubtype.Unknown, binary.Subtype);
        }

        [Test]
        public void CanCreateBinaryFromNull(){
            var binary = new Binary(null);
            Assert.IsNull(binary.Bytes);
            Assert.AreEqual(BinarySubtype.General, binary.Subtype);
        }

        [Test]
        public void CanCreateBinaryFromBytes(){
            var bytes = new byte[] { 10 };
            var binary = new Binary(bytes);
            Assert.AreEqual(bytes,binary.Bytes);
            Assert.AreEqual(BinarySubtype.General, binary.Subtype);
        }

        [Test]
        public void CanCreateBinaryFromBytesAndSubtype(){
            var bytes = new byte[] {10};
            var binary = new Binary(bytes,BinarySubtype.UserDefined);
            Assert.AreEqual(bytes, binary.Bytes);
            Assert.AreEqual(BinarySubtype.UserDefined, binary.Subtype);
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
            var binarySource = new Binary(new byte[] {10, 20}, BinarySubtype.UserDefined);
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

        [Test]
        public void CanBeBinarySerialized()
        {
            var source = new Binary(new byte[] {10, 20}, BinarySubtype.Md5);
            var formatter = new BinaryFormatter();

            var mem = new MemoryStream();
            formatter.Serialize(mem, source);
            mem.Position = 0;

            var dest = (Binary)formatter.Deserialize(mem);

            Assert.AreEqual(source, dest);
        }

        [Test]
        public void CanBeEqual()
        {
            var binary1 = new Binary(new byte[] { 10, 20 }, BinarySubtype.Md5);
            var binary2 = new Binary(new byte[] { 10, 20 }, BinarySubtype.Md5);

            Assert.AreEqual(binary1,binary2);
        }

        [Test]
        public void CanBeXmlSerialized()
        {
            var source = new Binary(new byte[] { 10, 20 }, BinarySubtype.Md5);
            var serializer = new XmlSerializer(typeof(Binary));

            var writer = new StringWriter();
            serializer.Serialize(writer, source);
            var dest = (Binary)serializer.Deserialize(new StringReader(writer.ToString()));

            Assert.AreEqual(source, dest);
        }

        [Test]
        public void CanBeXmlSerializedWhenNullBytes()
        {
            var source = new Binary(null, BinarySubtype.Md5);
            var serializer = new XmlSerializer(typeof(Binary));

            var writer = new StringWriter();
            serializer.Serialize(writer, source);
            var dest = (Binary)serializer.Deserialize(new StringReader(writer.ToString()));

            Assert.AreEqual(source, dest);
        }
    }
}
