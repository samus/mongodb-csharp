using System.Collections.Generic;

namespace MongoDB.Serialization
{
    /// <summary>
    /// 
    /// </summary>
    public class DocumentObjectDescriptorAdapter : IObjectDescriptor
    {
        /// <summary>
        /// Generates the id.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <returns></returns>
        public object GenerateId(object instance)
        {
            return Oid.NewOid();
        }

        /// <summary>
        /// Gets the property value.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <param name="mongoName">Name of the mongo.</param>
        /// <returns></returns>
        public object GetPropertyValue(object instance, string mongoName)
        {
            return ((Document)instance)[mongoName];
        }

        /// <summary>
        /// Sets the property value.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <param name="mongoName">Name of the mongo.</param>
        /// <param name="value">The value.</param>
        public void SetPropertyValue(object instance, string mongoName, object value)
        {
            ((Document)instance)[mongoName] = value;
        }

        /// <summary>
        /// Gets the mongo property names.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <returns></returns>
        public IEnumerable<string> GetMongoPropertyNames(object instance)
        {
            return ((Document)instance).Keys;
        }
    }
}