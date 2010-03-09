using System;

namespace MongoDB.Driver.Bson
{
    public static class BsonInfo
    {
        /// <summary>
        /// Initializes the <see cref="BsonInfo"/> class.
        /// </summary>
        static BsonInfo(){
            Epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        }

        /// <summary>
        /// Gets or sets the epoch.
        /// </summary>
        /// <value>The epoch.</value>
        public static DateTime Epoch { get; private set; }
    }
}