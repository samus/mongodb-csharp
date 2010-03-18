using System;

namespace MongoDB.Driver.Serialization.Handlers
{
    public class ObjectBuilderHandler : IBsonBuilderHandler
    {
        private readonly SerializationFactory _serializationFactory;
        private readonly Type _type;
        private readonly object _instance;
        private readonly TypeEntry _typeEntry;
        private TypeProperty _currentProperty;

        public ObjectBuilderHandler(SerializationFactory serializationFactory, Type type){
            if(serializationFactory == null)
                throw new ArgumentNullException("serializationFactory");
            if(type == null)
                throw new ArgumentNullException("type");

            _type = type;
            _serializationFactory = serializationFactory;
            _typeEntry = _serializationFactory.Registry.GetOrCreate(_type);
            _instance = _typeEntry.CreateInstance();
        }

        public Type BeginProperty(string name){
            _currentProperty = _typeEntry.GetProperty(name);
            return _currentProperty.PropertyType;
        }

        public void EndProperty(object value){
            _currentProperty.SetValue(_instance, value);
        }

        public object Complete(){
            return _instance;
        }
    }
}