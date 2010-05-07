using System;

namespace MongoDB
{
    /// <summary>
    /// </summary>
    public class Binary
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