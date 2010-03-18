using System;
using System.Collections.Generic;

namespace MongoDB.Driver.Serialization
{
    /// <summary>
    /// 
    /// </summary>
    public class TypeRegistry
    {
        private readonly Dictionary<Type, TypeRegistryItem> _items = new Dictionary<Type, TypeRegistryItem>();
        private readonly Dictionary<object, TypeRegistryItem> _names = new Dictionary<object, TypeRegistryItem>();

        /// <summary>
        /// Gets the or create.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public TypeRegistryItem GetOrCreate(Type type){
            TypeRegistryItem item;
            
            if(_items.TryGetValue(type,out item))
                return item;

            _items.Add(type,item = new TypeRegistryItem(type));
            _names.Add(item.TypeName, item);

            return item;
        }
    }
}