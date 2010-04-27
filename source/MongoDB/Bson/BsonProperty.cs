namespace MongoDB.Bson
{
    /// <summary>
    /// 
    /// </summary>
    public class BsonProperty
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BsonProperty"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        public BsonProperty(string name){
            Name = name;
        }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>The value.</value>
        public object Value { get; set; }
    }
}