namespace MongoDB.Driver
{
    /// <summary>
    /// 
    /// </summary>
    public class MongoRegex
    {   
        /// <summary>
        /// A valid regex string including the enclosing / characters.
        /// </summary>
        public string Expression {get; set;}
        
        /// <summary>
        /// A string that may contain only the characters 'g', 'i', and 'm'. 
        /// Because the JS and TenGen representations support a limited range of options, 
        /// any nonconforming options will be dropped when converting to this representation
        /// </summary>        
        public string Options {get;set;}

        /// <summary>
        /// Initializes a new instance of the <see cref="MongoRegex"/> class.
        /// </summary>
        public MongoRegex(){}

        /// <summary>
        /// Initializes a new instance of the <see cref="MongoRegex"/> class.
        /// </summary>
        /// <param name="expression">The expression.</param>
        public MongoRegex(string expression):this(expression,string.Empty){}

        /// <summary>
        /// Initializes a new instance of the <see cref="MongoRegex"/> class.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <param name="options">The options.</param>
        public MongoRegex(string expression, string options){
            this.Expression = expression;
            this.Options = options;
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString (){
            return string.Format("{0}{1}", Expression, Options);
        }
    }
}
