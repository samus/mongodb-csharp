using MongoDB.Driver.Util;

namespace MongoDB.Driver
{
    /// <summary>
    /// 
    /// </summary>
    public class Code
    {
        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>The value.</value>
        public string Value {get; set;}

        /// <summary>
        /// Initializes a new instance of the <see cref="Code"/> class.
        /// </summary>
        public Code(){}

        /// <summary>
        /// Initializes a new instance of the <see cref="Code"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        public Code(string value){
            this.Value = value;
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object"/> to compare with this instance.</param>
        /// <returns>
        /// 	<c>true</c> if the specified <see cref="System.Object"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="T:System.NullReferenceException">
        /// The <paramref name="obj"/> parameter is null.
        /// </exception>
        public override bool Equals(object obj)
        {
            Code other = obj as Code;
            if (other == null)
                return false;

            return Value == other.Value;
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString() {
            return string.Format(@"{{ ""$code"": ""{0}"" }}", JsonFormatter.Escape(Value));
        }        
    }
}
