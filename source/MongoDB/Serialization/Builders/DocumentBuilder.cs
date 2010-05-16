using System;

namespace MongoDB.Serialization.Builders
{
    internal class DocumentBuilder : IObjectBuilder
    {
        private readonly Document _document;

        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentBuilder"/> class.
        /// </summary>
        public DocumentBuilder()
        {
            _document = new Document();
        }

        /// <summary>
        /// Adds the property.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        public void AddProperty(string name, object value)
        {
            _document.Add(name, value);
        }

        /// <summary>
        /// Builds the object.
        /// </summary>
        /// <returns></returns>
        public object BuildObject()
        {
            return _document;
        }

        /// <summary>
        /// Gets the type of the property.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public PropertyDescriptor GetPropertyDescriptor(string name)
        {
            return new PropertyDescriptor();
        }
    }
}