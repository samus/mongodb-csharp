using System;
using System.Collections;
using System.Linq;

using MongoDB.Configuration.DictionaryAdapters;


namespace MongoDB.Configuration.Mapping.Model
{
    /// <summary>
    /// 
    /// </summary>
    public class DictionaryMemberMap : PersistentMemberMap
    {
        private readonly IDictionaryAdapter _dictionaryAdapter;
        private readonly Type _valueType;

        public Type ValueType
        {
            get { return _valueType; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DictionaryMemberMap"/> class.
        /// </summary>
        /// <param name="memberName">Name of the member.</param>
        /// <param name="getter">The getter.</param>
        /// <param name="setter">The setter.</param>
        /// <param name="alias">The alias.</param>
        /// <param name="persistNull">if set to <c>true</c> [persist null].</param>
        public DictionaryMemberMap(string memberName, Func<object, object> getter, Action<object, object> setter, string alias, bool persistNull, IDictionaryAdapter dictionaryAdapter, Type valueType)
            : base(memberName, typeof(Document), getter, setter, null, alias, persistNull)
        {
            _dictionaryAdapter = dictionaryAdapter;
            _valueType = valueType;
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <returns></returns>
        public override object GetValue(object instance)
        {
            var value = base.GetValue(instance);
            return _dictionaryAdapter.GetDocument(value, _valueType);
        }

        /// <summary>
        /// Sets the value on the specified instance.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <param name="value">The value.</param>
        public override void SetValue(object instance, object value)
        {
            base.SetValue(instance, _dictionaryAdapter.CreateDictionary(_valueType, (Document)value));
        }
    }
}