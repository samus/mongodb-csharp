using System;

namespace MongoDB.Configuration.Mapping.Conventions
{
    /// <summary>
    /// 
    /// </summary>
    public interface IDiscriminatorConvention
    {
        /// <summary>
        /// Gets the discriminator if one exists.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        object GetDiscriminator(Type type);
    }
}