namespace MongoDB.Configuration.Mapping.Auto
{
    ///<summary>
    ///</summary>
    public class MemberOverrides
    {
        /// <summary>
        /// Gets or sets the alias to use for the member.
        /// </summary>
        /// <value>The alias.</value>
        public string Alias { get; set; }

        /// <summary>
        /// Gets or sets the default value.
        /// </summary>
        /// <value>The default value.</value>
        public object DefaultValue { get; set; }

        /// <summary>
        /// Gets or sets a value whether the member should be ignored from the map.
        /// </summary>
        /// <value><c>true</c> if exclude; otherwise, <c>false</c>.</value>
        public bool? Ignore { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether a member with the default value gets persisted.
        /// </summary>
        /// <value>The persist default value.</value>
        public bool? PersistDefaultValue { get; set; }
    }
}