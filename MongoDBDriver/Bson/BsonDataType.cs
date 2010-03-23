namespace MongoDB.Driver.Bson
{
    /// <summary>
    /// 
    /// </summary>
    public enum BsonDataType
    {
        /// <summary>
        /// 
        /// </summary>
        Number = 1,
        /// <summary>
        /// int32 
        /// cstring The int32 is the # bytes following (# of bytes in string + 1 for terminating NULL)
        /// </summary>
        String = 2,
        /// <summary>
        /// bson object
        /// </summary>
        Obj = 3, //
        /// <summary>
        /// bson object
        /// </summary>
        Array = 4,
        /// <summary>
        /// int32 byte byte[]  
        /// The first int32 is the # of bytes following the byte subtype
        /// </summary>
        Binary = 5,
        /// <summary>
        /// VOID  
        /// Conceptually equiValent to Javascript undefined.  Deprecated.
        /// </summary>
        Undefined = 6,
        /// <summary>
        /// byte[12]     
        /// 12 byte object id.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Oid")]
        Oid = 7,
        /// <summary>
        /// byte     
        /// legal values: 0x00 -> false, 0x01 -> true
        /// </summary>
        Boolean = 8, 
        /// <summary>
        /// int64   
        /// milliseconds since epoch (e.g. new Date.getTime())
        /// </summary>
        Date = 9,
        /// <summary>
        /// VOID   
        /// Mapped to Null in programming languages which have a Null value or type.  Conceptually equivalent to Javascript null.
        /// </summary>
        Null = 10,
        /// <summary>
        /// cstring cstring   
        /// first ctring is regex expression, second cstring are regex options
        /// </summary>
        Regex = 11,
        /// <summary>
        /// int32 cstring byte[12]    
        /// Deprecated.  Please use a subobject instead
        /// The int32 is the length in bytes of the cstring.
        /// The cstring is the Namespace: full collection name.
        /// The byte array is a 12 byte object id. See note on data_oid.
        /// </summary>
        Reference = 12,
        /// <summary>
        /// Int32 cstring
        /// The int32 is the # bytes following (# of bytes in string + 1 
        /// for terminating NULL) and then the code as cstring. data_code should 
        /// be supported in BSON encoders/decoders, but has been deprecated in 
        /// favor of data_code_w_scope
        /// </summary>
        Code = 13,
        /// <summary>
        /// int32 
        /// int32 cstring bson_object  The first int32 is the total # of
        /// bytes (size of cstring + size of bson_object + 8 for the two int32s).
        /// The second int 32 is the size of the cstring (# of bytes in string + 1 for 
        /// terminating NULL). The cstring is the code. The bson_object is an object 
        /// mapping identifiers to values, representing the scope in which the code 
        /// should be evaluated.
        /// </summary>
        CodeWScope = 15,
        /// <summary>
        /// Int32
        /// </summary>
        Integer = 16,
        /// <summary>
        /// Int64 - first 4 byte are a timestamp, next 4 byte are an incremented field
        /// </summary>
        Timestamp = 17, 
        /// <summary>
        /// Int64 - 64 bit integer 
        /// </summary>
        Long     = 18,
        /// <summary>
        /// VOID - Special type that compares lower than all other types.
        /// </summary>
        MinKey = -1,
        /// <summary>
        /// VOID - Special type that compares higher than all other types.
        /// </summary>
        MaxKey = 127
    }
}
