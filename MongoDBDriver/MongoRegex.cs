namespace MongoDB.Driver
{
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
        
        public MongoRegex(){}
        
        public MongoRegex(string expression):this(expression,string.Empty){}

        public MongoRegex(string expression, string options){
            this.Expression = expression;
            this.Options = options;
        }
        
        public override string ToString ()
        {
            return string.Format("{0}{1}", Expression, Options);
        }
    }
}
