using System;
using System.Collections;
using System.Globalization;
using System.Text;

namespace MongoDB.Driver.Util
{
    /// <summary>
    /// Lightweight routines to handle basic json serializing.
    /// </summary>
    public class JsonFormatter
    {
        /// <summary>
        /// Serializes the specified doc.
        /// </summary>
        /// <param name="doc">The doc.</param>
        /// <returns></returns>
        public static string Serialize(Document doc){
            var json = new StringBuilder();
            json.Append("{ ");
            var first = true;
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

        /// <summary>
        /// Serializes the type.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="json">The json.</param>
        private static void SerializeType(object value, StringBuilder json) {
            if (value == null) {
                json.Append("null");
                return;
            }
            if (value is bool) {
                json.Append(((bool)value) ? "true" : "false");
            } else if(value is Document ||
                value is Oid ||
                value is Binary ||
                value is DBRef ||
                value is MongoMinKey ||
                value is MongoMaxKey ||
                value is Code ||
                value is CodeWScope) {
                json.Append(value);                
            } else if(value is int ||
                value is long || 
                value is float || 
                value is double ) {
                // Format numbers allways culture invariant
                // Example: Without this in Germany 10.3 is outputed as 10,3
                json.Append(((IFormattable)value).ToString("G", CultureInfo.InvariantCulture));
            } else if(value is string){
                json.AppendFormat(@"""{0}""", Escape((string)value));
            } else if (value is DateTime) {
                json.AppendFormat(@"""{0}""", ((DateTime)value).ToUniversalTime().ToString("o"));
            } else if (value is Guid) {
                json.Append(String.Format(@"{{ ""$uid"": ""{0}"" }}",value));
            } else if (value is IEnumerable) {
                json.Append("[ ");
                var first = true;
                foreach (var v in (IEnumerable)value) {
                    if (first) {
                        first = false;
                    } else {
                        json.Append(", ");
                    }
                    SerializeType(v, json);
                }
                json.Append(" ]");                
            } else {
                json.AppendFormat(@"""{0}""", Escape(value.ToString()));
            }
            return;
        }
        
        /// <summary>
        /// Escapes any characters that are special to javascript.
        /// </summary>
        public static string Escape(string text){
            var builder = new StringBuilder();
            foreach(char c in text){
                switch(c){
                    case '\b':
                        builder.Append(@"\b");
                        break;
                    case '\f':
                        builder.Append(@"\f");
                        break;
                    case '\n':
                        builder.Append(@"\n");
                        break;
                    case '\r':
                        builder.Append(@"\r");
                        break;
                    case '\t':
                        builder.Append(@"\t");
                        break;
                    case '\v':
                        builder.Append(@"\v");
                        break;
                    case '\'':
                        builder.Append(@"\'");
                        break;
                    case '"':
                        builder.Append("\\\"");
                        break;
                    case '\\':
                        builder.Append(@"\\");
                        break;
                    default:
                        if(c <= '\u001f'){
                            builder.Append("\\u");
                            builder.Append(((int)c).ToString("x4"));
                        }else{
                            builder.Append(c);
                        }
                        break;
                }
            }
            return builder.ToString();
        }
    }
}
