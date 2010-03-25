using System;

namespace MongoDB.Driver.Bson
{
    /// <summary>
    /// 
    /// </summary>
    public class BsonReaderSettings
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BsonReaderSettings"/> class.
        /// </summary>
        public BsonReaderSettings()
            :this(new BsonDocumentBuilder()){
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BsonReaderSettings"/> class.
        /// </summary>
        /// <param name="builder">The builder.</param>
        public BsonReaderSettings(IBsonObjectBuilder builder){
            if(builder == null)
                throw new ArgumentNullException("builder");

            Builder = builder;
        }

        /// <summary>
        /// Gets or sets the builder.
        /// </summary>
        /// <value>The builder.</value>
        public IBsonObjectBuilder Builder { get; private set; }

        /// <summary>
        /// MongoDB stores all time values in UTC timezone. If true the 
        /// time is read as if it ware local time.
        /// </summary>
        /// <value><c>true</c> if [read as local time]; otherwise, <c>false</c>.</value>
        public bool ReadAsLocalTime { get; set; }

        /// <summary>
        /// MongoDB stores all time values in UTC timezone. If true the 
        /// time is converter to local timezone.
        /// </summary>
        /// <value><c>true</c> if [convert to local time]; otherwise, <c>false</c>.</value>
        public bool ConvertToLocalTime { get; set; }
    }
}