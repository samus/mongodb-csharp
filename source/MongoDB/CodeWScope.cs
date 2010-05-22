using System;

namespace MongoDB
{
    /// <summary>
    /// </summary>
    [Serializable]
    public class CodeWScope : IEquatable<CodeWScope>
    {
        /// <summary>
        ///   Initializes a new instance of the <see cref = "CodeWScope" /> class.
        /// </summary>
        public CodeWScope()
        {
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref = "CodeWScope" /> class.
        /// </summary>
        /// <param name = "code">The code.</param>
        public CodeWScope(String code)
            : this(code, new Document())
        {
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref = "CodeWScope" /> class.
        /// </summary>
        /// <param name = "code">The code.</param>
        /// <param name = "scope">The scope.</param>
        public CodeWScope(String code, Document scope)
        {
            Value = code;
            Scope = scope;
        }

        /// <summary>
        ///   Gets or sets the value.
        /// </summary>
        /// <value>The value.</value>
        public string Value { get; set; }

        /// <summary>
        ///   Gets or sets the scope.
        /// </summary>
        /// <value>The scope.</value>
        public Document Scope { get; set; }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        public bool Equals(CodeWScope other)
        {
            if(ReferenceEquals(null, other))
                return false;
            if(ReferenceEquals(this, other))
                return true;
            return Equals(other.Value, Value) && Equals(other.Scope, Scope);
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object"/> to compare with this instance.</param>
        /// <returns>
        /// 	<c>true</c> if the specified <see cref="System.Object"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="T:System.NullReferenceException">
        /// The <paramref name="obj"/> parameter is null.
        /// </exception>
        public override bool Equals(object obj)
        {
            if(ReferenceEquals(null, obj))
                return false;
            if(ReferenceEquals(this, obj))
                return true;
            return obj.GetType() == typeof(CodeWScope) && Equals((CodeWScope)obj);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            unchecked
            {
                return ((Value != null ? Value.GetHashCode() : 0)*397) ^ (Scope != null ? Scope.GetHashCode() : 0);
            }
        }

        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator ==(CodeWScope left, CodeWScope right)
        {
            return Equals(left, right);
        }

        /// <summary>
        /// Implements the operator !=.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator !=(CodeWScope left, CodeWScope right)
        {
            return !Equals(left, right);
        }
    }
}