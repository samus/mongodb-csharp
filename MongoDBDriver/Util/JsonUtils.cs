using System;
using System.Globalization;
using System.Text;


namespace MongoDB.Driver
{
    /// <summary>
    /// Lightweight routines to handle basic json serializing.
    /// </summary>
    public class JsonUtils
    {   
        public static string Serialize(Document doc){
            var json = new StringBuilder();
            json.Append("{ ");
            bool first = true;
            foreach (String key in doc.Keys) {
                if (first) {
                    first = false;
                } else {
                    json.Append(", ");
                }
                json.AppendFormat(@"""{0}"": ", key);
                SerializeType(doc[key], json);
            }
            json.Append(" }");
            return json.ToString();
        }
        
        private static void SerializeType(object value, StringBuilder json) {
            if (value == null) {
                json.Append("null");
                return;
            }
            var t = value.GetType();
            if (value is bool) {
                json.Append(((bool)value) ? "true" : "false");
            } else if (t.IsArray) {
                json.Append("[ ");
                bool first = true;
                foreach (var v in (Array)value) {
                    if (first) {
                        first = false;
                    } else {
                        json.Append(", ");
                    }
                    SerializeType(v, json);
                }
                json.Append(" ]");
            } else if (value is Document ||
                value is Oid ||
                value is Binary ||
                value is DBRef ||
                value is MongoMinKey ||
                value is MongoMaxKey ||
                value is Code ||
                value is CodeWScope ||
                value is int ||
                value is Int32 ||
                value is long ||
                value is float ||
                value is double) {
                json.Append(value);
            } else if (value is DateTime) {
                json.AppendFormat(@"""{0}""", ((DateTime)value).ToUniversalTime().ToString("o"));
            } else if (value is Guid) {
                json.Append(String.Format(@"{{ ""$uid"": ""{0}"" }}",value.ToString()));
            } else if (value is IFormattable) {
                json.Append(((IFormattable)value).ToString("G", CultureInfo.InvariantCulture));
            } else {                
                json.AppendFormat(@"""{0}""", Escape(value.ToString()));
            }
            return;
        }
        
        /// <summary>
        /// Escapes any characters that are special to javascript.
        /// </summary>
        public static string Escape(string text){
            StringBuilder sb = new StringBuilder();
            foreach(char c in text){
                switch(c){
                    case '\b':
                        sb.Append(@"\b");
                        break;
                    case '\f':
                        sb.Append(@"\f");
                        break;
                    case '\n':
                        sb.Append(@"\n");
                        break;
                    case '\r':
                        sb.Append(@"\r");
                        break;
                    case '\t':
                        sb.Append(@"\t");
                        break;
                    case '\v':
                        sb.Append(@"\v");
                        break;
                    case '\'':
                        sb.Append(@"\'");
                        break;
                    case '"':
                        sb.Append("\\\"");
                        break;
                    case '\\':
                        sb.Append(@"\\");
                        break;
                    default:
                        if(c <= '\u001f'){
                            sb.Append("\\u");
                            sb.Append(((int)c).ToString("x4"));
                        }else{
                            sb.Append(c);
                        }
                        break;
                }
            }
            return sb.ToString();
        }
    }
}
