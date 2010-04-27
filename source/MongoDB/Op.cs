using System;
using MongoDB.Bson;

namespace MongoDB
{
    /// <summary>
    /// Staticly typed way of using MongoDB query operators.
    /// </summary>
    public class Op : Document
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Op"/> class.
        /// </summary>
        /// <remarks>Only allow instantiation through static methods.</remarks>
        private Op()
        { }

        /// <summary>
        /// Matches an object which is greater than the specified value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static Op GreaterThan<T>(T value)
        {
            return (Op)new Op().Add("$gt", value);
        }

        /// <summary>
        /// Matches an object which is greater than or equal to the specified value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static Op GreaterThanOrEqual<T>(T value)
        {
            return (Op)new Op().Add("$gte", value);
        }

        /// <summary>
        /// Matches an object which is less than the specified value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static Op LessThan<T>(T value)
        {
            return (Op)new Op().Add("$lt", value);
        }

        /// <summary>
        /// Matches an object which is less than or equal to the specified value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static Op LessThanOrEqual<T>(T value)
        {
            return (Op)new Op().Add("$lte", value);
        }

        /// <summary>
        /// Matches an object which does not equal the specified value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static Op NotEqual<T>(T value)
        {
            return (Op)new Op().Add("$ne", value);
        }

        /// <summary>
        /// Matches an array which has one of the specified values.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="values">The values.</param>
        /// <returns></returns>
        public static Op In<T>(params T[] values)
        {
            return (Op)new Op().Add("$in", values);
        }

        /// <summary>
        /// Matches an array which does not have any of the specified values.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="values">The values.</param>
        /// <returns></returns>
        public static Op NotIn<T>(params T[] values)
        {
            return (Op)new Op().Add("$nin", values);
        }

        /// <summary>
        /// Matches an array which has all of the specified values.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="values">The values.</param>
        /// <returns></returns>
        public static Op All<T>(params T[] values)
        {
            return (Op)new Op().Add("$all", values);
        }

        /// <summary>
        /// Modulus operator.
        /// </summary>
        /// <param name="denominator">The denominator.</param>
        /// <param name="result">The result.</param>
        /// <returns></returns>
        public static Op Mod(int denominator, int result)
        {
            return (Op)new Op().Add("$mod", new[] { denominator, result });
        }

        /// <summary>
        /// Matches any array with the specified number of elements
        /// </summary>
        /// <param name="size">The size.</param>
        /// <returns></returns>
        public static Op Size(int size)
        {
            return (Op)new Op().Add("$size", size);
        }

        /// <summary>
        /// Check for existence of a field.
        /// </summary>
        /// <returns></returns>
        public static Op Exists()
        {
            return (Op)new Op().Add("$exists", true);
        }

        /// <summary>
        /// Check for lack of existence of a field.
        /// </summary>
        /// <returns></returns>
        public static Op NotExists()
        {
            return (Op)new Op().Add("$exists", false);
        }

        /// <summary>
        /// Matches values based on their bson type.
        /// </summary>
        /// <param name="bsonType">Type of the bson.</param>
        /// <returns></returns>
        public static Op Type(BsonDataType bsonType)
        {
            return (Op)new Op().Add("$type", (int)bsonType);
        }

        /// <summary>
        /// Sends the Javascript expressiosn to the server.
        /// </summary>
        /// <param name="javascript">The javascript.</param>
        /// <returns></returns>
        public static Op Where(string javascript)
        {
            if(javascript == null)
                throw new ArgumentNullException("javascript");

            return (Op)new Op().Add("$where", new Code(javascript));
        }

        /// <summary>
        /// Implements the operator &amp;.  This is used for conjunctions.
        /// </summary>
        /// <param name="op1">The op1.</param>
        /// <param name="op2">The op2.</param>
        /// <returns>The result of the operator.</returns>
        public static Op operator &(Op op1, Op op2)
        {
            return (Op)new Op().Merge(op1).Merge(op2);
        }

        /// <summary>
        /// Implements the operator !. This is used for the meta operator $not.
        /// </summary>
        /// <param name="op">The op.</param>
        /// <returns>The result of the operator.</returns>
        public static Op operator !(Op op)
        {
            return (Op)new Op().Add("$not", op);
        }
    }
}