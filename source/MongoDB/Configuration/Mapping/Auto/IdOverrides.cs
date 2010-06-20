using System.Reflection;
using MongoDB.Configuration.IdGenerators;

namespace MongoDB.Configuration.Mapping.Auto
{
    /// <summary>
    /// 
    /// </summary>
    public class IdOverrides
    {
        /// <summary>
        /// Gets or sets the member.
        /// </summary>
        /// <value>The member.</value>
        public MemberInfo Member { get; set; }

        /// <summary>
        /// Gets or sets the generator.
        /// </summary>
        /// <value>The generator.</value>
        public IIdGenerator Generator { get; set; }

        /// <summary>
        /// Gets or sets the unsaved value.
        /// </summary>
        /// <value>The unsaved value.</value>
        public object UnsavedValue { get; set; }
    }
}