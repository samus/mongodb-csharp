using MongoDB.Util;

namespace MongoDB
{
    /// <summary>
    /// </summary>
    public class Code
    {
        /// <summary>
        ///   Initializes a new instance of the <see cref = "Code" /> class.
        /// </summary>
        public Code(){
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref = "Code" /> class.
        /// </summary>
        /// <param name = "value">The value.</param>
        public Code(string value){
            Value = value;
        }

        /// <summary>
        ///   Gets or sets the value.
        /// </summary>
        /// <value>The value.</value>
        public string Value { get; set; }

        /// <summary>
        ///   Determines whether the specified <see cref = "System.Object" /> is equal to this instance.
        /// </summary>
        /// <param name = "obj">The <see cref = "System.Object" /> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref = "System.Object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref = "T:System.NullReferenceException">
        ///   The <paramref name = "obj" /> parameter is null.
        /// </exception>
        public override bool Equals(object obj){
            var other = obj as Code;

            if(other == null)
                return false;

            return Value == other.Value;
        }

        /// <summary>
        ///   Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        ///   A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode(){
            return (Value != null ? Value.GetHashCode() : 0);
        }

        /// <summary>
        ///   Returns a <see cref = "System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        ///   A <see cref = "System.String" /> that represents this instance.
        /// </returns>
        public override string ToString(){
            return string.Format(@"{{ ""$code"": ""{0}"" }}", JsonFormatter.Escape(Value));
        }
    }
}