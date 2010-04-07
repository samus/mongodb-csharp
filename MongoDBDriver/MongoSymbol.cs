using System;

namespace MongoDB.Driver
{

    /// <summary>
    /// Type to hold an interned string that maps to the bson symbol type.
    /// </summary>
    public struct MongoSymbol : IEquatable<MongoSymbol>, IEquatable<String>, IComparable<MongoSymbol>, IComparable<String>
    {
        public string Value{get; private set;}
        
            
        public MongoSymbol(string val){
            if(string.IsNullOrEmpty(val)) throw new ArgumentNullException("Symbol Value");
            this.Value = String.Intern(val);
            
        }
        
        public override string ToString (){
            return this.Value;
        }

        public override bool Equals (object obj){
            return Value.Equals(obj);
        }

        public bool Equals (MongoSymbol other){
            return Value.Equals(other.Value);
        }
        
        public bool Equals (string other){
            return Value.Equals(other);
        }
        
        public int CompareTo (MongoSymbol other){
            return Value.CompareTo(other.Value);
        }
        
        public int CompareTo (string other){
            return Value.CompareTo(other);
        }
        
        public static bool operator ==(MongoSymbol a, MongoSymbol b){
            return SymbolEqual(a.Value, b.Value);
        }
        
        public static bool operator ==(MongoSymbol a, string b){
            return SymbolEqual(a.Value, b);
        }
        
        public static bool operator ==(string a, MongoSymbol b){
            return SymbolEqual(a, b.Value);
        }
        public static bool operator !=(MongoSymbol a, MongoSymbol b){
            return !(a == b);
        }
        
        public static bool operator !=(MongoSymbol a, String b){
            return !(a == b);
        }
        
        public static bool operator !=(string a, MongoSymbol b){
            return !(a == b);
        }
        
        public static implicit operator string(MongoSymbol s){
            return s.Value;
        }
        
        public static implicit operator MongoSymbol(string s){
            return new MongoSymbol(s);
        }
        
        private static MongoSymbol _empty;
        public static MongoSymbol Empty{
            get{
                return _empty;
            }
        }
        
        public static bool IsEmpty(MongoSymbol s){
            return s == _empty;
        }
            
        private static bool SymbolEqual(string a, string b){
            return a == b;
        }
    }
}
