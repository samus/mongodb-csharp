using System;

namespace MongoDB.Driver.Serialization.Builders
{
    /// <summary>
    /// 
    /// </summary>
    public class ObjectBuilder : IObjectBuilder
    {
        private readonly SerializationFactory _serializationFactory;
        private readonly Type _type;
        private readonly object _instance;
        private readonly TypeEntry _typeEntry;
        private TypeProperty _currentProperty;

        /// <summary>
        /// Initializes a new instance of the <see cref="ObjectBuilder"/> class.
        /// </summary>
        /// <param name="serializationFactory">The serialization factory.</param>
        /// <param name="type">The type.</param>
        public ObjectBuilder(SerializationFactory serializationFactory, Type type){
            if(serializationFactory == null)
                throw new ArgumentNullException("serializationFactory");
            if(type == null)
                throw new ArgumentNullException("type");

            _type = type;
            _serializationFactory = serializationFactory;
            _typeEntry = _serializationFactory.Registry.GetOrCreate(_type);
            _instance = _typeEntry.CreateInstance();
        }

        /// <summary>
        /// Begins the property.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public Type BeginProperty(string name){
            _currentProperty = _typeEntry.GetPropertyFromMongoName(name);
            return _currentProperty.PropertyType;
        }

        /// <summary>
        /// Ends the property.
        /// </summary>
        /// <param name="value">The value.</param>
        public void EndProperty(object value){
            _currentProperty.SetValue(_instance, value);
        }

        /// <summary>
        /// Completes this instance.
        /// </summary>
        /// <returns></returns>
        public object Complete(){
            return _instance;
        }
    }
}