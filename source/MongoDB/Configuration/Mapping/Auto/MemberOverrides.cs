namespace MongoDB.Driver.Configuration.Mapping.Auto
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
        /// Gets or sets a value indicating whether.
        /// </summary>
        /// <value><c>true</c> if [persist if null]; otherwise, <c>false</c>.</value>
        public bool? PersistIfNull { get; set; }
    }
}