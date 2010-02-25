namespace MongoDB.Driver.Bson
{
    public enum BsonDataType:sbyte
    {
        Number = 1,
        String = 2, //int32 cstring The int32 is the # bytes following (# of bytes in string + 1 for terminating NULL)
        Obj = 3, //bson object      
        Array = 4, //bson_object
        Binary = 5,//int32 byte byte[]  The first int32 is the # of bytes following the byte subtype
        Undefined = 6, //VOID   Conceptually equiValent to Javascript undefined.  Deprecated.
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Oid")]
        Oid = 7, //byte[12]     12 byte object id.
        Boolean = 8, //byte     legal values: 0x00 -> false, 0x01 -> true
        Date = 9, //int64   value: milliseconds since epoch (e.g. new Date.getTime())
        Null = 10, //VOID   Mapped to Null in programming languages which have a Null value or type.  Conceptually equivalent to Javascript null.
        Regex = 11, //cstring cstring   first ctring is regex expression, second cstring are regex options
        Reference = 12, //int32 cstring byte[12]    Deprecated.  Please use a subobject instead
        //The int32 is the length in bytes of the cstring.
        //The cstring is the Namespace: full collection name.
        //The byte array is a 12 byte object id. See note on data_oid.
        Code = 13,  //int32 cstring     The int32 is the # bytes following (# of bytes in string + 1 
        //for terminating NULL) and then the code as cstring. data_code should 
        //be supported in BSON encoders/decoders, but has been deprecated in 
        //favor of data_code_w_scope
        CodeWScope = 15, //int32 int32 cstring bson_object  The first int32 is the total # of
        //bytes (size of cstring + size of bson_object + 8 for the two int32s).
        //The second int 32 is the size of the cstring (# of bytes in string + 1 for 
        //terminating NULL). The cstring is the code. The bson_object is an object 
        //mapping identifiers to values, representing the scope in which the code 
        //should be evaluated.
        Integer = 16, //int32
        Timestamp = 17, //int64     first 4 are a timestamp, next 4 are an incremented field
        Long     = 18,  //int64      64 bit integer 
        MinKey = -1, //VOID     //Special type that compares lower than all other types.
        MaxKey = 127 //VOID     //Special type that compares higher than all other types.
    }
}