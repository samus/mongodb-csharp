namespace MongoDB.Driver{
    /// <summary>
    /// Placeholder type for database nulls.
    /// </summary>
    public class MongoDBNull{
        static MongoDBNull val = new MongoDBNull();
        public static MongoDBNull Value {
            get {
                return (val);
            }
        }            
        protected MongoDBNull(){
            
        }
    }
}
