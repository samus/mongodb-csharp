using System;
using System.Collections.Generic;
using MongoDB.Configuration.Mapping.Model;
using MongoDB.Configuration.Mapping;
using MongoDB.Bson;

namespace MongoDB.Serialization.Descriptors
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
        public override IEnumerable<BsonProperty> GetProperties()
        {
            if(ShouldAddDiscriminator())
            {
                if (_document.ContainsKey("count")) //this is a special case
                {
                    var queryDoc = _document["query"] as Document;
                    if (queryDoc == null)
                    {
                        //TODO: implement someway of shoving this value into the doc...
                        throw new NotSupportedException("Count queries on subclasses using anonymous types is not supported.");
                    }
                    else if (!queryDoc.ContainsKey(ClassMap.DiscriminatorAlias))
                        queryDoc.Append(ClassMap.DiscriminatorAlias, ClassMap.Discriminator);
                }
                else
                    yield return CreateProperty(ClassMap.DiscriminatorAlias, ClassMap.Discriminator.GetType(), ClassMap.Discriminator, false);
            }

            foreach (string key in _document.Keys)
            {
                var alias = GetAliasFromMemberName(key);
                var valueAndType = GetValue(key);
                if (alias.MemberMap != null && !alias.MemberMap.PersistDefaultValue && object.Equals(alias.MemberMap.DefaultValue, valueAndType.Value))
                    continue;
                yield return CreateProperty(alias.Alias, valueAndType);
            }
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        private BsonPropertyValue GetValue(string name)
        {
            if (ClassMap.DiscriminatorAlias == name && ShouldAddDiscriminator())
                return new BsonPropertyValue(ClassMap.Discriminator.GetType(), ClassMap.Discriminator, false);

            var value = _document[name];
            if (value != null && typeof(Code).IsAssignableFrom(value.GetType()))
            {
                Code code = (Code)value;
                code.Value = TranslateJavascript(code.Value);
                return new BsonPropertyValue(typeof(Code), code, false);
            }

            var memberMap = GetAliasFromMemberName(name).MemberMap;
            var type = typeof(Document);
            bool isDictionary = false;

            if (memberMap != null)
            {
                type = memberMap.MemberReturnType;
                if (memberMap is CollectionMemberMap)
                    type = ((CollectionMemberMap)memberMap).ElementType;
                else if (memberMap is DictionaryMemberMap)
                {
                    type = ((DictionaryMemberMap)memberMap).ValueType;
                    isDictionary = true;
                }
            }
            else if (name.StartsWith("$") || name == "query" || name == "orderby") //we are a modifier, a special case of querying, or order fields
                type = ClassMap.ClassType; //we'll pass this along so that the fields get replaced correctly...
            else if (value != null)
                type = value.GetType();

            return new BsonPropertyValue(type, value, isDictionary);
        }
    }
}