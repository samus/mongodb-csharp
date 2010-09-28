using System;
using System.Collections;

using MongoDB.Configuration.CollectionAdapters;

namespace MongoDB.Configuration.Mapping.Model
{
    /// <summary>
    /// 
    /// </summary>
    public class CollectionMemberMap : PersistentMemberMap
    {
        private readonly ICollectionAdapter _collectionAdapter;

        /// <summary>
        /// Gets the type of the element.
        /// </summary>
        /// <value>The type of the element.</value>
        public Type ElementType { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CollectionMemberMap"/> class.
        /// </summary>
        /// <param name="memberName">Name of the member.</param>
        /// <param name="memberReturnType">Type of the member return.</param>
        /// <param name="getter">The getter.</param>
        /// <param name="setter">The setter.</param>
        /// <param name="alias">The alias.</param>
        /// <param name="persistDefaultValue">if set to <c>true</c> [persist default value].</param>
        /// <param name="collectionAdapter">Type of the collection.</param>
        /// <param name="elementType">Type of the element.</param>
        public CollectionMemberMap(string memberName, Type memberReturnType, Func<object, object> getter, Action<object, object> setter, string alias, bool persistDefaultValue, ICollectionAdapter collectionAdapter, Type elementType)
            : base(memberName, memberReturnType, getter, setter, null, alias, persistDefaultValue)
        {
            _collectionAdapter = collectionAdapter;
            ElementType = elementType;
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <returns></returns>
        public override object GetValue(object instance)
        {
            var elements = _collectionAdapter.GetElementsFromCollection(base.GetValue(instance));

            if(elements == null)
                return null;

            var list = new ArrayList();
            
            foreach (var element in elements)
                list.Add(element);

            return list.ToArray();
        }

        /// <summary>
        /// Sets the value on the specified instance.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <param name="value">The value.</param>
        public override void SetValue(object instance, object value)
        {
            base.SetValue(instance, _collectionAdapter.CreateCollection(ElementType, (object[])value));
        }
    }
}