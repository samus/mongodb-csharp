using System;
using System.Configuration;

namespace MongoDB.Driver.Configuration
{
    public class ConnectionCollection : ConfigurationElementCollection
    {

        public override ConfigurationElementCollectionType CollectionType {
            get { return ConfigurationElementCollectionType.AddRemoveClearMap; }
        }

        protected override ConfigurationElement CreateNewElement ()
        {
            return new ConnectionElement ();
        }

        protected override Object GetElementKey (ConfigurationElement element)
        {
            return ((ConnectionElement)element).Name;
        }

        public ConnectionElement this[int index] {
            get { return (ConnectionElement)BaseGet (index); }
            set {
                if (BaseGet (index) != null) {
                    BaseRemoveAt (index);
                }
                BaseAdd (index, value);
            }
        }
        public new ConnectionElement this[string Name] {
            get { return (ConnectionElement)BaseGet (Name); }
        }

        public int IndexOf (ConnectionElement conn)
        {
            return BaseIndexOf (conn);
        }
        public void Add (ConnectionElement conn)
        {
            BaseAdd (conn);
        }
        protected override void BaseAdd (ConfigurationElement element)
        {
            BaseAdd (element, false);
        }

        public void Remove (ConnectionElement conn)
        {
            if (BaseIndexOf (conn) >= 0)
                BaseRemove (conn.Name);
        }

        public void RemoveAt (int index)
        {
            BaseRemoveAt (index);
        }

        public void Remove (string name)
        {
            BaseRemove (name);
        }

        public void Clear ()
        {
            BaseClear ();
        }       
    }
}
