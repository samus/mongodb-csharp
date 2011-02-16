using System;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using MongoDB.Bson;
using MongoDB.Util;

namespace MongoDB
{
    /// <summary>
    ///   Oid is an immutable object that represents a Mongo ObjectId.
    /// </summary>
    [Serializable]
    public sealed class Oid : IEquatable<Oid>, IComparable<Oid>, IFormattable, IXmlSerializable
    {
        private static readonly OidGenerator OidGenerator = new OidGenerator();
        private byte[] _bytes;

        /// <summary>
        /// Initializes a new instance of the <see cref="Oid"/> class.
        /// </summary>
        /// <remarks>
        /// Needed for some serializers.
        /// </remarks>
        private Oid()
        {
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref = "Oid" /> class.
        /// </summary>
        /// <param name = "value">The value.</param>
        public Oid(string value)
        {
            if(value == null)
                throw new ArgumentNullException("value");

            ParseBytes(value);
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref = "Oid" /> class.
        /// </summary>
        /// <param name = "value">The value.</param>
        public Oid(byte[] value)
        {
            if(value == null)
                throw new ArgumentNullException("value");

            _bytes = new byte[12];
            Array.Copy(value, _bytes, 12);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Oid"/> class.
        /// </summary>
        /// <param name="oid">The oid.</param>
        public Oid(Oid oid)
        {
            if(oid == null)
                throw new ArgumentNullException("oid");

            _bytes = oid._bytes;
        }

        /// <summary>
        ///   Gets the created.
        /// </summary>
        /// <value>The created.</value>
        public DateTime Created
        {
            get
            {
                var time = new byte[4];
                Array.Copy(_bytes, time, 4);
                Array.Reverse(time);
                var seconds = BitConverter.ToInt32(time, 0);
                return BsonInfo.Epoch.AddSeconds(seconds);
            }
        }

        /// <summary>
        ///   Compares the current object with another object of the same type.
        /// </summary>
        /// <param name = "other">An object to compare with this object.</param>
        /// <returns>
        ///   A 32-bit signed integer that indicates the relative order of the objects being compared. The return value has the following meanings:
        ///   Value
        ///   Meaning
        ///   Less than zero
        ///   This object is less than the <paramref name = "other" /> parameter.
        ///   Zero
        ///   This object is equal to <paramref name = "other" />.
        ///   Greater than zero
        ///   This object is greater than <paramref name = "other" />.
        /// </returns>
        public int CompareTo(Oid other)
        {
            if(ReferenceEquals(other, null))
                return 1;
            var otherBytes = other._bytes;
            for(var x = 0; x < _bytes.Length; x++)
                if(_bytes[x] < otherBytes[x])
                    return -1;
                else if(_bytes[x] > otherBytes[x])
                    return 1;
            return 0;
        }

        /// <summary>
        ///   Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name = "other">An object to compare with this object.</param>
        /// <returns>
        ///   true if the current object is equal to the <paramref name = "other" /> parameter; otherwise, false.
        /// </returns>
        public bool Equals(Oid other)
        {
            return CompareTo(other) == 0;
        }

        /// <summary>
        ///   Determines whether the specified <see cref = "System.Object" /> is equal to this instance.
        /// </summary>
        /// <param name = "obj">The <see cref = "System.Object" /> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref = "System.Object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if(obj is Oid)
                return CompareTo((Oid)obj) == 0;
            return false;
        }

        /// <summary>
        ///   Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        ///   A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        /// <summary>
        ///   Returns a <see cref = "System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        ///   A <see cref = "System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return BitConverter.ToString(_bytes).Replace("-", "").ToLower();
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        /// <remarks>
        /// J = Returns Javascript string
        /// </remarks>
        public string ToString(string format)
        {
            if(string.IsNullOrEmpty(format))
                return ToString();
            
            if(format == "J")
                return String.Format("\"{0}\"", ToString());
            
            throw new ArgumentException("Invalid format string","format");
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="formatProvider">The format provider.</param>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        /// <remarks>
        /// J = Returns Javascript string
        /// </remarks>
        public string ToString(string format, IFormatProvider formatProvider)
        {
            return ToString(format);
        }

        /// <summary>
        ///   Converts the Oid to a byte array.
        /// </summary>
        public byte[] ToByteArray()
        {
            var ret = new byte[12];
            Array.Copy(_bytes, ret, 12);
            return ret;
        }

        /// <summary>
        ///   Generates an Oid using OidGenerator.
        /// </summary>
        /// <returns>
        ///   A <see cref = "Oid" />
        /// </returns>
        public static Oid NewOid()
        {
            return OidGenerator.Generate();
        }

        /// <summary>
        ///   Implements the operator ==.
        /// </summary>
        /// <param name = "a">A.</param>
        /// <param name = "b">The b.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator ==(Oid a, Oid b){
            if (ReferenceEquals(a, b)){
                return true;
            }
            if((Object)a == null || (Object)b == null){
                return false;
            }
            
            return a.Equals(b);
        }

        /// <summary>
        ///   Implements the operator !=.
        /// </summary>
        /// <param name = "a">A.</param>
        /// <param name = "b">The b.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator !=(Oid a, Oid b)
        {
            return !(a == b);
        }

        /// <summary>
        ///   Implements the operator &gt;.
        /// </summary>
        /// <param name = "a">A.</param>
        /// <param name = "b">The b.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator >(Oid a, Oid b)
        {
            return a.CompareTo(b) > 0;
        }

        /// <summary>
        ///   Implements the operator &lt;.
        /// </summary>
        /// <param name = "a">A.</param>
        /// <param name = "b">The b.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator <(Oid a, Oid b)
        {
            return a.CompareTo(b) < 0;
        }

        /// <summary>
        ///   Validates the hex.
        /// </summary>
        /// <param name = "value">The value.</param>
        private void ValidateHex(string value)
        {
            if(value == null || value.Length != 24)
                throw new ArgumentException("Oid strings should be 24 characters");

            var notHexChars = new Regex(@"[^A-Fa-f0-9]", RegexOptions.None);
            if(notHexChars.IsMatch(value))
                throw new ArgumentOutOfRangeException("value", "Value contains invalid characters");
        }

        /// <summary>
        ///   Decodes the hex.
        /// </summary>
        /// <param name = "value">The value.</param>
        /// <returns></returns>
        private static byte[] DecodeHex(string value)
        {
            var numberChars = value.Length;

            var bytes = new byte[numberChars/2];
            for(var i = 0; i < numberChars; i += 2)
                try
                {
                    bytes[i/2] = Convert.ToByte(value.Substring(i, 2), 16);
                }
                catch
                {
                    //failed to convert these 2 chars, they may contain illegal charracters
                    bytes[i/2] = 0;
                }
            return bytes;
        }

        /// <summary>
        /// Parses the bytes.
        /// </summary>
        /// <param name="value">The value.</param>
        private void ParseBytes(string value)
        {
            value = value.Replace("\"", "");
            ValidateHex(value);
            _bytes = DecodeHex(value);
        }

        /// <summary>
        /// This method is reserved and should not be used. When implementing the IXmlSerializable interface, you should return null (Nothing in Visual Basic) from this method, and instead, if specifying a custom schema is required, apply the <see cref="T:System.Xml.Serialization.XmlSchemaProviderAttribute"/> to the class.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Xml.Schema.XmlSchema"/> that describes the XML representation of the object that is produced by the <see cref="M:System.Xml.Serialization.IXmlSerializable.WriteXml(System.Xml.XmlWriter)"/> method and consumed by the <see cref="M:System.Xml.Serialization.IXmlSerializable.ReadXml(System.Xml.XmlReader)"/> method.
        /// </returns>
        XmlSchema IXmlSerializable.GetSchema()
        {
            return null;
        }

        /// <summary>
        /// Generates an object from its XML representation.
        /// </summary>
        /// <param name="reader">The <see cref="T:System.Xml.XmlReader"/> stream from which the object is deserialized.</param>
        void IXmlSerializable.ReadXml(XmlReader reader)
        {
            ParseBytes(reader.ReadElementContentAsString());
        }

        /// <summary>
        /// Converts an object into its XML representation.
        /// </summary>
        /// <param name="writer">The <see cref="T:System.Xml.XmlWriter"/> stream to which the object is serialized.</param>
        void IXmlSerializable.WriteXml(XmlWriter writer)
        {
            writer.WriteString(ToString());
        }
    }
}