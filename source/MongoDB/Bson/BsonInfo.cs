using System;

namespace MongoDB.Driver.Bson
{
    /// <summary>
    /// 
    /// </summary>
    public static class BsonInfo
    {
        /// <summary>
        /// Initializes the <see cref="BsonInfo"/> class.
        /// </summary>
        static BsonInfo(){
            Epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            MaxDocumentSize = 1024 * 1024 * 4; //4MB.
        }

        /// <summary>
        /// Gets or sets the epoch.
        /// </summary>
        /// <value>The epoch.</value>
        public static DateTime Epoch { get; private set; }
        
        /// <summary>
        /// The maximum size a document can be.
        /// </summary>
        public static int MaxDocumentSize { get; private set; }
    }
}