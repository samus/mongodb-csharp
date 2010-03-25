using System;

namespace MongoDB.Driver.Configuration.Mapping.Conventions
{
    /// <summary>
    /// 
    /// </summary>
    public class DelegateCollectionNameConvention : ICollectionNameConvention
    {
        private readonly Func<Type, string> _collectionName;

        /// <summary>
        /// Initializes a new instance of the <see cref="DelegateCollectionNameConvention"/> class.
        /// </summary>
        /// <param name="collectionName">Name of the collection.</param>
        public DelegateCollectionNameConvention(Func<Type, string> collectionName)
        {
            _collectionName = collectionName;
        }

        /// <summary>
        /// Gets the name of the collection.
        /// </summary>
        /// <param name="classType">Type of the entity.</param>
        /// <returns></returns>
        public string GetCollectionName(Type classType)
        {
            return _collectionName(classType);
        }
    }
}