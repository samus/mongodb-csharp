namespace MongoDB
{
    /// <summary>
    /// </summary>
    public enum BinarySubtype : byte
    {
        /// <summary>
        /// </summary>
        Unknown = 0,
        /// <summary>
        /// </summary>
        General = 2,
        // Uuid = 3 is now replaced by Guid
        /// <summary>
        /// </summary>
        Md5 = 5,
        /// <summary>
        /// </summary>
        UserDefined = 80
    }
}