using System;
using MongoDB.Driver;

namespace MongoDB.Linq {

    /// <summary>
    /// This class is a construct for writing strongly typed query expressions for Document fields.
    /// It is not meant to be used outside of expressions, since most functions and operators return
    /// fake data and are only used to extract parameter information from expressions.
    /// </summary>
    public class MongoDocumentQuery {
        private readonly Document document;
        private readonly string key;

        public MongoDocumentQuery(Document document, string key) {
            this.document = document;
            this.key = key;
        }

        public string Key { get { return key; } }

        public bool In<T>(params T[] values) {
            return false;
        }
        public bool NotIn<T>(params T[] values) {
            return false;
        }

        #region operator overloads for cast-less expressions
        #region string overloads
        public static bool operator ==(MongoDocumentQuery a, string b) { return false; }
        public static bool operator !=(MongoDocumentQuery a, string b) { return false; }
        public static bool operator ==(string a, MongoDocumentQuery b) { return false; }
        public static bool operator !=(string a, MongoDocumentQuery b) { return false; }
        #endregion

        #region int overloads
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
        #endregion

        #region double overloads
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
        #endregion

        #region DateTime overloads
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
        #endregion
        #endregion
    }
}
