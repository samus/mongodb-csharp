using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using MongoDB.Util;

namespace MongoDB
{
    /// <summary>
    /// Description of Document.
    /// </summary>
    [Serializable]
    public class Document : IDictionary<string,object>, IXmlSerializable
    {
        private readonly List<string> _orderedKeys;
        private readonly Dictionary<string,object > _dictionary;
        private readonly IComparer<string> _keyComparer;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="Document"/> class.
        /// </summary>
        public Document(){
            _dictionary = new Dictionary<string, object>();
            _orderedKeys = new List<String>();
        }
        
        /// <summary>
        /// Initialize a new instance of the <see cref="Document"/> class with an optional key sorter.
        /// </summary>
        public Document(IComparer<string> keyComparer)
            :this()
        {
            if(keyComparer == null)
                throw new ArgumentNullException("keyComparer");

            _keyComparer = keyComparer;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Document"/> class and
        /// add's the given values to it.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        public Document(string key,object value)
            : this()
        {
            Add(key, value);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Document"/> class.
        /// </summary>
        /// <param name="dictionary">The dictionary.</param>
        public Document(IEnumerable<KeyValuePair<string, object>> dictionary)
            :this()
        {
            if(dictionary == null)
                throw new ArgumentNullException("dictionary");

            foreach(var entry in dictionary)
                Add(entry.Key, entry.Value);
        }

        /// <summary>
        /// Gets or sets the <see cref="System.Object"/> with the specified key.
        /// </summary>
        /// <value></value>
        public object this[string key]{
            get { return Get(key); }
            set { Set(key, value); }
        }

        /// <summary>
        /// Gets an <see cref="T:System.Collections.Generic.ICollection`1"/> containing the values in the <see cref="T:System.Collections.Generic.IDictionary`2"/>.
        /// </summary>
        /// <value></value>
        /// <returns>
        /// An <see cref="T:System.Collections.Generic.ICollection`1"/> containing the values in the object that implements <see cref="T:System.Collections.Generic.IDictionary`2"/>.
        /// </returns>
        public ICollection<object> Values{
            get { return _dictionary.Values; }
        }

        /// <summary>
        /// Gets an <see cref="T:System.Collections.Generic.ICollection`1"/> containing the keys of the <see cref="T:System.Collections.Generic.IDictionary`2"/>.
        /// </summary>
        /// <value></value>
        /// <returns>
        /// An <see cref="T:System.Collections.Generic.ICollection`1"/> containing the keys of the object that implements <see cref="T:System.Collections.Generic.IDictionary`2"/>.
        /// </returns>
        public ICollection<string> Keys{
            get { return _orderedKeys.AsReadOnly(); }
        }

        /// <summary>
        /// Gets or sets the mongo _id field.
        /// </summary>
        /// <value>The id.</value>
        public object Id
        {
            get { return this["_id"]; }
            set { this["_id"] = value; }
        }

        /// <summary>
        /// Gets the value of the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public object Get(string key)
        {
            object item;
            return _dictionary.TryGetValue(key, out item) ? item : null;
        }

        /// <summary>
        /// Gets the typed value of the specified key.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public T Get<T>(string key){
            var value = Get(key);
            if (value == null)
                return default(T);
            return (T)Convert.ChangeType(value, typeof(T));
        }

        /// <summary>
        /// Gets the value associated with the specified key.
        /// </summary>
        /// <param name="key">The key whose value to get.</param>
        /// <param name="value">When this method returns, the value associated with the specified key, if the key is found; otherwise, the default value for the type of the <paramref name="value"/> parameter. This parameter is passed uninitialized.</param>
        /// <returns>
        /// true if the object that implements <see cref="T:System.Collections.Generic.IDictionary`2"/> contains an element with the specified key; otherwise, false.
        /// </returns>
        /// <exception cref="T:System.ArgumentNullException">
        /// 	<paramref name="key"/> is null.
        /// </exception>
        public bool TryGetValue(string key, out object value){
            return _dictionary.TryGetValue(key, out value);
        }

        /// <summary>
        /// Determines whether the <see cref="T:System.Collections.Generic.IDictionary`2"/> contains an element with the specified key.
        /// </summary>
        /// <param name="key">The key to locate in the <see cref="T:System.Collections.Generic.IDictionary`2"/>.</param>
        /// <returns>
        /// true if the <see cref="T:System.Collections.Generic.IDictionary`2"/> contains an element with the key; otherwise, false.
        /// </returns>
        /// <exception cref="T:System.ArgumentNullException">
        /// 	<paramref name="key"/> is null.
        /// </exception>
        public bool ContainsKey(string key){
            return _dictionary.ContainsKey(key);
        }

        /// <summary>
        /// Adds an element with the provided key and value to the <see cref="T:System.Collections.Generic.IDictionary`2"/>.
        /// </summary>
        /// <param name="key">The object to use as the key of the element to add.</param>
        /// <param name="value">The object to use as the value of the element to add.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// 	<paramref name="key"/> is null.
        /// </exception>
        /// <exception cref="T:System.ArgumentException">
        /// An element with the same key already exists in the <see cref="T:System.Collections.Generic.IDictionary`2"/>.
        /// </exception>
        /// <exception cref="T:System.NotSupportedException">
        /// The <see cref="T:System.Collections.Generic.IDictionary`2"/> is read-only.
        /// </exception>
        public Document Add(string key, object value)
        {
            _dictionary.Add(key, value);
            _orderedKeys.Add(key);//Relies on ArgumentException from above if key already exists.
            EnsureKeyOrdering();
            return this;
        }

        /// <summary>
        /// Adds an element with the provided key and value to the <see cref="T:System.Collections.Generic.IDictionary`2"/>.
        /// </summary>
        /// <param name="key">The object to use as the key of the element to add.</param>
        /// <param name="value">The object to use as the value of the element to add.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// 	<paramref name="key"/> is null.
        /// </exception>
        /// <exception cref="T:System.ArgumentException">
        /// An element with the same key already exists in the <see cref="T:System.Collections.Generic.IDictionary`2"/>.
        /// </exception>
        /// <exception cref="T:System.NotSupportedException">
        /// The <see cref="T:System.Collections.Generic.IDictionary`2"/> is read-only.
        /// </exception>
        void IDictionary<string,object>.Add(string key, object value){
            Add(key,value);
        }

        /// <summary>
        /// Appends the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        [Obsolete("Use Add instead. This method is about to be removed in a future version.")]
        public Document Append(string key, object value){
            return Add(key, value);
        }

        /// <summary>
        /// Sets the value of the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public Document Set(string key, object value){
            if(key == null)
                throw new ArgumentNullException("key");

            if(!_orderedKeys.Contains(key))
                _orderedKeys.Add(key);

            _dictionary[key] = value;

            EnsureKeyOrdering();

            return this;
        }

        /// <summary>
        /// Adds an item to the Document at the specified position
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <param name="position">The position.</param>
        public void Insert(string key, object value, int position){
            _dictionary.Add(key, value);//Relies on ArgumentException from above if key already exists.
            _orderedKeys.Insert(position, key);
            EnsureKeyOrdering();
        }

        /// <summary>
        /// Prepends the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <returns>This document</returns>
        public Document Prepend(string key, object value){
            Insert(key, value, 0);
            return this;
        }

        /// <summary>
        /// Merges the source document into this.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns>This document</returns>
        public Document Merge(Document source)
        {
            if(source == null)
                return this;

            foreach(var key in source.Keys)
                this[key] = source[key];

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
            return (_orderedKeys.Contains(key));
        }

        /// <summary>
        /// Removes the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>
        /// true if the element is successfully removed; otherwise, false.  This method also returns false if <paramref name="key"/> was not found in the original <see cref="T:System.Collections.Generic.IDictionary`2"/>.
        /// </returns>
        /// <exception cref="T:System.ArgumentNullException">
        /// 	<paramref name="key"/> is null.
        /// </exception>
        /// <exception cref="T:System.NotSupportedException">
        /// The <see cref="T:System.Collections.Generic.IDictionary`2"/> is read-only.
        /// </exception>
        public bool Remove(string key){
            _orderedKeys.Remove(key);
            return _dictionary.Remove(key);
        }

        /// <summary>
        /// Adds an item to the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </summary>
        /// <param name="item">The object to add to the <see cref="T:System.Collections.Generic.ICollection`1"/>.</param>
        /// <exception cref="T:System.NotSupportedException">
        /// The <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only.
        /// </exception>
        void ICollection<KeyValuePair<string, object>>.Add(KeyValuePair<string, object> item){
            Add(item.Key, item.Value);
        }

        /// <summary>
        /// Clears the contents of the <see cref="T:System.Collections.DictionaryBase"/> instance.
        /// </summary>
        /// <exception cref="T:System.NotSupportedException">
        /// The <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only.
        /// </exception>
        public void Clear(){
            _dictionary.Clear();
            _orderedKeys.Clear();
        }

        /// <summary>
        /// Determines whether the <see cref="T:System.Collections.Generic.ICollection`1"/> contains a specific value.
        /// </summary>
        /// <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.ICollection`1"/>.</param>
        /// <returns>
        /// true if <paramref name="item"/> is found in the <see cref="T:System.Collections.Generic.ICollection`1"/>; otherwise, false.
        /// </returns>
        bool ICollection<KeyValuePair<string, object>>.Contains(KeyValuePair<string, object> item){
            return ((IDictionary<string, object>)_dictionary).Contains(item);
        }

        /// <summary>
        /// Copies the elements of the <see cref="T:System.Collections.Generic.ICollection`1"/> to an <see cref="T:System.Array"/>, starting at a particular <see cref="T:System.Array"/> index.
        /// </summary>
        /// <param name="array">The one-dimensional <see cref="T:System.Array"/> that is the destination of the elements copied from <see cref="T:System.Collections.Generic.ICollection`1"/>. The <see cref="T:System.Array"/> must have zero-based indexing.</param>
        /// <param name="arrayIndex">The zero-based index in <paramref name="array"/> at which copying begins.</param>
        void ICollection<KeyValuePair<string, object>>.CopyTo(KeyValuePair<string, object>[] array, int arrayIndex){
            ((ICollection<KeyValuePair<string, object>>)_dictionary).CopyTo(array,arrayIndex);
        }
        
        /// <summary>
        /// TODO Fix any accidental reordering issues.
        /// </summary>
        /// <param name="destinationDocument">The dest.</param>
        public void CopyTo(Document destinationDocument){
            if(destinationDocument == null)
                throw new ArgumentNullException("destinationDocument");
            
            //Todo: Fix any accidental reordering issues.

            foreach(var key in _orderedKeys){
                if(destinationDocument.Contains(key))
                    destinationDocument.Remove(key);
                destinationDocument[key] = this[key];
            }
        }
        /// <summary>
        /// Removes the first occurrence of a specific object from the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </summary>
        /// <param name="item">The object to remove from the <see cref="T:System.Collections.Generic.ICollection`1"/>.</param>
        /// <returns>
        /// true if <paramref name="item"/> was successfully removed from the <see cref="T:System.Collections.Generic.ICollection`1"/>; otherwise, false. This method also returns false if <paramref name="item"/> is not found in the original <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </returns>
        /// <exception cref="T:System.NotSupportedException">
        /// The <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only.
        /// </exception>
        public bool Remove(KeyValuePair<string, object> item){
            var removed = ((ICollection<KeyValuePair<string, object>>)_dictionary).Remove(item);
            if(removed)
                _orderedKeys.Remove(item.Key);
            return removed;
        }
        
        /// <summary>
        /// Gets the number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </summary>
        /// <value></value>
        /// <returns>
        /// The number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </returns>
        public int Count{
            get { return _dictionary.Count; }
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only.
        /// </summary>
        /// <value></value>
        /// <returns>true if the <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only; otherwise, false.
        /// </returns>
        public bool IsReadOnly{
            get { return false; }
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
        /// <param name="document">The obj.</param>
        /// <returns></returns>
        public bool Equals(Document document){
            if(document == null)
                return false;
            if(_orderedKeys.Count != document._orderedKeys.Count)
                return false;
            return GetHashCode() == document.GetHashCode();
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode(){
            var hash = 27;
            foreach(var key in _orderedKeys){
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
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator(){
            return GetEnumerator();
        }
        
        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<KeyValuePair<string, object>> GetEnumerator(){
            return _orderedKeys.Select(orderedKey => new KeyValuePair<string, object>(orderedKey, _dictionary[orderedKey])).GetEnumerator();
        }

        /// <summary>
        /// Toes the dictionary.
        /// </summary>
        /// <returns></returns>
        public Dictionary<string,object> ToDictionary(){
            return new Dictionary<string, object>(this);
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

        /// <summary>
        /// Ensures the key ordering.
        /// </summary>
        private void EnsureKeyOrdering(){
            if(_keyComparer==null)
                return;

            _orderedKeys.Sort(_keyComparer);
        }

        /// <summary>
        /// This method is reserved and should not be used. When implementing the IXmlSerializable interface, you should return null (Nothing in Visual Basic) from this method, and instead, if specifying a custom schema is required, apply the <see cref="T:System.Xml.Serialization.XmlSchemaProviderAttribute"/> to the class.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Xml.Schema.XmlSchema"/> that describes the XML representation of the object that is produced by the <see cref="M:System.Xml.Serialization.IXmlSerializable.WriteXml(System.Xml.XmlWriter)"/> method and consumed by the <see cref="M:System.Xml.Serialization.IXmlSerializable.ReadXml(System.Xml.XmlReader)"/> method.
        /// </returns>
        XmlSchema IXmlSerializable.GetSchema()
        {
            return null;
        }

        /// <summary>
        /// Generates an object from its XML representation.
        /// </summary>
        /// <param name="reader">The <see cref="T:System.Xml.XmlReader"/> stream from which the object is deserialized.</param>
        void IXmlSerializable.ReadXml(XmlReader reader)
        {
            reader.ReadStartElement();

            while(reader.IsStartElement())
            {
                var key = reader.Name;
                object value = null;

                if(reader.MoveToAttribute("type"))
                {
                    var type = Type.GetType(reader.Value);

                    reader.ReadStartElement();

                    var serializer = new XmlSerializer(type);
                    value = serializer.Deserialize(reader);
                }
                else
                    reader.Read();

                Add(key, value);
            }
        }

        /// <summary>
        /// Converts an object into its XML representation.
        /// </summary>
        /// <param name="writer">The <see cref="T:System.Xml.XmlWriter"/> stream to which the object is serialized.</param>
        void IXmlSerializable.WriteXml(XmlWriter writer)
        {
            foreach(var pair in this)
            {
                writer.WriteStartElement(pair.Key);

                if(pair.Value == null)
                    continue;
                
                var type = pair.Value.GetType();
                writer.WriteAttributeString("type", type.AssemblyQualifiedName);
                var serializer = new XmlSerializer(type);
                serializer.Serialize(writer,pair.Value);
            }
        }
    }
}
