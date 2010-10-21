using System;
using System.Linq;

namespace MongoDB.Configuration.Mapping.Model
{
    /// <summary>
    /// 
    /// </summary>
    public class ClassMap : ClassMapBase
    {
        /// <summary>
        ///   Initializes a new instance of the <see cref = "ClassMap&lt;T&gt;" /> class.
        /// </summary>
        public ClassMap(Type type)
            : base(type)
        {
        }

        /// <summary>
        ///   Gets the name of the collection.
        /// </summary>
        /// <value>The name of the collection.</value>
        public override string CollectionName { get; internal set; }

        /// <summary>
        ///   Gets the alias used to store the discriminator.
        /// </summary>
        /// <value>The discriminator alias.</value>
        public override string DiscriminatorAlias { get; internal set; }

        /// <summary>
        ///   Gets the extended properties map.
        /// </summary>
        /// <value>The extended properties map.</value>
        public override ExtendedPropertiesMap ExtendedPropertiesMap { get; internal set; }

        /// <summary>
        ///   Gets the id map.
        /// </summary>
        /// <value>The id map.</value>
        public override IdMap IdMap { get; internal set; }

        /// <summary>
        ///   Gets a value indicating whether this class map is a subclass.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this class map is a subclass; otherwise, <c>false</c>.
        /// </value>
        public override bool IsSubClass
        {
            get { return false; }
        }

    }

    /// <summary>
    /// </summary>
    public class ClassMap<T> : ClassMap where T:new()
    {
        /// <summary>
        ///   Initializes a new instance of the <see cref = "ClassMap&lt;T&gt;" /> class.
        /// </summary>
        public ClassMap()
            : base(typeof(T))
        {
        }

        /// <summary>
        /// Creates an instance of the entity.
        /// </summary>
        /// <returns>a new entity instance</returns>
        public override object CreateInstance()
        {
            T instance = new T();

            //initialize all default values in case something isn't specified when reader the document.
            foreach (var memberMap in MemberMaps.Where(x => x.DefaultValue != null))
                memberMap.SetValue(instance, memberMap.DefaultValue);
            return instance;
        }
    }
}