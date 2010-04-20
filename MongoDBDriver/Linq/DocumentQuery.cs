using System;
using MongoDB.Driver;

namespace MongoDB.Driver.Linq {

    /// <summary>
    /// This class is a construct for writing strongly typed query expressions for Document fields.
    /// It is not meant to be used outside of expressions, since most functions and operators return
    /// fake data and are only used to extract parameter information from expressions.
    /// </summary>
    public class DocumentQuery {
        private readonly string key;

        public DocumentQuery(Document document, string key) {
            this.key = key;
        }

        public string Key { get { return key; } }

        public static bool operator ==(DocumentQuery a, string b) { return false; }
        public static bool operator !=(DocumentQuery a, string b) { return false; }
        public static bool operator ==(string a, DocumentQuery b) { return false; }
        public static bool operator !=(string a, DocumentQuery b) { return false; }

        public static bool operator >(DocumentQuery a, int b) { return false; }
        public static bool operator >=(DocumentQuery a, int b) { return false; }
        public static bool operator <(DocumentQuery a, int b) { return false; }
        public static bool operator <=(DocumentQuery a, int b) { return false; }
        public static bool operator ==(DocumentQuery a, int b) { return false; }
        public static bool operator !=(DocumentQuery a, int b) { return false; }
        public static bool operator >(int a, DocumentQuery b) { return false; }
        public static bool operator >=(int a, DocumentQuery b) { return false; }
        public static bool operator <(int a, DocumentQuery b) { return false; }
        public static bool operator <=(int a, DocumentQuery b) { return false; }
        public static bool operator ==(int a, DocumentQuery b) { return false; }
        public static bool operator !=(int a, DocumentQuery b) { return false; }

        public static bool operator >(DocumentQuery a, double b) { return false; }
        public static bool operator >=(DocumentQuery a, double b) { return false; }
        public static bool operator <(DocumentQuery a, double b) { return false; }
        public static bool operator <=(DocumentQuery a, double b) { return false; }
        public static bool operator ==(DocumentQuery a, double b) { return false; }
        public static bool operator !=(DocumentQuery a, double b) { return false; }
        public static bool operator >(double a, DocumentQuery b) { return false; }
        public static bool operator >=(double a, DocumentQuery b) { return false; }
        public static bool operator <(double a, DocumentQuery b) { return false; }
        public static bool operator <=(double a, DocumentQuery b) { return false; }
        public static bool operator ==(double a, DocumentQuery b) { return false; }
        public static bool operator !=(double a, DocumentQuery b) { return false; }

        public static bool operator >(DocumentQuery a, DateTime b) { return false; }
        public static bool operator >=(DocumentQuery a, DateTime b) { return false; }
        public static bool operator <(DocumentQuery a, DateTime b) { return false; }
        public static bool operator <=(DocumentQuery a, DateTime b) { return false; }
        public static bool operator ==(DocumentQuery a, DateTime b) { return false; }
        public static bool operator !=(DocumentQuery a, DateTime b) { return false; }
        public static bool operator >(DateTime a, DocumentQuery b) { return false; }
        public static bool operator >=(DateTime a, DocumentQuery b) { return false; }
        public static bool operator <(DateTime a, DocumentQuery b) { return false; }
        public static bool operator <=(DateTime a, DocumentQuery b) { return false; }
        public static bool operator ==(DateTime a, DocumentQuery b) { return false; }
        public static bool operator !=(DateTime a, DocumentQuery b) { return false; }

        public bool Equals(DocumentQuery other)
        {
            if(ReferenceEquals(null, other))
                return false;
            if(ReferenceEquals(this, other))
                return true;
            return Equals(other.key, key);
        }

        public override bool Equals(object obj)
        {
            if(ReferenceEquals(null, obj))
                return false;
            if(ReferenceEquals(this, obj))
                return true;
            if(obj.GetType() != typeof(DocumentQuery))
                return false;
            return Equals((DocumentQuery)obj);
        }

        public override int GetHashCode()
        {
            return (key != null ? key.GetHashCode() : 0);
        }
    }
}
