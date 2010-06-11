using System;
using MongoDB.Configuration.IdGenerators;

namespace MongoDB.Configuration.Mapping.Conventions
{
    /// <summary>
    /// 
    /// </summary>
    public class DefaultIdGeneratorConvention : IIdGeneratorConvention
    {
        ///<summary>
        ///</summary>
        public static readonly DefaultIdGeneratorConvention Instance = new DefaultIdGeneratorConvention();

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultIdGeneratorConvention"/> class.
        /// </summary>
        private DefaultIdGeneratorConvention()
        { }

        /// <summary>
        /// Gets the generator.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public IIdGenerator GetGenerator(Type type)
        {
            if (type == typeof(Oid))
                return new OidGenerator();

            if (type == typeof(Guid))
                return new GuidCombGenerator();

            return new AssignedIdGenerator();
        }
    }
}