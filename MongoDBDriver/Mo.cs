namespace MongoDB.Driver
{
    /// <summary>
    /// Modifiers
    /// </summary>
    public class Mo : Document
    {
        private Mo()
        { }

        private Mo(string name,object value){
            Add(name, value);
        }

        /// <summary>
        /// Increments field by the number value. 
        /// </summary>
        /// <remarks>
        /// If field is present in the object, otherwise sets field to the number value.
        /// </remarks>
        /// <param name="field">The field.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static Mo Inc(string field,object value){
            return new Mo("$inc", new Document(field, value));
        }

        /// <summary>
        /// Increments the fields from an Document or an anonymous type 
        /// by its values. 
        /// </summary>
        /// <remarks>
        /// If field is present in the object, otherwise sets field to the number value.
        /// </remarks>
        /// <param name="value">Document or anonymous type.</param>
        /// <returns></returns>
        public static Mo Inc(object value){
            return new Mo("$inc", value);
        }

        /// <summary>
        /// Implements the operator &amp;.  This is used for conjunctions.
        /// </summary>
        /// <param name="modifier1">The modifier1.</param>
        /// <param name="modifier2">The modifier2.</param>
        /// <returns>The result of the modifier.</returns>
        public static Mo operator &(Mo modifier1, Mo modifier2){
            var mo = new Mo();

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