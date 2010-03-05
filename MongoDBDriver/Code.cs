using System;

using MongoDB.Driver.Util;

namespace MongoDB.Driver
{
    public class Code
    {
        public string Value {get; set;}
        
        public Code(){}
        
        public Code(string value){
            this.Value = value;
        }
        
        public override string ToString() {
            return string.Format(@"{{ $code : ""{0}"" }}", JsonFormatter.Escape(Value));
        }        
    }
}
