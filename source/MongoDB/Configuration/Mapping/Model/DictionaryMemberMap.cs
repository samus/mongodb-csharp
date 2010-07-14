using System;
using MongoDB.Configuration.DictionaryAdapters;


namespace MongoDB.Configuration.Mapping.Model
{
    /// <summary>
    /// 
    /// </summary>
    public class DictionaryMemberMap : PersistentMemberMap
    {
        private readonly IDictionaryAdapter _dictionaryAdapter;

        /// <summary>
        /// Gets the type of the key.
        /// </summary>
        /// <value>The type of the key.</value>
        public Type KeyType
        {
            get { return _dictionaryAdapter.KeyType; }
        }

        /// <summary>
        /// Gets the type of the value.
        /// </summary>
        /// <value>The type of the value.</value>
        public Type ValueType
        {
            get { return _dictionaryAdapter.ValueType; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DictionaryMemberMap"/> class.
        /// </summary>
        /// <param name="memberName">Name of the member.</param>
        /// <param name="getter">The getter.</param>
        /// <param name="setter">The setter.</param>
        /// <param name="alias">The alias.</param>
        /// <param name="persistDefaultValue">if set to <c>true</c> [persist default value].</param>
        /// <param name="dictionaryAdapter">The dictionary adapter.</param>
        public DictionaryMemberMap(string memberName, Func<object, object> getter, Action<object, object> setter, string alias, bool persistDefaultValue, IDictionaryAdapter dictionaryAdapter)
            : base(memberName, typeof(Document), getter, setter, null, alias, persistDefaultValue)
        {
            _dictionaryAdapter = dictionaryAdapter;
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <returns></returns>
        public override object GetValue(object instance)
        {
            var value = base.GetValue(instance);
            return _dictionaryAdapter.GetDocument(value);
        }

        /// <summary>
        /// Sets the value on the specified instance.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <param name="value">The value.</param>
        public override void SetValue(object instance, object value)
        {
            base.SetValue(instance, _dictionaryAdapter.CreateDictionary((Document)value));
        }
    }
}