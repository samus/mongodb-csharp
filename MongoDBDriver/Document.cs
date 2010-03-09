using System;
using System.Collections;
using System.Collections.Generic;

namespace MongoDB.Driver
{
    /// <summary>
    /// A Document is the base container for an entry inside of a <see cref="Collection/>.
    /// </summary>
    public class Document : IEnumerable<KeyValuePair<String, Object>>
    {
        private List<String> orderedKeys = new List<String> ();
        private Dictionary<String, Object> dictionary = new Dictionary<String, Object>();
        

        #region "Properties"
        public Object this[String key] {
            get { return dictionary[key]; }
            set {
                if (orderedKeys.Contains (key) == false) {
                    orderedKeys.Add (key);
                }
                dictionary[key] = value;
            }
        }

        public ICollection Keys {
            get { return (orderedKeys); }
        }

        public ICollection Values {
            get { return (dictionary.Values); }
        }

        public int Count {
            get { return orderedKeys.Count;}
        }
        
        #endregion
        
        #region "Constructors"
        public Document(){}
        
        public Document(string key, object value){
            this.Add(key, value);
        }
        
        public Document(IEnumerable<KeyValuePair<string,object>> kvps){
            foreach(KeyValuePair<string,object> kvp in kvps){
                this.Add(kvp.Key, kvp.Value);
            }
        }
        #endregion
        
        public void Add (String key, Object value){
            dictionary.Add (key, value);
            //Relies on ArgumentException from above if key already exists.
            orderedKeys.Add (key);
        }

        public Document Append (String key, Object value){
            this.Add (key, value);
            return this;
        }

        /// <summary>
        /// Adds an item to the Document at the specified position
        /// </summary>
        public void Insert (String key, Object value, int Position){
            dictionary.Add (key, value);
            //Relies on ArgumentException from above if key already exists.
            orderedKeys.Insert (Position, key);
        }
        public Document Prepend (String key, Object value){
            this.Insert (key, value, 0);
            return this;
        }

        public Document Update (Document @from){
            if (@from == null)
                return this;
            foreach (String key in @from.Keys) {
                this[key] = @from[key];
            }
            return this;
        }

        public bool Contains (String key){
            return (orderedKeys.Contains (key));
        }

        public void Remove (String key){
            dictionary.Remove (key);
            orderedKeys.Remove (key);
        }

        public new void Clear (){
            dictionary.Clear ();
            orderedKeys.Clear ();
        }

        /// <summary>
        /// TODO Fix any accidental reordering issues.
        /// </summary>
        /// <param name="dest"></param>
        public void CopyTo (Document dest){
            foreach (String key in orderedKeys) {
                if (dest.Contains (key))
                    dest.Remove (key);
                dest[key] = this[key];
            }
        }
        
        public override bool Equals (object obj){
            if (obj is Document) {
                return Equals (obj as Document);
            }
            return base.Equals (obj);
        }

        public bool Equals (Document obj){
            if (obj == null)
                return false;
            if (orderedKeys.Count != obj.orderedKeys.Count)
                return false;
            return this.GetHashCode () == obj.GetHashCode ();
        }

        public override int GetHashCode (){
            
            int hash = 27;
            foreach (var key in orderedKeys) {
                var valueHashCode = GetValueHashCode (this[key]);
                unchecked {
                    hash = (13 * hash) + key.GetHashCode ();
                    hash = (13 * hash) + valueHashCode;
                }
            }
            return hash;
        }

        private int GetValueHashCode (object value){
            if (value == null) {
                return 0;
            }
            return (value is Array) ? GetArrayHashcode ((Array)value) : value.GetHashCode ();
        }

        private int GetArrayHashcode (Array array){
            var hash = 0;
            foreach (var value in array) {
                var valueHashCode = GetValueHashCode (value);
                unchecked {
                    hash = (13 * hash) + valueHashCode;
                }
            }
            return hash;
        }

        public override string ToString (){
            return JsonFormatter.Serialize (this);
        }
        
        
        public IEnumerator<KeyValuePair<string, object>> GetEnumerator (){
            foreach(KeyValuePair<string,object> kvp in dictionary){
                yield return kvp;
            }
        }
        
        IEnumerator IEnumerable.GetEnumerator (){
            return GetEnumerator();
        }
        
    }
}
