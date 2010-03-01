namespace MongoDB.Driver
{
    public class Code
    {
        private string value;       
        public string Value {
            get {return this.value;}
            set {
                this.value = value; 
            }
        }
        
        public Code(){}
        
        public Code(string value){
            this.Value = value;
        }
    }
}
