using System;
using System.Collections;
using System.Collections.Generic;

namespace MongoDB.Driver
{
    /// <summary>
    /// Description of Document.
    /// </summary>
    public class Document : DictionaryBase
    {
        private bool _dirty;
        private List<String> orderedKeys;
        private IComparer<string> keyComparer;


        public Document() : this(null) {
        }

        public Document(IComparer<string> comparer) {
            orderedKeys = new List<string>();
            keyComparer = comparer;
        }

        public Object this[String key] {
            get { return Dictionary[key]; }
            set {
                _dirty = true;
                Dictionary[key] = value;
            }
        }

        public IList<string> Keys {
            get {
                if(keyComparer != null && _dirty)
                    orderedKeys.Sort(keyComparer);
                return orderedKeys;
            }
        }

        public ICollection Values {
            get { return (Dictionary.Values); }
        }

        public void Add(String key, Object value) {
            _dirty = true;
            Dictionary.Add(key, value);
        }

        public Document Append(String key, Object value) {
            this.Add(key, value);
            return this;
        }

        /// <summary>
        /// Adds an item to the Document at the specified position
        /// </summary>
        public void Insert(String key, Object value, int index) {
            bool alreadyContains = orderedKeys.Contains(key);
            if(!alreadyContains)
                orderedKeys.Insert(index, key);
            else
                throw new ArgumentException("Key already exists in Document", "key");
            
            try {
                Dictionary.Add(key, value);
            } catch {
                if(!alreadyContains)
                    orderedKeys.Remove(key);
                throw;
            }
            _dirty = true;
        }

        public Document Prepend(String key, Object value) {
            this.Insert(key, value, 0);
            return this;
        }

        public Document Update(Document @from) {
            if(@from == null)
                return this;
            foreach(String key in @from.Keys) {
                this[key] = @from[key];
            }
            return this;
        }

        public bool Contains(String key) {
            return (orderedKeys.Contains(key));
        }

        public void Remove(String key) {
            _dirty = true;
            Dictionary.Remove(key);
        }

        public new void Clear() {
            Dictionary.Clear();
        }

        /// <summary>
        /// TODO Fix any accidental reordering issues.
        /// </summary>
        /// <param name="dest"></param>
        public void CopyTo(Document dest) {
            foreach(String key in orderedKeys) {
                if(dest.Contains(key))
                    dest.Remove(key);
                dest[key] = this[key];
            }
        }

        public override bool Equals(object obj) {
            if(obj is Document) {
                return Equals(obj as Document);
            }
            return base.Equals(obj);
        }

        public bool Equals(Document obj) {
            if(obj == null)
                return false;
            if(Keys.Count != obj.Keys.Count)
                return false;
            return this.GetHashCode() == obj.GetHashCode();
        }

        public override int GetHashCode() {
            int hash = 27;
            foreach(string key in Keys) {
                var valueHashCode = GetValueHashCode(this[key]);
                unchecked {
                    hash = (13 * hash) + key.GetHashCode();
                    hash = (13 * hash) + valueHashCode;
                }
            }
            return hash;
        }

        private int GetValueHashCode(object value) {
            if(value == null) {
                return 0;
            }
            return (value is Array) ? GetArrayHashcode((Array)value) : value.GetHashCode();
        }

        private int GetArrayHashcode(Array array) {
            var hash = 0;
            foreach(var value in array) {
                var valueHashCode = GetValueHashCode(value);
                unchecked {
                    hash = (13 * hash) + valueHashCode;
                }
            }
            return hash;
        }

        protected override void OnClear() {
            orderedKeys.Clear();
            base.OnClear();
        }

        protected override void OnInsertComplete(object key, object value) {
            string strKey = key as string;
            if(!orderedKeys.Contains(strKey))
                orderedKeys.Add(strKey);
            base.OnInsertComplete(key, value);
        }

        protected override void OnRemoveComplete(object key, object value) {
            string strKey = key as string;
            orderedKeys.Remove(strKey);
            base.OnRemoveComplete(key, value);
        }

        protected override void OnSetComplete(object key, object oldValue, object newValue) {
            string strKey = key as string;
            if(!orderedKeys.Contains(strKey))
                orderedKeys.Add(strKey);
            base.OnSetComplete(key, oldValue, newValue);
        }


        public override string ToString() {
            return JsonFormatter.Serialize(this);
        }
    }
}
