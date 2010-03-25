using System;

namespace MongoDB.Driver.Bson
{
    /// <summary>
    /// 
    /// </summary>
    public class BsonWriterSettings
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BsonWriterSettings"/> class.
        /// </summary>
        public BsonWriterSettings()
            : this(new BsonDocumentDescriptor())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BsonWriterSettings"/> class.
        /// </summary>
        /// <param name="descriptor">The descriptor.</param>
        public BsonWriterSettings(IBsonObjectDescriptor descriptor){
            if(descriptor == null)
                throw new ArgumentNullException("descriptor");

            Descriptor = descriptor;
        }

        /// <summary>
        /// Gets or sets the descriptor.
        /// </summary>
        /// <value>The descriptor.</value>
        public IBsonObjectDescriptor Descriptor { get; private set; }

        /// <summary>
        /// MongoDB stores all time values in UTC timezone. If true 
        /// time values are not converterd to UTC.
        /// </summary>
        /// <value><c>true</c> if [write as local time]; otherwise, <c>false</c>.</value>
        public bool WriteLocalTime { get; set; }
    }
}