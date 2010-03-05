using System;

namespace MongoDB.Driver
{
    public class CodeWScope : Code
    {
        public Document Scope {get;set;}
        
        public CodeWScope(){}

        public CodeWScope(String code):this(code, new Document()){}
        
        public CodeWScope(String code, Document scope){
            this.Value = code;
            this.Scope = scope;
        }
    }
}
