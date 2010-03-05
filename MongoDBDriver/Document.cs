using System;
using System.Collections;
using System.Collections.Generic;

namespace MongoDB.Driver {
    /// <summary>
    /// Description of Document.
    /// </summary>
    public class Document : DictionaryBase {
        private List<String> orderedKeys = new List<String>();
        
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

        public new void Clear(){
            Dictionary.Clear();
            orderedKeys.Clear();
        }

        /// <summary>
        /// TODO Fix any accidental reordering issues.
        /// </summary>
        /// <param name="dest"></param>
        public void CopyTo(Document dest) {
            foreach (String key in orderedKeys) {
                if(dest.Contains(key))
                    dest.Remove(key);
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
            return JsonFormatter.Serialize(this);
        }
    }
}
