using System;
using System.Collections;
using System.Collections.Generic;
using MongoDB.Configuration.DictionaryAdapters;

namespace MongoDB.Configuration.Mapping.Conventions
{
    /// <summary>
    /// </summary>
    public class DefaultDictionaryAdapterConvention : IDictionarynAdapterConvention
    {
        /// <summary>
        /// </summary>
        private static readonly Dictionary<Type, DictionaryTypeFactoryDelegate> DictionaryTypes = new Dictionary<Type, DictionaryTypeFactoryDelegate>
        {
            {typeof(Document), CreateDocumentType},
            {typeof(IDictionary), CreateHashtableType},
            {typeof(Hashtable), CreateHashtableType},
            {typeof(IEnumerable<DictionaryEntry>), CreateHashtableType}
        };

        /// <summary>
        /// </summary>
        public static readonly DefaultDictionaryAdapterConvention Instance = new DefaultDictionaryAdapterConvention();

        private DefaultDictionaryAdapterConvention()
        {
        }

        /// <summary>
        ///   Gets the type of the dictionary.
        /// </summary>
        /// <param name = "type">The type.</param>
        /// <returns></returns>
        public IDictionaryAdapter GetDictionaryAdapter(Type type)
        {
            DictionaryTypeFactoryDelegate factory;
            if(DictionaryTypes.TryGetValue(type, out factory))
                return factory();

            if(type.IsGenericType && !type.IsGenericTypeDefinition)
            {
                var genericType = type.GetGenericTypeDefinition();

                if(genericType == typeof(SortedList<,>))
                {
                    var genericArgs = type.GetGenericArguments();
                    var adapterType = typeof(GenericSortedListDictionaryAdapter<,>).MakeGenericType(genericArgs[0], genericArgs[1]);
                    return (IDictionaryAdapter)Activator.CreateInstance(adapterType);
                }

                if(genericType == typeof(IDictionary<,>) ||
                   genericType == typeof(Dictionary<,>))
                {
                    var genericArgs = type.GetGenericArguments();
                    var adapterType = typeof(GenericDictionaryDictionaryAdapter<,>).MakeGenericType(genericArgs[0], genericArgs[1]);
                    return (IDictionaryAdapter)Activator.CreateInstance(adapterType);
                }
            }

            return null;
        }

        private static IDictionaryAdapter CreateDocumentType()
        {
            return new DocumentDictionaryAdapter();
        }

        private static IDictionaryAdapter CreateHashtableType()
        {
            return new HashtableDictionaryAdapter();
        }

        /// <summary>
        /// </summary>
        private delegate IDictionaryAdapter DictionaryTypeFactoryDelegate();
    }
}