using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace MongoDB.Bson
{
    /// <summary>
    /// </summary>
    public class BsonDocumentBuilder : IBsonObjectBuilder
    {
        /// <summary>
        ///   Begins the object.
        /// </summary>
        /// <returns></returns>
        public object BeginObject()
        {
            return new Document();
        }

        /// <summary>
        ///   Ends the object.
        /// </summary>
        /// <param name = "instance">The instance.</param>
        /// <returns></returns>
        public object EndObject(object instance)
        {
            var document = (Document)instance;

            if(DBRef.IsDocumentDBRef(document))
                return DBRef.FromDocument(document);

            return document;
        }

        /// <summary>
        ///   Begins the array.
        /// </summary>
        /// <returns></returns>
        public object BeginArray()
        {
            return BeginObject();
        }

        /// <summary>
        ///   Ends the array.
        /// </summary>
        /// <param name = "instance">The instance.</param>
        /// <returns></returns>
        public object EndArray(object instance)
        {
            var document = (Document)EndObject(instance);
            return ConvertToArray(document);
        }

        /// <summary>
        ///   Begins the property.
        /// </summary>
        /// <param name = "instance">The instance.</param>
        /// <param name = "name">The name.</param>
        public void BeginProperty(object instance, string name)
        {
        }

        /// <summary>
        ///   Ends the property.
        /// </summary>
        /// <param name = "instance">The instance.</param>
        /// <param name = "name">The name.</param>
        /// <param name = "value">The value.</param>
        public void EndProperty(object instance, string name, object value)
        {
            var document = (Document)instance;
            document.Add(name, value);
        }

        /// <summary>
        ///   Gets the type for IEnumerable.
        /// </summary>
        /// <param name = "doc">The doc.</param>
        /// <returns></returns>
        private Type GetTypeForIEnumerable(IDictionary<string, object> doc)
        {
            if(doc.Keys.Count < 1)
                return typeof(Object);
            
            Type comp = null;
            
            foreach(var test in doc.Keys.Select(key => doc[key])
                .Select(obj => obj.GetType()))
            {
                if(comp == null)
                    comp = test;
                else if(comp != test)
                    return typeof(Object);
            }

            return comp;
        }

        /// <summary>
        ///   Converts to array.
        /// </summary>
        /// <param name = "doc">The doc.</param>
        /// <returns></returns>
        private IEnumerable ConvertToArray(Document doc)
        {
            var genericListType = typeof(List<>);
            var arrayType = GetTypeForIEnumerable(doc);
            var listType = genericListType.MakeGenericType(arrayType);

            var list = (IList)Activator.CreateInstance(listType,doc.Count);

            foreach(var key in doc.Keys)
                list.Add(doc[key]);

            return list;
        }
    }
}