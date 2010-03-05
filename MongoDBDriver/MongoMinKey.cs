namespace MongoDB.Driver
{
    /// <summary>
    /// Class representing the MinKey Bson type.  It will always compare lower than any other type.
    /// </summary>
    public class MongoMinKey
    {
        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>The value.</value>
        public static MongoMinKey Value { get; private set; }

        /// <summary>
        /// Initializes the <see cref="MongoMinKey"/> class.
        /// </summary>
        static MongoMinKey(){
            Value = new MongoMinKey();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MongoMinKey"/> class.
        /// </summary>
        protected MongoMinKey (){}

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString (){
            return "{ \"$minkey\" : 1 }";
        }
    }
}