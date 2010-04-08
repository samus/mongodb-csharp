using System;
using System.Collections.Generic;
using MongoDB.Driver.Configuration.Mapping.Model;

namespace MongoDB.Driver.Serialization.Descriptors
{
    internal class DocumentClassMapPropertyDescriptor : ClassMapPropertyDescriptorBase
    {
        private readonly Document _document;

        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentClassMapPropertyDescriptor"/> class.
        /// </summary>
        /// <param name="classMap">The class map.</param>
        /// <param name="document">The document.</param>
        public DocumentClassMapPropertyDescriptor(IClassMap classMap, Document document)
            : base(classMap)
        {
            if (document == null)
                throw new ArgumentNullException("document");

            _document = document;
        }

        /// <summary>
        /// Gets the property names.
        /// </summary>
        /// <returns></returns>
        public override IEnumerable<string> GetPropertyNames()
        {
            if (ShouldPersistDiscriminator())
                yield return ClassMap.DiscriminatorAlias;

            PersistentMemberMap memberMap;
            foreach (string key in _document.Keys)
            {
                memberMap = ClassMap.GetMemberMapFromMemberName(key) as PersistentMemberMap;
                if (memberMap == null)
                    yield return key; //if it isn't mapped, we'll persist it anyways...
                else
                    yield return memberMap.Alias;
            }
        }

        /// <summary>
        /// Gets the property type and value.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public override KeyValuePair<Type, object> GetPropertyTypeAndValue(string name)
        {
            if (ClassMap.DiscriminatorAlias == name && ShouldPersistDiscriminator())
                return new KeyValuePair<Type, object>(ClassMap.Discriminator.GetType(), ClassMap.Discriminator);

            object value = null;

            var memberMap = ClassMap.GetMemberMapFromAlias(name);
            if (memberMap != null)
                value = _document[memberMap.MemberName] ?? _document[name];
            else 
                value = _document[name];

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