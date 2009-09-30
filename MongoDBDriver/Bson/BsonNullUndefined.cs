using System;

namespace MongoDB.Driver.Bson
{
    /// <summary>
    /// Base type for empty and null types.
    /// </summary>
    public abstract class BsonEmpty : BsonType
    {
        
        public BsonEmpty(){}
        
        
        public int Size {
            get {return 0;}
        }
        
        public abstract byte TypeNum{
            get;
        }
        

        
        public int Read(BsonReader reader){
            return 0;
        }
        
        public void Write(BsonWriter writer){
            return;
        }
        
        public object ToNative(){
            return null;
        }
    }
    
    public class BsonUndefined : BsonEmpty{
        public override byte TypeNum{
            get{
                return (byte)BsonDataType.Undefined;
            }
        }
    }

    public class BsonNull : BsonEmpty{
        public override byte TypeNum{
            get{
                return (byte)BsonDataType.Null;
            }
        }
    }   
}
