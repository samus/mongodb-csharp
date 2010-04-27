namespace MongoDB
{
    /// <summary>
    /// Class representing the MaxKey Bson type.  It will always compare higher than any other type.
    /// </summary>
    public class MongoMaxKey
    {
        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <value>The value.</value>
        public static MongoMaxKey Value { get; private set; }

        /// <summary>
        /// Initializes the <see cref="MongoMaxKey"/> class.
        /// </summary>
        static MongoMaxKey(){
            Value = new MongoMaxKey();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MongoMaxKey"/> class.
        /// </summary>
        protected MongoMaxKey (){}

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString (){
            return "{ \"$maxkey\": 1 }";
        }      
    }
}