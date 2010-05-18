using System;
using System.Collections;
using System.Collections.Generic;

namespace MongoDB
{
    /// <summary>
    /// </summary>
    public class Binary : IEquatable<Binary>, ICloneable, IEnumerable<byte>
    {
        /// <summary>
        /// </summary>
        public enum TypeCode : byte
        {
            /// <summary>
            /// </summary>
            Unknown = 0,
            /// <summary>
            /// </summary>
            General = 2,
            // Uuid = 3 is now replaced by Guid
            /// <summary>
            /// </summary>
            Md5 = 5,
            /// <summary>
            /// </summary>
            UserDefined = 80
        }

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
            Subtype = TypeCode.General;
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref = "Binary" /> class.
        /// </summary>
        /// <param name = "bytes">The bytes.</param>
        /// <param name = "subtype">The subtype.</param>
        public Binary(byte[] bytes, TypeCode subtype)
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
        public TypeCode Subtype { get; set; }

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
        ///   Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns>
        ///   true if the current object is equal to the <paramref name = "other" /> parameter; otherwise, false.
        /// </returns>
        /// <param name = "other">An object to compare with this object.
        /// </param>
        public bool Equals(Binary other)
        {
            if(ReferenceEquals(null, other))
                return false;
            return ReferenceEquals(this, other) || Equals(other.Bytes, Bytes);
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
        ///   Determines whether the specified <see cref = "T:System.Object" /> is equal to the current <see cref = "T:System.Object" />.
        /// </summary>
        /// <returns>
        ///   true if the specified <see cref = "T:System.Object" /> is equal to the current <see cref = "T:System.Object" />; otherwise, false.
        /// </returns>
        /// <param name = "obj">The <see cref = "T:System.Object" /> to compare with the current <see cref = "T:System.Object" />. 
        /// </param>
        /// <exception cref = "T:System.NullReferenceException">The <paramref name = "obj" /> parameter is null.
        /// </exception>
        /// <filterpriority>2</filterpriority>
        public override bool Equals(object obj)
        {
            if(ReferenceEquals(null, obj))
                return false;
            if(ReferenceEquals(this, obj))
                return true;
            return obj.GetType() == typeof(Binary) && Equals((Binary)obj);
        }

        /// <summary>
        ///   Serves as a hash function for a particular type.
        /// </summary>
        /// <returns>
        ///   A hash code for the current <see cref = "T:System.Object" />.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override int GetHashCode()
        {
            return (Bytes != null ? Bytes.GetHashCode() : 0);
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