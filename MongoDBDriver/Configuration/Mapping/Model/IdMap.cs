using System;

using MongoDB.Driver.Configuration.IdGenerators;

namespace MongoDB.Driver.Configuration.Mapping.Model
{
    public sealed class IdMap : PersistentMemberMap
    {
        private readonly IIdGenerator _generator;
        private readonly object _unsavedValue;

        /// <summary>
        /// Gets the id's unsaved value.
        /// </summary>
        /// <value>The unsaved value.</value>
        public object UnsavedValue
        {
            get { return _unsavedValue; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IdMap"/> class.
        /// </summary>
        /// <param name="memberName">Name of the member.</param>
        /// <param name="memberType">Type of the member.</param>
        /// <param name="getter">The getter.</param>
        /// <param name="setter">The setter.</param>
        /// <param name="generator">The generator.</param>
        /// <param name="unsavedValue">The unsaved value.</param>
        /// <param name="valueConverter">The value converter.</param>
        public IdMap(string memberName, Type memberType, Func<object, object> getter, Action<object, object> setter, IIdGenerator generator, object unsavedValue)
            : base(memberName, memberType, getter, setter, null, "_id", true)
        {
            _generator = generator;
            _unsavedValue = unsavedValue;
        }

        /// <summary>
        /// Generates the specified entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns></returns>
        public object Generate(object entity)
        {
            return _generator.Generate(entity, this);
        }
    }
}