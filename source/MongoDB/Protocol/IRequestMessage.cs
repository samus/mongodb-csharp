using System.IO;

namespace MongoDB.Protocol
{
    /// <summary>
    /// A Message that is to be written to the database.
    /// </summary>
    internal interface IRequestMessage
    {
        /// <summary>
        /// Writes the specified stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        void Write (Stream stream);
    }
}
