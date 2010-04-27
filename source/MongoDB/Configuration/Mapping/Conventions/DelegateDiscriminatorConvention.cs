using System;

namespace MongoDB.Configuration.Mapping.Conventions
{
    /// <summary>
    /// 
    /// </summary>
    public class DelegateDiscriminatorConvention : IDiscriminatorConvention
    {
        private readonly Func<Type, object> _discriminator;

        /// <summary>
        /// Initializes a new instance of the <see cref="DelegateDiscriminatorConvention"/> class.
        /// </summary>
        /// <param name="discriminator">The discriminator.</param>
        public DelegateDiscriminatorConvention(Func<Type, object> discriminator)
        {
            _discriminator = discriminator;
        }

        /// <summary>
        /// Gets the discriminator.
        /// </summary>
        /// <param name="classType">Type of the entity.</param>
        /// <returns></returns>
        public object GetDiscriminator(Type classType)
        {
            return _discriminator(classType);
        }
    }
}
