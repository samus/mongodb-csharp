using System;

namespace MongoDB
{
    /// <summary>
    /// Mongo Regex options
    /// </summary>
    [Flags]
    public enum MongoRegexOption
    {
        /// <summary>
        /// Specifies that no options are set.
        /// </summary>
        None = 0,
        /// <summary>
        /// i - Specifies case-insensitive matching.
        /// </summary>
        IgnoreCase = 1,
        /// <summary>
        /// m - Multiline mode. Changes the meaning of ^ and $ so they match at the beginning 
        /// and end, respectively, of any line, and not just the beginning and end of the 
        /// entire string. 
        /// </summary>
        Multiline = 2,
        /// <summary>
        /// g - Eliminates unescaped white space from the pattern. 
        /// </summary>
        IgnorePatternWhitespace = 4
    }
}