using System;

namespace MongoDB.Driver.Serialization.Handlers
{
    public interface IBsonBuilderHandler
    {
        /// <summary>
        /// Completes this instance.
        /// </summary>
        /// <returns></returns>
        object Complete();

        /// <summary>
        /// Begins the property.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        Type BeginProperty(string name);

        /// <summary>
        /// Ends the property.
        /// </summary>
        /// <param name="value">The value.</param>
        void EndProperty(object value);
    }
}