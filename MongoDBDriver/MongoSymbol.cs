using System;

namespace MongoDB.Driver
{
    /// <summary>
    ///   Type to hold an interned string that maps to the bson symbol type.
    /// </summary>
    public struct MongoSymbol : IEquatable<MongoSymbol>, IEquatable<String>, IComparable<MongoSymbol>, IComparable<String>
    {
        /// <summary>
        /// Gets or sets the empty.
        /// </summary>
        /// <value>The empty.</value>
        public static MongoSymbol Empty { get; private set; }

        /// <summary>
        /// Initializes the <see cref="MongoSymbol"/> struct.
        /// </summary>
        static MongoSymbol(){
            Empty = new MongoSymbol();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MongoSymbol"/> struct.
        /// </summary>
        /// <param name="value">The value.</param>
        public MongoSymbol(string value)
            : this(){
            if(string.IsNullOrEmpty(value))
                throw new ArgumentNullException("value");

            Value = String.Intern(value);
        }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>The value.</value>
        public string Value { get; private set; }

        /// <summary>
        /// Compares the current object with another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        /// A 32-bit signed integer that indicates the relative order of the objects being compared. The return value has the following meanings:
        /// Value
        /// Meaning
        /// Less than zero
        /// This object is less than the <paramref name="other"/> parameter.
        /// Zero
        /// This object is equal to <paramref name="other"/>.
        /// Greater than zero
        /// This object is greater than <paramref name="other"/>.
        /// </returns>
        public int CompareTo(MongoSymbol other){
            return Value.CompareTo(other.Value);
        }

        /// <summary>
        /// Compares the current object with another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        /// A 32-bit signed integer that indicates the relative order of the objects being compared. The return value has the following meanings:
        /// Value
        /// Meaning
        /// Less than zero
        /// This object is less than the <paramref name="other"/> parameter.
        /// Zero
        /// This object is equal to <paramref name="other"/>.
        /// Greater than zero
        /// This object is greater than <paramref name="other"/>.
        /// </returns>
        public int CompareTo(string other){
            return Value.CompareTo(other);
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        public bool Equals(MongoSymbol other){
            return Value.Equals(other.Value);
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        public bool Equals(string other){
            return Value.Equals(other);
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString(){
            return Value;
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object"/> to compare with this instance.</param>
        /// <returns>
        /// 	<c>true</c> if the specified <see cref="System.Object"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj){
            return Value.Equals(obj);
        }

        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="a">A.</param>
        /// <param name="b">The b.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator ==(MongoSymbol a, MongoSymbol b){
            return SymbolEqual(a.Value, b.Value);
        }

        /*
        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="a">A.</param>
        /// <param name="b">The b.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator ==(MongoSymbol a, string b){
            return SymbolEqual(a.Value, b);
        }*/

        /*
        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="a">A.</param>
        /// <param name="b">The b.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator ==(string a, MongoSymbol b){
            return SymbolEqual(a, b.Value);
        }*/

        /// <summary>
        /// Implements the operator !=.
        /// </summary>
        /// <param name="a">A.</param>
        /// <param name="b">The b.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator !=(MongoSymbol a, MongoSymbol b){
            return !(a == b);
        }

        /*
        /// <summary>
        /// Implements the operator !=.
        /// </summary>
        /// <param name="a">A.</param>
        /// <param name="b">The b.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator !=(MongoSymbol a, String b){
            return !(a == b);
        }*/

        /*
        /// <summary>
        /// Implements the operator !=.
        /// </summary>
        /// <param name="a">A.</param>
        /// <param name="b">The b.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator !=(string a, MongoSymbol b){
            return !(a == b);
        }*/

        /// <summary>
        /// Performs an implicit conversion from <see cref="MongoDB.Driver.MongoSymbol"/> to <see cref="System.String"/>.
        /// </summary>
        /// <param name="s">The s.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator string(MongoSymbol s){
            return s.Value;
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="System.String"/> to <see cref="MongoDB.Driver.MongoSymbol"/>.
        /// </summary>
        /// <param name="s">The s.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator MongoSymbol(string s){
            return new MongoSymbol(s);
        }

        /// <summary>
        /// Determines whether the specified s is empty.
        /// </summary>
        /// <param name="s">The s.</param>
        /// <returns>
        /// 	<c>true</c> if the specified s is empty; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsEmpty(MongoSymbol s){
            return s == Empty;
        }

        /// <summary>
        /// Symbols the equal.
        /// </summary>
        /// <param name="a">A.</param>
        /// <param name="b">The b.</param>
        /// <returns></returns>
        private static bool SymbolEqual(string a, string b){
            return a == b;
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode(){
            return Value.GetHashCode();
        }
    }
}