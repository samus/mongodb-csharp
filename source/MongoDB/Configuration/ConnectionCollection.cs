using System;
using System.Configuration;

namespace MongoDB.Configuration
{
    /// <summary>
    /// 
    /// </summary>
    public class ConnectionCollection : ConfigurationElementCollection
    {
        /// <summary>
        /// Gets the type of the <see cref="T:System.Configuration.ConfigurationElementCollection"/>.
        /// </summary>
        /// <value></value>
        /// <returns>
        /// The <see cref="T:System.Configuration.ConfigurationElementCollectionType"/> of this collection.
        /// </returns>
        public override ConfigurationElementCollectionType CollectionType {
            get { return ConfigurationElementCollectionType.AddRemoveClearMap; }
        }

        /// <summary>
        /// When overridden in a derived class, creates a new <see cref="T:System.Configuration.ConfigurationElement"/>.
        /// </summary>
        /// <returns>
        /// A new <see cref="T:System.Configuration.ConfigurationElement"/>.
        /// </returns>
        protected override ConfigurationElement CreateNewElement ()
        {
            return new ConnectionElement ();
        }

        /// <summary>
        /// Gets the element key for a specified configuration element when overridden in a derived class.
        /// </summary>
        /// <param name="element">The <see cref="T:System.Configuration.ConfigurationElement"/> to return the key for.</param>
        /// <returns>
        /// An <see cref="T:System.Object"/> that acts as the key for the specified <see cref="T:System.Configuration.ConfigurationElement"/>.
        /// </returns>
        protected override Object GetElementKey (ConfigurationElement element)
        {
            return ((ConnectionElement)element).Name;
        }

        /// <summary>
        /// Gets or sets the <see cref="MongoDB.Configuration.ConnectionElement"/> at the specified index.
        /// </summary>
        /// <value></value>
        public ConnectionElement this[int index] {
            get { return (ConnectionElement)BaseGet (index); }
            set {
                if (BaseGet (index) != null) {
                    BaseRemoveAt (index);
                }
                BaseAdd (index, value);
            }
        }

        /// <summary>
        /// Gets the <see cref="MongoDB.Configuration.ConnectionElement"/> with the specified name.
        /// </summary>
        /// <value></value>
        public new ConnectionElement this[string Name] {
            get { return (ConnectionElement)BaseGet (Name); }
        }

        /// <summary>
        /// Indexes the of.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <returns></returns>
        public int IndexOf (ConnectionElement connection)
        {
            return BaseIndexOf (connection);
        }

        /// <summary>
        /// Adds the specified connection.
        /// </summary>
        /// <param name="connection">The connection.</param>
        public void Add (ConnectionElement connection)
        {
            BaseAdd (connection);
        }

        /// <summary>
        /// Adds a configuration element to the <see cref="T:System.Configuration.ConfigurationElementCollection"/>.
        /// </summary>
        /// <param name="element">The <see cref="T:System.Configuration.ConfigurationElement"/> to add.</param>
        protected override void BaseAdd (ConfigurationElement element)
        {
            BaseAdd (element, false);
        }

        /// <summary>
        /// Removes the specified connection.
        /// </summary>
        /// <param name="connection">The connection.</param>
        public void Remove (ConnectionElement connection)
        {
            if (BaseIndexOf (connection) >= 0)
                BaseRemove (connection.Name);
        }

        /// <summary>
        /// Removes at.
        /// </summary>
        /// <param name="index">The index.</param>
        public void RemoveAt (int index)
        {
            BaseRemoveAt (index);
        }

        /// <summary>
        /// Removes the specified name.
        /// </summary>
        /// <param name="name">The name.</param>
        public void Remove (string name)
        {
            BaseRemove (name);
        }

        /// <summary>
        /// Clears this instance.
        /// </summary>
        public void Clear ()
        {
            BaseClear ();
        }       
    }
}
