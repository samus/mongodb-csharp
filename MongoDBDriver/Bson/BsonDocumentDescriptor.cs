using System.Collections;
using System.Collections.Generic;

namespace MongoDB.Driver.Bson
{
    /// <summary>
    /// 
    /// </summary>
    public class BsonDocumentDescriptor : IBsonObjectDescriptor
    {
        /// <summary>
        /// Begins the object.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <returns></returns>
        public object BeginObject(object instance){
            return instance;
        }

        /// <summary>
        /// Begins the array.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <returns></returns>
        public object BeginArray(object instance){
            var document = new Document();

            var i = 0;
            foreach(var item in (IEnumerable)instance)
                document.Add((i++).ToString(), item);

            return document;
        }

        /// <summary>
        /// Gets the property names.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <returns></returns>
        public IEnumerable<BsonProperty> GetPropertys(object instance){
            var document = (Document)instance;
            foreach(var key in document.Keys)
                yield return new BsonProperty(key);
        }

        /// <summary>
        /// Begins the property.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <param name="property">The property.</param>
        /// <returns></returns>
        public void BeginProperty(object instance, BsonProperty property){
            var document = (Document)instance;
            property.Value = document[property.Name];
        }

        /// <summary>
        /// Ends the property.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <param name="property">The property.</param>
        public void EndProperty(object instance, BsonProperty property){
        }

        /// <summary>
        /// Ends the array.
        /// </summary>
        /// <param name="instance">The instance.</param>
        public void EndArray(object instance){
        }

        /// <summary>
        /// Ends the object.
        /// </summary>
        /// <param name="obj">The obj.</param>
        public void EndObject(object obj){
        }

        /// <summary>
        /// Determines whether the specified obj is array.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <returns>
        /// 	<c>true</c> if the specified obj is array; otherwise, <c>false</c>.
        /// </returns>
        public bool IsArray(object obj){
            if(obj is Document)
                return false;

            return obj is IEnumerable;
        }

        /// <summary>
        /// Determines whether the specified obj is object.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <returns>
        /// 	<c>true</c> if the specified obj is object; otherwise, <c>false</c>.
        /// </returns>
        public bool IsObject(object obj){
            return obj is Document;
        }
    }
}