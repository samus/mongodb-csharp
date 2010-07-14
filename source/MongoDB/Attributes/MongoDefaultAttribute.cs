using System;

namespace MongoDB.Attributes
{
    /// <summary>
    /// 
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class MongoDefaultAttribute : Attribute
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="MongoDefaultAttribute"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        public MongoDefaultAttribute(object value)
            : this(value, true)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="MongoDefaultAttribute"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        public MongoDefaultAttribute(object value, bool persistDefaultValue)
        {
            Value = value;
            PersistDefaultValue = persistDefaultValue;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the default value should be persisted.
        /// </summary>
        /// <value><c>true</c> if [persist default value]; otherwise, <c>false</c>.</value>
        public bool PersistDefaultValue { get; private set; }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>The value.</value>
        public object Value { get; private set; }
    }
}