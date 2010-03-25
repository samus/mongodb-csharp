using System;
using System.Collections;

using MongoDB.Driver.Configuration.CollectionAdapters;

namespace MongoDB.Driver.Configuration.Mapping.Model
{
    public class CollectionMemberMap : PersistentMemberMap
    {
        private readonly ICollectionAdapter _collectionType;
        private readonly Type _elementType;

        /// <summary>
        /// Gets the type of the element.
        /// </summary>
        /// <value>The type of the element.</value>
        public Type ElementType
        {
            get { return _elementType; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CollectionMemberMap"/> class.
        /// </summary>
        /// <param name="memberName">Name of the member.</param>
        /// <param name="memberReturnType">Type of the member return.</param>
        /// <param name="getter">The getter.</param>
        /// <param name="setter">The setter.</param>
        /// <param name="alias">The alias.</param>
        /// <param name="persistNull">if set to <c>true</c> [persist null].</param>
        /// <param name="collectionType">Type of the collection.</param>
        /// <param name="elementType">Type of the element.</param>
        /// <param name="elementValueConverter">The element value converter.</param>
        public CollectionMemberMap(string memberName, Type memberReturnType, Func<object, object> getter, Action<object, object> setter, string alias, bool persistNull, ICollectionAdapter collectionType, Type elementType)
            : base(memberName, memberReturnType, getter, setter, null, alias, persistNull)
        {
            _collectionType = collectionType;
            _elementType = elementType;
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <returns></returns>
        public override object GetValue(object instance)
        {
            var elements = _collectionType.GetElementsFromCollection(base.GetValue(instance));
            var list = new ArrayList();
            foreach (var element in elements)
                list.Add(element);

            return list.ToArray();
        }

        public override void SetValue(object instance, object value)
        {
            base.SetValue(instance, _collectionType.CreateCollection(_elementType, (object[])value));
        }
    }
}