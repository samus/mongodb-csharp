using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace MongoDB
{
    /// <summary>
    /// </summary>
    [Serializable]
    public class Binary : IEquatable<Binary>, ICloneable, IEnumerable<byte>
    {
        /// <summary>
        ///   Initializes a new instance of the <see cref = "Binary" /> class.
        /// </summary>
        public Binary()
        {
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref = "Binary" /> class.
        /// </summary>
        /// <param name = "bytes">The value.</param>
        public Binary(byte[] bytes)
        {
            Bytes = bytes;
            Subtype = BinarySubtype.General;
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref = "Binary" /> class.
        /// </summary>
        /// <param name = "bytes">The bytes.</param>
        /// <param name = "subtype">The subtype.</param>
        public Binary(byte[] bytes, BinarySubtype subtype)
        {
            Bytes = bytes;
            Subtype = subtype;
        }

        /// <summary>
        ///   Gets or sets the bytes.
        /// </summary>
        /// <value>The bytes.</value>
        public byte[] Bytes { get; set; }

        /// <summary>
        ///   Gets or sets the subtype.
        /// </summary>
        /// <value>The subtype.</value>
        public BinarySubtype Subtype { get; set; }

        /// <summary>
        ///   Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        ///   A new object that is a copy of this instance.
        /// </returns>
        public object Clone()
        {
            return new Binary(Bytes) {Subtype = Subtype};
        }

        /// <summary>
        ///   Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        ///   An <see cref = "T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        ///   Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        ///   A <see cref = "T:System.Collections.Generic.IEnumerator`1" /> that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<byte> GetEnumerator()
        {
            if(Bytes == null)
                yield break;

            foreach(var b in Bytes)
                yield return b;
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="MongoDB.Binary"/> to <see cref="System.Byte"/>.
        /// </summary>
        /// <param name="binary">The binary.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator byte[](Binary binary)
        {
            if(binary == null)
                throw new ArgumentNullException("binary");

            return binary.Bytes;
        }

        /// <summary>
        ///   Performs an implicit conversion from <see cref = "System.Byte" /> to <see cref = "MongoDB.Binary" />.
        /// </summary>
        /// <param name = "bytes">The bytes.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator Binary(byte[] bytes)
        {
            if(bytes == null)
                throw new ArgumentNullException("bytes");

            return new Binary(bytes);
        }

        /// <summary>
        ///   Implements the operator ==.
        /// </summary>
        /// <param name = "left">The left.</param>
        /// <param name = "right">The right.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator ==(Binary left, Binary right)
        {
            return Equals(left, right);
        }

        /// <summary>
        ///   Implements the operator !=.
        /// </summary>
        /// <param name = "left">The left.</param>
        /// <param name = "right">The right.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator !=(Binary left, Binary right)
        {
            return !Equals(left, right);
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        public bool Equals(Binary other)
        {
            if(ReferenceEquals(null, other))
                return false;
            if(ReferenceEquals(this, other))
                return true;
            return  other.Bytes.SequenceEqual(Bytes) && Equals(other.Subtype, Subtype);
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object"/> to compare with this instance.</param>
        /// <returns>
        /// 	<c>true</c> if the specified <see cref="System.Object"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="T:System.NullReferenceException">
        /// The <paramref name="obj"/> parameter is null.
        /// </exception>
        public override bool Equals(object obj)
        {
            if(ReferenceEquals(null, obj))
                return false;
            if(ReferenceEquals(this, obj))
                return true;
            return obj.GetType() == typeof(Binary) && Equals((Binary)obj);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            unchecked
            {
                return ((Bytes != null ? Bytes.GetHashCode() : 0)*397) ^ Subtype.GetHashCode();
            }
        }

        /// <summary>
        ///   Returns a <see cref = "System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        ///   A <see cref = "System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return String.Format(@"{{ ""$binary"": ""{0}"", ""$type"" : {1} }}",
                Convert.ToBase64String(Bytes),
                (int)Subtype);
        }
    }
}