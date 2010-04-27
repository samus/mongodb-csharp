using System;
using System.Collections;
using System.Collections.Generic;

using MongoDB.Driver.Configuration.CollectionAdapters;

namespace MongoDB.Driver.Configuration.Mapping.Conventions
{
    /// <summary>
    /// 
    /// </summary>
    public class DefaultCollectionAdapterConvention : ICollectionAdapterConvention
    {
        /// <summary>
        /// 
        /// </summary>
        public static readonly DefaultCollectionAdapterConvention Instance = new DefaultCollectionAdapterConvention();

        /// <summary>
        /// 
        /// </summary>
        private delegate ICollectionAdapter CollectionTypeFactoryDelegate();

        /// <summary>
        /// 
        /// </summary>
        private static readonly Dictionary<Type, CollectionTypeFactoryDelegate> CollectionTypes = new Dictionary<Type, CollectionTypeFactoryDelegate>
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

        /// <summary>
        /// 
        /// </summary>
        private static readonly Dictionary<Type, ElementTypeFactoryDelegate> ElementTypes = new Dictionary<Type, ElementTypeFactoryDelegate>
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

        /// <summary>
        /// Gets the type of the collection.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public ICollectionAdapter GetCollectionType(Type type)
        {
            CollectionTypeFactoryDelegate factory;
            if (CollectionTypes.TryGetValue(type, out factory))
                return factory();

            if (type.IsArray)
            {
                return new ArrayCollectionAdapter();
            }

            if (type.IsGenericType && !type.IsGenericTypeDefinition)
            {
                Type genericType = type.GetGenericTypeDefinition();
                if (CollectionTypes.TryGetValue(genericType, out factory))
                    return factory();
            }

            return null;
        }

        /// <summary>
        /// Gets the type of the element.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public Type GetElementType(Type type)
        {
            ElementTypeFactoryDelegate factory;
            if (ElementTypes.TryGetValue(type, out factory))
                return factory(type);

            if (type.IsArray)
            {
                return type.GetElementType();
            }

            if (type.IsGenericType && !type.IsGenericTypeDefinition)
            {
                var genericType = type.GetGenericTypeDefinition();
                if (ElementTypes.TryGetValue(genericType, out factory))
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