namespace MongoDB.Driver
{
    /// <summary>
    /// Class representing the MinKey Bson type.  It will always compare lower than any other type.
    /// </summary>
    public class MongoMinKey
    {
        static MongoMinKey val = new MongoMinKey();        
        public static MongoMinKey Value {
            get{
                return val;
            }
        }
        protected MongoMinKey (){}
        
        public override string ToString (){
            return "{ \"$minkey\": 1 }";
        }

    }
        
    /// <summary>
    /// Class representing the MaxKey Bson type.  It will always compare higher than any other type.
    /// </summary>
    public class MongoMaxKey
    {
        static MongoMaxKey val = new MongoMaxKey();        
        public static MongoMaxKey Value {
            get{
                return val;
            }
        }
        protected MongoMaxKey (){}
        
        public override string ToString (){
            return "{ \"$maxkey\": 1 }";
        }        
    }    
}
