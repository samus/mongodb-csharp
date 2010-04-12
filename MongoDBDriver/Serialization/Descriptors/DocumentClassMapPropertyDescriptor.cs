using System;
using System.Collections.Generic;
using MongoDB.Driver.Configuration.Mapping.Model;
using MongoDB.Driver.Configuration.Mapping;

namespace MongoDB.Driver.Serialization.Descriptors
{
    internal class DocumentClassMapPropertyDescriptor : ClassMapPropertyDescriptorBase
    {
        private readonly Document _document;

        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentClassMapPropertyDescriptor"/> class.
        /// </summary>
        /// <param name="mappingStore">The mapping store.</param>
        /// <param name="classMap">The class map.</param>
        /// <param name="document">The document.</param>
        public DocumentClassMapPropertyDescriptor(IMappingStore mappingStore, IClassMap classMap, Document document)
            : base(mappingStore, classMap)
        {
            if (document == null)
                throw new ArgumentNullException("document");

            _document = document;
        }

        /// <summary>
        /// Gets the property names.
        /// </summary>
        /// <returns></returns>
        public override IEnumerable<KeyValuePair<string, KeyValuePair<Type, object>>> GetProperties()
        {
            if (ShouldPersistDiscriminator())
                yield return CreateProperty(ClassMap.DiscriminatorAlias, ClassMap.Discriminator.GetType(), ClassMap.Discriminator);

            foreach (string key in _document.Keys)
            {
                var alias = GetAliasFromMemberName(key);
                var valueAndType = GetPropertyTypeAndValue(key);
                yield return CreateProperty(alias, valueAndType);
            }
        }

        /// <summary>
        /// Gets the property type and value.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        private KeyValuePair<Type, object> GetPropertyTypeAndValue(string name)
        {
            if (ClassMap.DiscriminatorAlias == name && ShouldPersistDiscriminator())
                return new KeyValuePair<Type, object>(ClassMap.Discriminator.GetType(), ClassMap.Discriminator);

            var value = _document[name];

            var memberMap = GetMemberMapFromMemberName(name);
            var type = typeof(Document);

            if (memberMap != null)
            {
                type = memberMap.MemberReturnType;
                if (memberMap is CollectionMemberMap)
                    type = ((CollectionMemberMap)memberMap).ElementType;
            }
            else if (name.StartsWith("$") || name == "query") //we are a modifier or a special case of querying
                type = ClassMap.ClassType; //we'll pass this along so that the fields get replaced correctly...
            else if (value != null)
                type = value.GetType();

            return new KeyValuePair<Type, object>(type, value);
        }
    }
}