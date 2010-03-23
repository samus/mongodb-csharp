using System.Collections.Generic;

namespace MongoDB.Driver.Bson
{
    /// <summary>
    /// 
    /// </summary>
    public interface IBsonObjectDescriptor
    {
        /// <summary>
        /// Begins the object.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <returns></returns>
        object BeginObject(object instance);

        /// <summary>
        /// Begins the array.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <returns></returns>
        object BeginArray(object instance);

        /// <summary>
        /// Gets the property names.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <returns></returns>
        IEnumerable<string> GetPropertyNames(object instance);

        /// <summary>
        /// Begins the property.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        object BeginProperty(object instance, string name);

        /// <summary>
        /// Ends the property.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        void EndProperty(object instance, string name, object value);

        /// <summary>
        /// Ends the array.
        /// </summary>
        /// <param name="instance">The instance.</param>
        void EndArray(object instance);

        /// <summary>
        /// Ends the object.
        /// </summary>
        /// <param name="instance">The instance.</param>
        void EndObject(object instance);

        /// <summary>
        /// Determines whether the specified instance is array.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <returns>
        /// 	<c>true</c> if the specified instance is array; otherwise, <c>false</c>.
        /// </returns>
        bool IsArray(object instance);

        /// <summary>
        /// Determines whether the specified instance is object.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <returns>
        /// 	<c>true</c> if the specified instance is object; otherwise, <c>false</c>.
        /// </returns>
        bool IsObject(object instance);
    }
}