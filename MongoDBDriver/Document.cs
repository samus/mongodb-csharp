/*
 * User: scorder
 * Date: 7/8/2009
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace MongoDB.Driver {
    /// <summary>
    /// Description of Document.
    /// </summary>
    public class Document : System.Collections.DictionaryBase {
        private List<String> orderedKeys = new List<String>();
        public Document() {
        }

        public Object this[String key] {
            get {
                return Dictionary[key];
            }
            set {
                if (orderedKeys.Contains(key) == false) {
                    orderedKeys.Add(key);
                }
                Dictionary[key] = value;
            }
        }

        public ICollection Keys {
            get {
                return (orderedKeys);
            }
        }

        public ICollection Values {
            get {
                return (Dictionary.Values);
            }
        }

        public void Add(String key, Object value) {
            Dictionary.Add(key, value);
            //Relies on ArgumentException from above if key already exists.
            orderedKeys.Add(key);
        }

        public Document Append(String key, Object value) {
            this.Add(key, value);
            return this;
        }
		
        /// <summary>
        /// Adds an item to the Document at the specified position
        /// </summary>
        public void Insert(String key, Object value, int Position){
            Dictionary.Add(key, value);
            //Relies on ArgumentException from above if key already exists.
            orderedKeys.Insert(Position,key);
        }
        public Document Prepend(String key, Object value) {
            this.Insert(key, value,0);
            return this;
        }
		
        public Document Update(Document from) {
            if (from == null) return this;
            foreach (String key in from.Keys) {
                this[key] = from[key];
            }
            return this;
        }

        public bool Contains(String key) {
            return (orderedKeys.Contains(key));
        }

        public void Remove(String key) {
            Dictionary.Remove(key);
            orderedKeys.Remove(key);
        }

        /// <summary>
        /// TODO Fix any accidental reordering issues.
        /// </summary>
        /// <param name="dest"></param>
        public void CopyTo(Document dest) {
            foreach (String key in orderedKeys) {
                dest[key] = this[key];
            }
        }

        public override bool Equals(object obj) {
            if (obj is Document) {
                return Equals(obj as Document);
            }
            return base.Equals(obj);
        }

        public bool Equals(Document obj) {
            if (obj == null)
                return false;
            if (orderedKeys.Count != obj.orderedKeys.Count)
                return false;
            return this.GetHashCode() == obj.GetHashCode();
        }

        public override int GetHashCode() {

            int hash = 27;
            foreach (var key in orderedKeys) {
                var valueHashCode = GetValueHashCode(this[key]);
                unchecked {
                    hash = (13 * hash) + key.GetHashCode();
                    hash = (13 * hash) + valueHashCode;
                }
            }
            return hash;
        }

        private int GetValueHashCode(object value) {
            if (value == null) {
                return 0;
            }
            return (value is Array) ? GetArrayHashcode((Array)value) : value.GetHashCode();
        }

        private int GetArrayHashcode(Array array) {
            var hash = 0;
            foreach (var value in array) {
                var valueHashCode = GetValueHashCode(value);
                unchecked {
                    hash = (13 * hash) + valueHashCode;
                }
            }
            return hash;
        }

        public override string ToString() {
            var json = new StringBuilder();
            json.Append("{ ");
            bool first = true;
            foreach (String key in orderedKeys) {
                if (first) {
                    first = false;
                } else {
                    json.Append(", ");
                }
                json.AppendFormat(@"""{0}"": ", key);
                SerializeType(this[key], json);
            }
            json.Append(" }");
            return json.ToString();
        }

        private void SerializeType(object value, StringBuilder json) {
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
            } else if(value is DateTime) {
                json.AppendFormat(@"""{0}""", ((DateTime)value).ToUniversalTime().ToString("o"));
            } else if(value is IFormattable) {
                json.Append(((IFormattable)value).ToString("G", CultureInfo.InvariantCulture));
            } else if(value is Document ||
                value is Oid ||
                value is int ||
                value is long ||
                value is float ||
                value is double) { 
                json.Append(value);
            } else {
                json.AppendFormat(@"""{0}""", value); 
            }
            return;
        }
    }
}
