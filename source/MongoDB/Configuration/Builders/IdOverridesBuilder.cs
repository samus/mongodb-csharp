using System;
using MongoDB.Configuration.Mapping.Auto;
using MongoDB.Configuration.IdGenerators;

namespace MongoDB.Configuration.Builders
{
    /// <summary>
    /// 
    /// </summary>
    public class IdOverridesBuilder
    {
        private readonly IdOverrides _overrides;

        /// <summary>
        /// Initializes a new instance of the <see cref="MemberOverridesBuilder"/> class.
        /// </summary>
        /// <param name="overrides">The overrides.</param>
        internal IdOverridesBuilder(IdOverrides overrides)
        {
            if (overrides == null)
                throw new ArgumentNullException("overrides");

            _overrides = overrides;
        }

        public IdOverridesBuilder GeneratedBy<T>() where T : IIdGenerator, new()
        {
            return GeneratedBy(new T());
        }

        public IdOverridesBuilder GeneratedBy(IIdGenerator generator)
        {
            _overrides.Generator = generator;
            return this;
        }

        public IdOverridesBuilder UnsavedValue(object unsavedValue)
        {
            _overrides.UnsavedValue = unsavedValue;
            return this;
        }
    }
}