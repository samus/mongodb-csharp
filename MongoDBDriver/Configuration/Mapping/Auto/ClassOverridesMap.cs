using System;
using System.Collections.Generic;

namespace MongoDB.Driver.Configuration.Mapping.Auto
{
    /// <summary>
    /// 
    /// </summary>
    public class ClassOverridesMap
    {
        private readonly Dictionary<Type, ClassOverrides> _overrides;

        /// <summary>
        /// Initializes a new instance of the <see cref="ClassOverridesMap"/> class.
        /// </summary>
        public ClassOverridesMap()
        {
            _overrides = new Dictionary<Type, ClassOverrides>();
        }

        /// <summary>
        /// Gets the class overrides for the type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public ClassOverrides GetOverridesForType(Type type)
        {
            if (type == null)
                throw new ArgumentNullException("type");

            ClassOverrides classOverrides;
            if (!this._overrides.TryGetValue(type, out classOverrides))
                classOverrides = this._overrides[type] = new ClassOverrides();

            return classOverrides;
        }

        /// <summary>
        /// Determines whether [has overrides for type] [the specified type].
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>
        /// 	<c>true</c> if [has overrides for type] [the specified type]; otherwise, <c>false</c>.
        /// </returns>
        public bool HasOverridesForType(Type type)
        {
            return _overrides.ContainsKey(type);
        }
    }
}