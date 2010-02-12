namespace MongoDB.Driver
{
    public class MongoRegex
    {   
        
        private string expression;
        public string Expression {
            get {return expression;}
            set {expression = value;}
        }       
        
        private string options;
        public string Options {
            get {return options;}
            set {options = value;}
        }
        
        public MongoRegex(){}
        
        public MongoRegex(string expression):this(expression,string.Empty){}

        public MongoRegex(string expression, string options){
            this.Expression = expression;
            this.Options = options;
        }
        
        
    }
}
