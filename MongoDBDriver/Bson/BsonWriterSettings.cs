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
    }
}