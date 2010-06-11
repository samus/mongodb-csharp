namespace MongoDB.Bson
{
    /// <summary>
    /// 
    /// </summary>
    public interface IBsonObjectBuilder
    {
        /// <summary>
        /// Begins the object.
        /// </summary>
        /// <returns></returns>
        object BeginObject();

        /// <summary>
        /// Ends the object.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <returns></returns>
        object EndObject(object instance);

        /// <summary>
        /// Begins the array.
        /// </summary>
        /// <returns></returns>
        object BeginArray();

        /// <summary>
        /// Ends the array.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <returns></returns>
        object EndArray(object instance);

        /// <summary>
        /// Begins the property.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <param name="name">The name.</param>
        void BeginProperty(object instance, string name);

        /// <summary>
        /// Ends the property.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        void EndProperty(object instance, string name, object value);
    }
}