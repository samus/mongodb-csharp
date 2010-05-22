using System;
using System.Text.RegularExpressions;

namespace MongoDB
{
    /// <summary>
    /// </summary>
    [Serializable]
    public class MongoRegex
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
        public MongoRegex(string expression, string options)
        {
            Expression = expression;
            Options = options;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MongoRegex"/> class.
        /// </summary>
        /// <param name="regex">The regex.</param>
        public MongoRegex(Regex regex)
        {
            if(regex == null)
                throw new ArgumentNullException("regex");

            Expression = regex.ToString();
            Options = string.Empty;

            if((regex.Options & RegexOptions.IgnoreCase) == RegexOptions.IgnoreCase)
                Options += "i";
        }

        /// <summary>
        ///   A valid regex string including the enclosing / characters.
        /// </summary>
        public string Expression { get; set; }

        /// <summary>
        ///   A string that may contain only the characters 'g', 'i', and 'm'. 
        ///   Because the JS and TenGen representations support a limited range of options, 
        ///   any nonconforming options will be dropped when converting to this representation
        /// </summary>
        public string Options { get; set; }

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
            var other = obj as MongoRegex;
            if(other == null)
                return false;

            return Expression == other.Expression && Options == other.Options;
        }

        /// <summary>
        ///   Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        ///   A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            return (Expression ?? string.Empty).GetHashCode()
                   ^ (Options ?? string.Empty).GetHashCode();
        }

        /// <summary>
        ///   Returns a <see cref = "System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        ///   A <see cref = "System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return string.Format("{0}{1}", Expression, Options);
        }
    }
}