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
