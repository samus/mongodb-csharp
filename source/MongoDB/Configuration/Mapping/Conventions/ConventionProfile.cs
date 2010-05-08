using System;

namespace MongoDB.Configuration.Mapping.Conventions
{
    /// <summary>
    /// 
    /// </summary>
    public class ConventionProfile
    {
        private IAliasConvention _aliasConvention;
        private ICollectionAdapterConvention _collectionAdapterConvention;
        private ICollectionNameConvention _collectionNameConvention;
        private IDefaultValueConvention _defaultValueConvention;
        private IDiscriminatorConvention _discriminatorConvention;
        private IDiscriminatorAliasConvention _discriminatorAliasConvention;
        private IExtendedPropertiesConvention _extendedPropertiesConvention;
        private IIdConvention _idConvention;
        private IIdGeneratorConvention _idGeneratorConvention;
        private IIdUnsavedValueConvention _idUnsavedValueConvention;

        /// <summary>
        /// Gets or sets the alias convention.
        /// </summary>
        /// <value>The alias convention.</value>
        public IAliasConvention AliasConvention
        {
            get { return _aliasConvention; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                _aliasConvention = value;
            }
        }

        /// <summary>
        /// Gets or sets the collection adapter convention.
        /// </summary>
        /// <value>The collection adapter convention.</value>
        public ICollectionAdapterConvention CollectionAdapterConvention
        {
            get { return _collectionAdapterConvention; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                _collectionAdapterConvention = value;
            }
        }

        /// <summary>
        /// Gets or sets the collection name convention.
        /// </summary>
        /// <value>The collection name convention.</value>
        public ICollectionNameConvention CollectionNameConvention
        {
            get { return _collectionNameConvention; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                _collectionNameConvention = value;
            }
        }

        /// <summary>
        /// Gets or sets the default value convention.
        /// </summary>
        /// <value>The default value convention.</value>
        public IDefaultValueConvention DefaultValueConvention
        {
            get { return _defaultValueConvention; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                _defaultValueConvention = value;
            }
        }

        /// <summary>
        /// Gets or sets the discriminator convention.
        /// </summary>
        /// <value>The discriminator convention.</value>
        public IDiscriminatorConvention DiscriminatorConvention
        {
            get { return _discriminatorConvention; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                _discriminatorConvention = value;
            }
        }

        /// <summary>
        /// Gets or sets the discriminator alias convention.
        /// </summary>
        /// <value>The discriminator alias convention.</value>
        public IDiscriminatorAliasConvention DiscriminatorAliasConvention
        {
            get { return _discriminatorAliasConvention; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                _discriminatorAliasConvention = value;
            }
        }

        /// <summary>
        /// Gets or sets the extended properties convention.
        /// </summary>
        /// <value>The extended properties convention.</value>
        public IExtendedPropertiesConvention ExtendedPropertiesConvention
        {
            get { return this._extendedPropertiesConvention; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                _extendedPropertiesConvention = value;
            }
        }

        /// <summary>
        /// Gets or sets the id convention.
        /// </summary>
        /// <value>The id convention.</value>
        public IIdConvention IdConvention
        {
            get { return _idConvention; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                _idConvention = value;
            }
        }

        /// <summary>
        /// Gets or sets the id generator convention.
        /// </summary>
        /// <value>The id generator convention.</value>
        public IIdGeneratorConvention IdGeneratorConvention
        {
            get { return _idGeneratorConvention; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                _idGeneratorConvention = value;
            }
        }

        /// <summary>
        /// Gets or sets the id unsaved value convention.
        /// </summary>
        /// <value>The id unsaved value convention.</value>
        public IIdUnsavedValueConvention IdUnsavedValueConvention
        {
            get { return _idUnsavedValueConvention; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                _idUnsavedValueConvention = value;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConventionProfile"/> class.
        /// </summary>
        public ConventionProfile()
        {
            _aliasConvention = new DelegateAliasConvention(m => m.Name);
            _collectionNameConvention = new DelegateCollectionNameConvention(t =>  t.Name);
            _collectionAdapterConvention = DefaultCollectionAdapterConvention.Instance;
            _defaultValueConvention = DefaultDefaultValueConvention.Instance;
            _discriminatorConvention = new DelegateDiscriminatorConvention(t => t.Name);
            _discriminatorAliasConvention = new DelegateDiscriminatorAliasConvention(t => "_t");
            _extendedPropertiesConvention = new DelegateExtendedPropertiesConvention(m => m.Name == "ExtendedProperties");
            _idConvention = new DelegateIdConvention(m => m.Name == "Id");
            _idGeneratorConvention = DefaultIdGeneratorConvention.Instance;
            _idUnsavedValueConvention = DefaultIdUnsavedValueConvention.Instance;
        }
    }

}