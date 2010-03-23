using System;
using System.Collections.Generic;

namespace MongoDB.Driver
{
    /// <summary>
    /// 
    /// </summary>
    public interface IMongoDatabase
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        string Name { get; }

        /// <summary>
        /// Gets the meta data.
        /// </summary>
        /// <value>The meta data.</value>
        DatabaseMetaData MetaData { get; }

        /// <summary>
        /// Gets the javascript.
        /// </summary>
        /// <value>The javascript.</value>
        DatabaseJavascript Javascript { get; }

        /// <summary>
        /// Gets the <see cref="MongoDB.Driver.IMongoCollection"/> with the specified name.
        /// </summary>
        /// <value></value>
        IMongoCollection this[String name] { get; }

        /// <summary>
        /// Gets the collection names.
        /// </summary>
        /// <returns></returns>
        List<String> GetCollectionNames();

        /// <summary>
        /// Gets the collection.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        IMongoCollection GetCollection(String name);

        /// <summary>
        /// Gets the collection.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        IMongoCollection<T> GetCollection<T>(String name) where T : class;

        /// <summary>
        /// Gets the document that a reference is pointing to.
        /// </summary>
        /// <param name="reference">The reference.</param>
        /// <returns></returns>
        Document FollowReference(DBRef reference);

        /// <summary>
        /// Follows the reference.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="reference">The reference.</param>
        /// <returns></returns>
        T FollowReference<T>(DBRef reference) where T:class;

        /// <summary>
        /// Most operations do not have a return code in order to save the client from having to wait for results.
        /// GetLastError can be called to retrieve the return code if clients want one.
        /// </summary>
        /// <returns></returns>
        Document GetLastError();

        /// <summary>
        /// Retrieves the last error and forces the database to fsync all files before returning.
        /// </summary>
        /// <param name="fsync">if set to <c>true</c> [fsync].</param>
        /// <returns></returns>
        /// <remarks>
        /// Server version 1.3+
        /// </remarks>
        Document GetLastError(bool fsync);

        /// <summary>
        /// Call after sending a bulk operation to the database.
        /// </summary>
        /// <returns></returns>
        Document GetPreviousError();

        /// <summary>
        /// Gets the sister database on the same Mongo connection with the given name.
        /// </summary>
        /// <param name="sisterDatabaseName">Name of the sister database.</param>
        /// <returns></returns>
        MongoDatabase GetSisterDatabase(string sisterDatabaseName);

        /// <summary>
        ///   Resets last error.  This is good to call before a bulk operation.
        /// </summary>
        void ResetError();

        /// <summary>
        /// Evals the specified javascript.
        /// </summary>
        /// <param name="javascript">The javascript.</param>
        /// <returns></returns>
        Document Eval(string javascript);

        /// <summary>
        /// Evals the specified javascript.
        /// </summary>
        /// <param name="javascript">The javascript.</param>
        /// <param name="scope">The scope.</param>
        /// <returns></returns>
        Document Eval(string javascript, Document scope);

        /// <summary>
        /// Evals the specified code scope.
        /// </summary>
        /// <param name="codeScope">The code scope.</param>
        /// <returns></returns>
        Document Eval(CodeWScope codeScope);

        /// <summary>
        /// Sends the command.
        /// </summary>
        /// <param name="commandName">The command name.</param>
        /// <returns></returns>
        Document SendCommand(string commandName);

        /// <summary>
        /// Sends the command.
        /// </summary>
        /// <param name="command">The CMD.</param>
        /// <returns></returns>
        Document SendCommand(Document command);
    }
}