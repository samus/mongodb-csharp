using System;

namespace MongoDB.Driver.Configuration.Mapping.Conventions
{
    /// <summary>
    /// 
    /// </summary>
    public class DelegateDiscriminatorAliasConvention : IDiscriminatorAliasConvention
    {
        private readonly Func<Type, string> _discriminatorAlias;

        /// <summary>
        /// Initializes a new instance of the <see cref="DelegateDiscriminatorAliasConvention"/> class.
        /// </summary>
        /// <param name="discriminatorAlias">The discriminator alias.</param>
        public DelegateDiscriminatorAliasConvention(Func<Type, string> discriminatorAlias)
        {
            _discriminatorAlias = discriminatorAlias;
        }

        /// <summary>
        /// Gets the discriminator alias.
        /// </summary>
        /// <param name="classType">Type of the class.</param>
        /// <returns></returns>
        public string GetDiscriminatorAlias(Type classType)
        {
            return _discriminatorAlias(classType);
        }
    }
}