using System;
using System.Collections;
using System.Collections.Generic;

namespace MongoDB.Driver
{
    /// <summary>
    ///   Description of Document.
    /// </summary>
    public class Document : DictionaryBase
    {
        private readonly List<String> orderedKeys = new List<String>();

        /// <summary>
        /// Gets or sets the <see cref="System.Object"/> with the specified key.
        /// </summary>
        /// <value></value>
        public Object this[String key]{
            get { return Dictionary[key]; }
            set{
                if(orderedKeys.Contains(key) == false)
                    orderedKeys.Add(key);
                Dictionary[key] = value;
            }
        }

        /// <summary>
        /// Gets or sets the mongo _id field.
        /// </summary>
        /// <value>The id.</value>
        public object Id{
            get { return this["_id"]; }
            set { this["_id"] = value; }
        }

        /// <summary>
        /// Gets an <see cref="T:System.Collections.ICollection"/> object containing the keys of the <see cref="T:System.Collections.IDictionary"/> object.
        /// </summary>
        /// <value></value>
        /// <returns>
        /// An <see cref="T:System.Collections.ICollection"/> object containing the keys of the <see cref="T:System.Collections.IDictionary"/> object.
        /// </returns>
        public ICollection Keys{
            get { return (orderedKeys); }
        }

        /// <summary>
        /// Gets an <see cref="T:System.Collections.ICollection"/> object containing the values in the <see cref="T:System.Collections.IDictionary"/> object.
        /// </summary>
        /// <value></value>
        /// <returns>
        /// An <see cref="T:System.Collections.ICollection"/> object containing the values in the <see cref="T:System.Collections.IDictionary"/> object.
        /// </returns>
        public ICollection Values{
            get { return (Dictionary.Values); }
        }

        /// <summary>
        /// Gets the typed value of the specified key.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public T Get<T>(string key){
            return (T)this[key];
        }

        /// <summary>
        /// Adds the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        public void Add(String key, Object value){
            Dictionary.Add(key, value);
            //Relies on ArgumentException from above if key already exists.
            orderedKeys.Add(key);
        }

        /// <summary>
        /// Appends the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public Document Append(String key, Object value){
            Add(key, value);
            return this;
        }

        /// <summary>
        /// Adds an item to the Document at the specified position
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <param name="position">The position.</param>
        public void Insert(String key, Object value, int position){
            Dictionary.Add(key, value);
            //Relies on ArgumentException from above if key already exists.
            orderedKeys.Insert(position, key);
        }

        /// <summary>
        /// Prepends the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public Document Prepend(String key, Object value){
            Insert(key, value, 0);
            return this;
        }

        /// <summary>
        /// Updates the specified from.
        /// </summary>
        /// <param name="from">From.</param>
        /// <returns></returns>
        public Document Update(Document @from){
            if(@from == null)
                return this;
            
            foreach(String key in @from.Keys)
                this[key] = @from[key];
            
            return this;
        }

        /// <summary>
        /// Determines whether [contains] [the specified key].
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>
        /// 	<c>true</c> if [contains] [the specified key]; otherwise, <c>false</c>.
        /// </returns>
        public bool Contains(String key){
            return (orderedKeys.Contains(key));
        }

        /// <summary>
        /// Removes the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        public void Remove(String key){
            Dictionary.Remove(key);
            orderedKeys.Remove(key);
        }

        /// <summary>
        /// Clears the contents of the <see cref="T:System.Collections.DictionaryBase"/> instance.
        /// </summary>
        public new void Clear(){
            Dictionary.Clear();
            orderedKeys.Clear();
        }

        /// <summary>
        /// TODO Fix any accidental reordering issues.
        /// </summary>
        /// <param name="dest">The dest.</param>
        public void CopyTo(Document dest){
            foreach(var key in orderedKeys){
                if(dest.Contains(key))
                    dest.Remove(key);
                dest[key] = this[key];
            }
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object"/> to compare with this instance.</param>
        /// <returns>
        /// 	<c>true</c> if the specified <see cref="System.Object"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="T:System.NullReferenceException">
        /// The <paramref name="obj"/> parameter is null.
        /// </exception>
        public override bool Equals(object obj){
            if(obj is Document)
                return Equals(obj as Document);
            return base.Equals(obj);
        }

        /// <summary>
        /// Equalses the specified obj.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <returns></returns>
        public bool Equals(Document obj){
            if(obj == null)
                return false;
            if(orderedKeys.Count != obj.orderedKeys.Count)
                return false;
            return GetHashCode() == obj.GetHashCode();
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode(){
            var hash = 27;
            foreach(var key in orderedKeys){
                var valueHashCode = GetValueHashCode(this[key]);
                unchecked{
                    hash = (13*hash) + key.GetHashCode();
                    hash = (13*hash) + valueHashCode;
                }
            }
            return hash;
        }

        /// <summary>
        /// Gets the value hash code.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        private int GetValueHashCode(object value){
            if(value == null)
                return 0;
            return (value is Array) ? GetArrayHashcode((Array)value) : value.GetHashCode();
        }

        /// <summary>
        /// Gets the array hashcode.
        /// </summary>
        /// <param name="array">The array.</param>
        /// <returns></returns>
        private int GetArrayHashcode(Array array){
            var hash = 0;
            foreach(var value in array){
                var valueHashCode = GetValueHashCode(value);
                unchecked{
                    hash = (13*hash) + valueHashCode;
                }
            }
            return hash;
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString(){
            return JsonFormatter.Serialize(this);
        }
    }
}