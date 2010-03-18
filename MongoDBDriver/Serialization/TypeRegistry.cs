using System;
using System.Collections.Generic;

namespace MongoDB.Driver.Serialization
{
    /// <summary>
    /// 
    /// </summary>
    public class TypeRegistry
    {
        private readonly SerializationFactory _serializationFactory;
        private readonly Dictionary<Type, TypeEntry> _entries = new Dictionary<Type, TypeEntry>();
        private readonly Dictionary<object, TypeEntry> _names = new Dictionary<object, TypeEntry>();

        /// <summary>
        /// Initializes a new instance of the <see cref="TypeRegistry"/> class.
        /// </summary>
        /// <param name="serializationFactory">The serialization factory.</param>
        public TypeRegistry(SerializationFactory serializationFactory){
            if(serializationFactory == null)
                throw new ArgumentNullException("serializationFactory");

            _serializationFactory = serializationFactory;
        }

        /// <summary>
        /// Gets the or create.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public TypeEntry GetOrCreate(Type type){
            TypeEntry item;
            
            if(_entries.TryGetValue(type,out item))
                return item;

            var typeName = _serializationFactory.TypeNameProvider.GetName(type);

            return CreateAndAddItem(type, typeName);
        }

        /// <summary>
        /// Gets the or create.
        /// </summary>
        /// <param name="typeName">Name of the type.</param>
        /// <returns></returns>
        public TypeEntry GetOrCreate(object typeName){
            TypeEntry item;

            if(_names.TryGetValue(typeName, out item))
                return item;

            var type = _serializationFactory.TypeNameProvider.GetType(typeName);

            return CreateAndAddItem(type, typeName);
        }

        /// <summary>
        /// Creates the and add item.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="typeName">Name of the type.</param>
        /// <returns></returns>
        private TypeEntry CreateAndAddItem(Type type,object typeName){
            var item = new TypeEntry(type, typeName);
            
            _entries.Add(type,item);
            _names.Add(item.TypeName, item);

            return item;
        }
    }
}