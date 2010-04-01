using System.Collections.Generic;

namespace MongoDB.Driver
{
    /// <summary>
    /// Staticly typed way of using MongoDB update modifiers.
    /// </summary>
    public class Mo : Document
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Mo"/> class.
        /// </summary>
        private Mo()
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Mo"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        private Mo(string name,object value){
            Add(name, value);
        }

        /// <summary>
        /// Increments field by the number value. If field is present in the object, 
        /// otherwise sets field to the number value.
        /// </summary>
        /// <param name="field">The field.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static Mo Inc(string field,object value){
            return new Mo("$inc", new Document(field, value));
        }

        /// <summary>
        /// Sets field to value.
        /// </summary>
        /// <param name="field">The field.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        /// <remarks>
        /// All datatypes are supported with $set.
        /// </remarks>
        public static new Mo Set(string field, object value){
            return new Mo("$set", new Document(field, value));
        }

        /// <summary>
        /// Deletes a given field.
        /// </summary>
        /// <param name="field">The field.</param>
        /// <returns></returns>
        /// <remarks>
        /// Supported version in MongoDB 1.3 and up.
        /// </remarks>
        public static Mo Unset(string field)
        {
            return new Mo("$unset", new Document(field, 1));
        }

        /// <summary>
        /// Deletes the given fields.
        /// </summary>
        /// <param name="fields">The fields.</param>
        /// <returns></returns>
        /// <remarks>
        /// Supported version in MongoDB 1.3 and up.
        /// </remarks>
        public static Mo Unset(IEnumerable<string> fields)
        {
            var document = new Document();

            foreach(var field in fields)
                document.Add(field, 1);

            return new Mo("$unset", document);
        }

        /// <summary>
        /// Appends value to field, if field is an existing array.
        /// Otherwise sets field to the array and add value if field is not present. 
        /// If field is present but is not an array, an error condition is raised.
        /// </summary>
        /// <param name="field">The field.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static Mo Push(string field,object value){
            return new Mo("$push", new Document(field, value));
        }

        /// <summary>
        /// Appends each value in values to field,
        /// if field is an existing array. 
        /// Otherwise sets field to the array values if field is not present.
        /// If field is present but is not an array, an error
        /// condition is raised.
        /// </summary>
        /// <param name="field">The field.</param>
        /// <param name="values">The values.</param>
        /// <returns></returns>
        public static Mo PushAll(string field, IEnumerable<object> values){
            return new Mo("$pushAll", new Document(field, values));
        }

        /// <summary>
        /// Adds value to the array only if its not in the array already.
        /// </summary>
        /// <param name="field">The field.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static Mo AddToSet(string field, object value){
            return new Mo("$addToSet", new Document(field, value));
        }

        /// <summary>
        /// Adds values to the array only if its not in the array already.
        /// </summary>
        /// <param name="field">The field.</param>
        /// <param name="values">The values.</param>
        /// <returns></returns>
        public static Mo AddToSet(string field, IEnumerable<object> values){
            return new Mo("$addToSet", new Document(field, new Document("$each", values)));
        }

        /// <summary>
        /// Removes the first element in an array.
        /// </summary>
        /// <param name="field">The field.</param>
        /// <returns></returns>
        /// <remarks>
        /// Supported in MongoDB 1.1 and up.
        /// </remarks>
        public static Mo PopFirst(string field){
            return new Mo("$pop", new Document(field, -1));
        }

        /// <summary>
        /// Removes the last element in an array.
        /// </summary>
        /// <param name="field">The field.</param>
        /// <returns></returns>
        /// <remarks>
        /// Supported in MongoDB 1.1 and up.
        /// </remarks>
        public static Mo PopLast(string field)
        {
            return new Mo("$pop", new Document(field, 1));
        }

        /// <summary>
        /// Removes all occurrences of value from field, if field is an array.
        /// If field is present but is not an array, an error condition is raised.
        /// </summary>
        /// <param name="field">The field.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static Mo Pull(string field, object value){
            return new Mo("$pull", new Document(field, value));
        }

        /// <summary>
        /// Removes all occurrences of each value in values from field,
        /// if field is an array.
        /// If field is present but is not an array, an error condition is raised.
        /// </summary>
        /// <param name="field">The field.</param>
        /// <param name="values">The values.</param>
        /// <returns></returns>
        public static Mo PullAll(string field, IEnumerable<object> values){
            return new Mo("$pullAll", new Document(field, values));
        }

        /// <summary>
        /// Implements the operator &amp;. This is used for conjunctions.
        /// </summary>
        /// <param name="modifier1">The modifier1.</param>
        /// <param name="modifier2">The modifier2.</param>
        /// <returns>The result of the modifier.</returns>
        public static Mo operator &(Mo modifier1, Mo modifier2){
            var mo = new Mo();

            //Todo: move as DeepMerge to Document

            foreach(var key in modifier1.Keys)
                mo[key] = modifier1[key];

            foreach(var pair2 in modifier2)
            {
                object value1;
                if(mo.TryGetValue(pair2.Key, out value1))
                {
                    if(pair2.Value is Document && value1 is Document)
                    {
                        mo[pair2.Key] = new Document()
                            .Merge((Document)value1)
                            .Merge((Document)pair2.Value);
                    }
                    else
                        mo[pair2.Key] = pair2.Value;

                }
                else
                    mo.Add(pair2.Key, pair2.Value);
            }

            return mo;
        }
    }
}