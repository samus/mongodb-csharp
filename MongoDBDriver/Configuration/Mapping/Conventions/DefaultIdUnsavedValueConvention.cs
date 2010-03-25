using System;

namespace MongoDB.Driver.Configuration.Mapping.Conventions
{
    /// <summary>
    /// 
    /// </summary>
    public class DefaultIdUnsavedValueConvention : IIdUnsavedValueConvention
    {
        /// <summary>
        /// 
        /// </summary>
        public static readonly DefaultIdUnsavedValueConvention Instance = new DefaultIdUnsavedValueConvention();

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultIdUnsavedValueConvention"/> class.
        /// </summary>
        private DefaultIdUnsavedValueConvention()
        { }

        /// <summary>
        /// Gets the unsaved value.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public object GetUnsavedValue(Type type)
        {
            return type.IsValueType ? Activator.CreateInstance(type) : null;
        }
    }
}