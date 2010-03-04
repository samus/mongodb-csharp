using System;
using MongoDB.Driver;

namespace MongoDB.Linq {

    /// <summary>
    /// This class is a construct for writing strongly typed query expressions for Document fields.
    /// It is not meant to be used outside of expressions, since most functions and operators return
    /// fake data and are only used to extract parameter information from expressions.
    /// </summary>
    public class MongoDocumentQuery {
        private readonly string key;

        public MongoDocumentQuery(Document document, string key) {
            this.key = key;
        }

        public string Key { get { return key; } }

        public bool In<T>(params T[] values) {
            return false;
        }
        public bool NotIn<T>(params T[] values) {
            return false;
        }

        public static bool operator ==(MongoDocumentQuery a, string b) { return false; }
        public static bool operator !=(MongoDocumentQuery a, string b) { return false; }
        public static bool operator ==(string a, MongoDocumentQuery b) { return false; }
        public static bool operator !=(string a, MongoDocumentQuery b) { return false; }

        public static bool operator >(MongoDocumentQuery a, int b) { return false; }
        public static bool operator >=(MongoDocumentQuery a, int b) { return false; }
        public static bool operator <(MongoDocumentQuery a, int b) { return false; }
        public static bool operator <=(MongoDocumentQuery a, int b) { return false; }
        public static bool operator ==(MongoDocumentQuery a, int b) { return false; }
        public static bool operator !=(MongoDocumentQuery a, int b) { return false; }
        public static bool operator >(int a, MongoDocumentQuery b) { return false; }
        public static bool operator >=(int a, MongoDocumentQuery b) { return false; }
        public static bool operator <(int a, MongoDocumentQuery b) { return false; }
        public static bool operator <=(int a, MongoDocumentQuery b) { return false; }
        public static bool operator ==(int a, MongoDocumentQuery b) { return false; }
        public static bool operator !=(int a, MongoDocumentQuery b) { return false; }

        public static bool operator >(MongoDocumentQuery a, double b) { return false; }
        public static bool operator >=(MongoDocumentQuery a, double b) { return false; }
        public static bool operator <(MongoDocumentQuery a, double b) { return false; }
        public static bool operator <=(MongoDocumentQuery a, double b) { return false; }
        public static bool operator ==(MongoDocumentQuery a, double b) { return false; }
        public static bool operator !=(MongoDocumentQuery a, double b) { return false; }
        public static bool operator >(double a, MongoDocumentQuery b) { return false; }
        public static bool operator >=(double a, MongoDocumentQuery b) { return false; }
        public static bool operator <(double a, MongoDocumentQuery b) { return false; }
        public static bool operator <=(double a, MongoDocumentQuery b) { return false; }
        public static bool operator ==(double a, MongoDocumentQuery b) { return false; }
        public static bool operator !=(double a, MongoDocumentQuery b) { return false; }

        public static bool operator >(MongoDocumentQuery a, DateTime b) { return false; }
        public static bool operator >=(MongoDocumentQuery a, DateTime b) { return false; }
        public static bool operator <(MongoDocumentQuery a, DateTime b) { return false; }
        public static bool operator <=(MongoDocumentQuery a, DateTime b) { return false; }
        public static bool operator ==(MongoDocumentQuery a, DateTime b) { return false; }
        public static bool operator !=(MongoDocumentQuery a, DateTime b) { return false; }
        public static bool operator >(DateTime a, MongoDocumentQuery b) { return false; }
        public static bool operator >=(DateTime a, MongoDocumentQuery b) { return false; }
        public static bool operator <(DateTime a, MongoDocumentQuery b) { return false; }
        public static bool operator <=(DateTime a, MongoDocumentQuery b) { return false; }
        public static bool operator ==(DateTime a, MongoDocumentQuery b) { return false; }
        public static bool operator !=(DateTime a, MongoDocumentQuery b) { return false; }

        public bool Equals(MongoDocumentQuery other)
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
            if(obj.GetType() != typeof(MongoDocumentQuery))
                return false;
            return Equals((MongoDocumentQuery)obj);
        }

        public override int GetHashCode()
        {
            return (key != null ? key.GetHashCode() : 0);
        }
    }
}
