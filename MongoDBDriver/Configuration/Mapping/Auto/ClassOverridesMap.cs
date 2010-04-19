using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MongoDB.Driver.Configuration.Mapping.Auto
{
    public class ClassOverridesMap
    {
        private Dictionary<Type, ClassOverrides> _overrides;

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