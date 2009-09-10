using System;

namespace MongoDB.Driver.Bson
{   
    
    public class BsonRegex:BsonType
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
        
        
        public BsonRegex(){}
        
        public BsonRegex(string expression):this(expression, string.Empty){}
        
        public BsonRegex(string expression, string options){
            this.Expression = expression;
            this.Options = options;
        }
        
        public byte TypeNum {
            get{
                return (byte)BsonDataType.Regex;
            }
        }
        
        public int Size {
            get {
                int ret = 0;
                if(this.Expression != null) {
                    ret += this.Expression.Length;
                }
                ret += 1; //terminator
                if(this.Options != null) {
                    ret += this.Options.Length;
                }
                ret += 1; //terminator
                return ret;
            }
        }       
        public void Write(BsonWriter writer){
            writer.Write(this.Expression);
            writer.Write(this.Options);
        }
        
        public int Read(BsonReader reader){
            int len = 0;
            this.Expression = reader.ReadString();
            len += this.Expression.Length + 1;
            
            this.Options = reader.ReadString();
            len += this.Options.Length + 1;         
            
            return len;
        }
        
        public override string ToString (){
            return string.Format("[BsonRegex: Expression={0}, Options={1}, TypeNum={2}, Size={3}]", Expression, Options, TypeNum, Size);
        }
        
        public object ToNative(){
            MongoRegex mr = new MongoRegex();
            mr.Expression = this.Expression;
            mr.Options = this.Options;
            return mr;
        }
    }
}
