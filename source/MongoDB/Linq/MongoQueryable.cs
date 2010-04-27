namespace MongoDB.Driver.Linq
{
    /// <summary>
    /// 
    /// </summary>
    public static class MongoQueryable
    {
        /// <summary>
        /// Keys the specified document.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="document">The document.</param>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public static DocumentQuery Key<T>(this T document, string key) where T : Document
        {
            return new DocumentQuery(document, key);
        }
    }
}