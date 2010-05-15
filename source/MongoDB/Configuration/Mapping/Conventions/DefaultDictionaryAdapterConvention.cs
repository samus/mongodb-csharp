using System;
using System.Collections;
using System.Collections.Generic;

using MongoDB.Configuration.CollectionAdapters;
using MongoDB.Configuration.DictionaryAdapters;

namespace MongoDB.Configuration.Mapping.Conventions
{
    /// <summary>
    /// 
    /// </summary>
    public class DefaultDictionaryAdapterConvention : IDictionarynAdapterConvention
    {
        /// <summary>
        /// 
        /// </summary>
        public static readonly DefaultDictionaryAdapterConvention Instance = new DefaultDictionaryAdapterConvention();

        /// <summary>
        /// 
        /// </summary>
        private delegate IDictionaryAdapter DictionaryTypeFactoryDelegate();

        /// <summary>
        /// 
        /// </summary>
        private static readonly Dictionary<Type, DictionaryTypeFactoryDelegate> DictionaryTypes = new Dictionary<Type, DictionaryTypeFactoryDelegate>
        {
            { typeof(Document), CreateDocumentType },
            { typeof(IDictionary), CreateHashtableType },
            { typeof(Hashtable), CreateHashtableType },
            { typeof(IEnumerable<DictionaryEntry>), CreateHashtableType },
            { typeof(Dictionary<,>), CreateGenericDictionaryType },
            { typeof(IDictionary<,>), CreateGenericDictionaryType },
        };

        private delegate Type ValueTypeFactoryDelegate(Type type);

        /// <summary>
        /// 
        /// </summary>
        private static readonly Dictionary<Type, ValueTypeFactoryDelegate> ValueTypes = new Dictionary<Type, ValueTypeFactoryDelegate>
        {
            { typeof(Document), GetDocumentValueType },
            { typeof(IDictionary), GetHashtableValueType },
            { typeof(Hashtable), GetHashtableValueType },
            { typeof(IEnumerable<DictionaryEntry>), GetHashtableValueType },
            { typeof(Dictionary<,>), GetGenericDictionaryValueType },
            { typeof(IDictionary<,>), GetGenericDictionaryValueType },
        };

        private DefaultDictionaryAdapterConvention()
        { }

        /// <summary>
        /// Gets the type of the dictionary.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public IDictionaryAdapter GetDictionaryType(Type type)
        {
            DictionaryTypeFactoryDelegate factory;
            if (DictionaryTypes.TryGetValue(type, out factory))
                return factory();

            if (type.IsGenericType && !type.IsGenericTypeDefinition)
            {
                Type genericType = type.GetGenericTypeDefinition();
                if (DictionaryTypes.TryGetValue(genericType, out factory))
                    return factory();
            }

            return null;
        }

        /// <summary>
        /// Gets the type of the value.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public Type GetValueType(Type type)
        {
            ValueTypeFactoryDelegate factory;
            if (ValueTypes.TryGetValue(type, out factory))
                return factory(type);

            if (type.IsGenericType && !type.IsGenericTypeDefinition)
            {
                var genericType = type.GetGenericTypeDefinition();
                if (ValueTypes.TryGetValue(genericType, out factory))
                    return factory(type);
            }

            return null;
        }

        private static IDictionaryAdapter CreateDocumentType()
        {
            return new DocumentDictionaryAdapter();
        }

        private static Type GetDocumentValueType(Type type)
        {
            return typeof(object);
        }

        private static IDictionaryAdapter CreateHashtableType()
        {
            return new HashtableDictionaryAdapter();
        }

        private static Type GetHashtableValueType(Type type)
        {
            return typeof(object);
        }

        private static IDictionaryAdapter CreateGenericDictionaryType()
        {
            return new GenericDictionaryDictionaryAdapter();
        }

        private static Type GetGenericDictionaryValueType(Type type)
        {
            type = type.GetGenericTypeDefinition();
            return type.GetGenericArguments()[1];
        }
    }
}