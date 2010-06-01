using System;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace MongoDB
{
    /// <summary>
    /// </summary>
    [Serializable]
    public sealed class MongoRegex : IEquatable<MongoRegex>, IXmlSerializable
    {
        /// <summary>
        ///   Initializes a new instance of the <see cref = "MongoRegex" /> class.
        /// </summary>
        public MongoRegex()
        {
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref = "MongoRegex" /> class.
        /// </summary>
        /// <param name = "expression">The expression.</param>
        public MongoRegex(string expression)
            : this(expression, string.Empty)
        {
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref = "MongoRegex" /> class.
        /// </summary>
        /// <param name = "expression">The expression.</param>
        /// <param name = "options">The options.</param>
        public MongoRegex(string expression, MongoRegexOption options)
        {
            Expression = expression;
            Options = options;
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref = "MongoRegex" /> class.
        /// </summary>
        /// <param name = "regex">The regex.</param>
        public MongoRegex(Regex regex)
        {
            if(regex == null)
                throw new ArgumentNullException("regex");

            Expression = regex.ToString();

            ToggleOption("i", (regex.Options & RegexOptions.IgnoreCase) != 0);
            ToggleOption("m", (regex.Options & RegexOptions.Multiline) != 0);
            ToggleOption("g", (regex.Options & RegexOptions.IgnorePatternWhitespace) != 0);
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref = "MongoRegex" /> class.
        /// </summary>
        /// <param name = "expression">The expression.</param>
        /// <param name = "options">The options.</param>
        public MongoRegex(string expression, string options)
        {
            Expression = expression;
            RawOptions = options;
        }

        /// <summary>
        ///   A valid regex string including the enclosing / characters.
        /// </summary>
        public string Expression { get; set; }

        /// <summary>
        ///   Gets or sets the options.
        /// </summary>
        /// <value>The options.</value>
        public MongoRegexOption Options
        {
            get
            {
                var options = MongoRegexOption.None;

                if(RawOptions != null)
                {
                    if(RawOptions.Contains("i"))
                        options = options | MongoRegexOption.IgnoreCase;
                    if(RawOptions.Contains("m"))
                        options = options | MongoRegexOption.Multiline;
                    if(RawOptions.Contains("g"))
                        options = options | MongoRegexOption.IgnorePatternWhitespace;
                }

                return options;
            }
            set
            {
                ToggleOption("i", (value & MongoRegexOption.IgnoreCase) != 0);
                ToggleOption("m", (value & MongoRegexOption.Multiline) != 0);
                ToggleOption("g", (value & MongoRegexOption.IgnorePatternWhitespace) != 0);
            }
        }

        /// <summary>
        ///   A string that may contain only the characters 'g', 'i', and 'm'. 
        ///   Because the JS and TenGen representations support a limited range of options, 
        ///   any nonconforming options will be dropped when converting to this representation
        /// </summary>
        public string RawOptions { get; set; }

        /// <summary>
        /// Builds a .Net Regex.
        /// </summary>
        /// <returns></returns>
        public Regex BuildRegex()
        {
            var options = RegexOptions.None;

            if(RawOptions != null)
            {
                if(RawOptions.Contains("i"))
                    options = options | RegexOptions.IgnoreCase;
                if(RawOptions.Contains("m"))
                    options = options | RegexOptions.Multiline;
                if(RawOptions.Contains("g"))
                    options = options | RegexOptions.IgnorePatternWhitespace;
            }

            return new Regex(Expression,options);
        }

        /// <summary>
        ///   Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name = "other">An object to compare with this object.</param>
        /// <returns>
        ///   true if the current object is equal to the <paramref name = "other" /> parameter; otherwise, false.
        /// </returns>
        public bool Equals(MongoRegex other)
        {
            if(ReferenceEquals(null, other))
                return false;
            if(ReferenceEquals(this, other))
                return true;
            return Equals(other.Expression, Expression) && Equals(other.RawOptions, RawOptions);
        }

        /// <summary>
        ///   This method is reserved and should not be used. When implementing the IXmlSerializable interface, you should return null (Nothing in Visual Basic) from this method, and instead, if specifying a custom schema is required, apply the <see cref = "T:System.Xml.Serialization.XmlSchemaProviderAttribute" /> to the class.
        /// </summary>
        /// <returns>
        ///   An <see cref = "T:System.Xml.Schema.XmlSchema" /> that describes the XML representation of the object that is produced by the <see cref = "M:System.Xml.Serialization.IXmlSerializable.WriteXml(System.Xml.XmlWriter)" /> method and consumed by the <see cref = "M:System.Xml.Serialization.IXmlSerializable.ReadXml(System.Xml.XmlReader)" /> method.
        /// </returns>
        XmlSchema IXmlSerializable.GetSchema()
        {
            return null;
        }

        /// <summary>
        ///   Generates an object from its XML representation.
        /// </summary>
        /// <param name = "reader">The <see cref = "T:System.Xml.XmlReader" /> stream from which the object is deserialized.</param>
        void IXmlSerializable.ReadXml(XmlReader reader)
        {
            if(reader.MoveToAttribute("options"))
                RawOptions = reader.Value;

            if(reader.IsEmptyElement)
                return;

            Expression = reader.ReadString();
        }

        /// <summary>
        ///   Converts an object into its XML representation.
        /// </summary>
        /// <param name = "writer">The <see cref = "T:System.Xml.XmlWriter" /> stream to which the object is serialized.</param>
        void IXmlSerializable.WriteXml(XmlWriter writer)
        {
            if(RawOptions != null)
                writer.WriteAttributeString("options", RawOptions);

            if(Expression == null)
                return;

            writer.WriteString(Expression);
        }

        /// <summary>
        ///   Toggles the option.
        /// </summary>
        /// <param name = "option">The option.</param>
        /// <param name = "enabled">if set to <c>true</c> [enabled].</param>
        private void ToggleOption(string option, bool enabled)
        {
            if(RawOptions == null)
                RawOptions = string.Empty;

            if(enabled)
            {
                if(RawOptions.Contains(option))
                    return;
                RawOptions += option;
            }
            else
            {
                if(!RawOptions.Contains(option))
                    return;
                RawOptions = RawOptions.Replace(option, string.Empty);
            }
        }

        /// <summary>
        ///   Determines whether the specified <see cref = "System.Object" /> is equal to this instance.
        /// </summary>
        /// <param name = "obj">The <see cref = "System.Object" /> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref = "System.Object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref = "T:System.NullReferenceException">
        ///   The <paramref name = "obj" /> parameter is null.
        /// </exception>
        public override bool Equals(object obj)
        {
            if(ReferenceEquals(null, obj))
                return false;
            if(ReferenceEquals(this, obj))
                return true;
            return obj.GetType() == typeof(MongoRegex) && Equals((MongoRegex)obj);
        }

        /// <summary>
        ///   Implements the operator ==.
        /// </summary>
        /// <param name = "left">The left.</param>
        /// <param name = "right">The right.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator ==(MongoRegex left, MongoRegex right)
        {
            return Equals(left, right);
        }

        /// <summary>
        ///   Implements the operator !=.
        /// </summary>
        /// <param name = "left">The left.</param>
        /// <param name = "right">The right.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator !=(MongoRegex left, MongoRegex right)
        {
            return !Equals(left, right);
        }

        /// <summary>
        ///   Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        ///   A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            unchecked
            {
                return ((Expression != null ? Expression.GetHashCode() : 0)*397) ^ (RawOptions != null ? RawOptions.GetHashCode() : 0);
            }
        }

        /// <summary>
        ///   Returns a <see cref = "System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        ///   A <see cref = "System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return string.Format("{0}{1}", Expression, RawOptions);
        }
    }
}