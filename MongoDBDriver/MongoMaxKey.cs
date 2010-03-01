namespace MongoDB.Driver
{
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
    }
}