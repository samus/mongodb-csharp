using System;

namespace MongoDB.Driver.Configuration.Mapping.Conventions
{
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

        public ConventionProfile()
        {
            _aliasConvention = new DelegateAliasConvention(m => m.Name);
            _collectionNameConvention = new DelegateCollectionNameConvention(delegate(Type t) { return t.Name; });
            _collectionAdapterConvention = DefaultCollectionAdapterConvention.Instance;
            _defaultValueConvention = DefaultDefaultValueConvention.Instance;
            _discriminatorConvention = new DelegateDiscriminatorConvention(delegate(Type t) { return t.Name; });
            _discriminatorAliasConvention = new DelegateDiscriminatorAliasConvention(delegate(Type t) { return "_t"; });
            _extendedPropertiesConvention = new NamedExtendedPropertiesConvention("ExtendedProperties");
            _idConvention = new NamedIdConvention("Id");
            _idGeneratorConvention = DefaultIdGeneratorConvention.Instance;
            _idUnsavedValueConvention = DefaultIdUnsavedValueConvention.Instance;
        }
    }

}