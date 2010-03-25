using System;
using System.Collections;
using System.Collections.Generic;

using MongoDB.Driver.Configuration.CollectionAdapters;

namespace MongoDB.Driver.Configuration.Mapping.Conventions
{
    public class DefaultCollectionAdapterConvention : ICollectionAdapterConvention
    {
        public static readonly DefaultCollectionAdapterConvention Instance = new DefaultCollectionAdapterConvention();

        private delegate ICollectionAdapter CollectionTypeFactoryDelegate();
        private static readonly Dictionary<Type, CollectionTypeFactoryDelegate> _collectionTypes = new Dictionary<Type, CollectionTypeFactoryDelegate>
        {
            { typeof(ArrayList), CreateArrayListCollectionType },
            { typeof(IList), CreateArrayListCollectionType },
            { typeof(ICollection), CreateArrayListCollectionType },
            { typeof(IEnumerable), CreateArrayListCollectionType },
            { typeof(List<>), CreateGenericListCollectionType },
            { typeof(IList<>), CreateGenericListCollectionType },
            { typeof(ICollection<>), CreateGenericListCollectionType },
            { typeof(IEnumerable<>), CreateGenericListCollectionType }
        };

        private delegate Type ElementTypeFactoryDelegate(Type type);
        private static readonly Dictionary<Type, ElementTypeFactoryDelegate> _elementTypes = new Dictionary<Type, ElementTypeFactoryDelegate>
        {
            { typeof(ArrayList), GetArrayListElementType },
            { typeof(IList), GetArrayListElementType },
            { typeof(ICollection), GetArrayListElementType },
            { typeof(IEnumerable), GetArrayListElementType },
            { typeof(List<>), GetGenericListElementType },
            { typeof(IList<>), GetGenericListElementType },
            { typeof(ICollection<>), GetGenericListElementType },
            { typeof(IEnumerable<>), GetGenericListElementType }
        };

        private DefaultCollectionAdapterConvention()
        { }

        public ICollectionAdapter GetCollectionType(Type type)
        {
            CollectionTypeFactoryDelegate factory;
            if (_collectionTypes.TryGetValue(type, out factory))
                return factory();

            if (type.IsGenericType && !type.IsGenericTypeDefinition)
            {
                Type genericType = type.GetGenericTypeDefinition();
                if (_collectionTypes.TryGetValue(genericType, out factory))
                    return factory();
            }

            return null;
        }

        public Type GetElementType(Type type)
        {
            ElementTypeFactoryDelegate factory;
            if (_elementTypes.TryGetValue(type, out factory))
                return factory(type);

            if (type.IsGenericType && !type.IsGenericTypeDefinition)
            {
                Type genericType = type.GetGenericTypeDefinition();
                if (_elementTypes.TryGetValue(genericType, out factory))
                    return factory(type);
            }

            return null;
        }

        private static ArrayListCollectionAdapter CreateArrayListCollectionType()
        {
            return new ArrayListCollectionAdapter();
        }

        private static Type GetArrayListElementType(Type type)
        {
            return typeof(object);
        }

        private static GenericListCollectionAdapter CreateGenericListCollectionType()
        {
            return new GenericListCollectionAdapter();
        }

        private static Type GetGenericListElementType(Type type)
        {
            return type.GetGenericArguments()[0];
        }
    }
}