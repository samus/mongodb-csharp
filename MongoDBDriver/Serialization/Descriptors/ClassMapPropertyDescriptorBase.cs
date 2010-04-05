using System;
using System.Collections.Generic;
using MongoDB.Driver.Configuration.Mapping.Model;

namespace MongoDB.Driver.Serialization.Descriptors
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class ClassMapPropertyDescriptorBase : IPropertyDescriptor
    {
        /// <summary>
        /// 
        /// </summary>
        protected readonly IClassMap ClassMap;

        /// <summary>
        /// Initializes a new instance of the <see cref="ClassMapDescriptorBase"/> class.
        /// </summary>
        /// <param name="classMap">The class map.</param>
        protected ClassMapPropertyDescriptorBase(IClassMap classMap)
        {
            if (classMap == null)
                throw new ArgumentNullException("classMap");

            ClassMap = classMap;
        }

        /// <summary>
        /// Gets the property names.
        /// </summary>
        /// <returns></returns>
        public abstract IEnumerable<string> GetPropertyNames();

        /// <summary>
        /// Gets the property type and value.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public abstract KeyValuePair<Type, object> GetPropertyTypeAndValue(string name);

        /// <summary>
        /// Shoulds the persist discriminator.
        /// </summary>
        /// <returns></returns>
        protected bool ShouldPersistDiscriminator()
        {
            return (ClassMap.IsPolymorphic && ClassMap.HasDiscriminator) || ClassMap.IsSubClass;
        }
    }
}