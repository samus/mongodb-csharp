using System;

namespace MongoDB.Driver.Configuration.Mapping.Conventions
{
    public interface IDiscriminatorAliasConvention
    {
        /// <summary>
        /// Gets the name of the discriminator property if one exists.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        string GetDiscriminatorAlias(Type type);
    }
}