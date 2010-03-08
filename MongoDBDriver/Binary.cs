using System;

namespace MongoDB.Driver
{
    public class Binary{
        public enum TypeCode:byte{
            Unknown = 0,
            General = 2,
            // Uuid is now replaced by Guid
            //Uuid = 3,
            Md5 = 5,
            UserDefined = 80
        }

        public byte[] Bytes{get;set;}
 
        public TypeCode Subtype{get;set;}

        public Binary() { }

        public Binary(byte[] value){
            Bytes = value;
            Subtype = TypeCode.General;         
        }

        public override string ToString (){
            return String.Format(@"{{ ""$binary"": ""{0}"", ""$type"" : {1} }}",
                        Convert.ToBase64String(Bytes), (int)Subtype);
        }
    }
}
