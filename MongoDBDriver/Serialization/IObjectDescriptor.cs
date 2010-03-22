using System.Collections.Generic;

namespace MongoDB.Driver.Serialization
{
    /// <summary>
    /// 
    /// </summary>
    public interface IObjectDescriptor
    {
        /// <summary>
        /// Generates the id.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <returns></returns>
        object GenerateId(object instance);

        /// <summary>
        /// Gets the property value.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <param name="mongoName">Name of the mongo.</param>
        /// <returns></returns>
        object GetPropertyValue(object instance, string mongoName);

        /// <summary>
        /// Sets the property value.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <param name="mongoName">Name of the mongo.</param>
        /// <param name="value">The value.</param>
        void SetPropertyValue(object instance, string mongoName, object value);

        /// <summary>
        /// Gets the mongo property names.
        /// </summary>
        /// <returns></returns>
        IEnumerable<string> GetMongoPropertyNames();
    }
}