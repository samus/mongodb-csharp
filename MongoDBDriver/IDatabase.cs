using System;
using System.Collections.Generic;

namespace MongoDB.Driver
{
	public interface IDatabase
	{
		string Name { get; }
		DatabaseMetaData MetaData { get; }
		DatabaseJS JS { get; }
		List<String> GetCollectionNames();
		IMongoCollection this[String name] { get; }
		IMongoCollection GetCollection(String name);
		/// <summary>
		/// Gets the document that a reference is pointing to.
		/// </summary>
		Document FollowReference(DBRef reference);
		/// <summary>
		/// Most operations do not have a return code in order to save the client from having to wait for results.
		/// GetLastError can be called to retrieve the return code if clients want one. 
		/// </summary>
		Document GetLastError();
		/// <summary>
		/// Retrieves the last error and forces the database to fsync all files before returning. 
		/// </summary>
		/// <remarks>Server version 1.3+</remarks>
		Document GetLastErrorAndFSync();
		/// <summary>
		/// Call after sending a bulk operation to the database. 
		/// </summary>
		Document GetPreviousError();
		/// <summary>
		/// Gets the sister database on the same Mongo connection with the given name.
		/// </summary>
		Database GetSisterDatabase(string sisterDbName);
		/// <summary>
		/// Resets last error.  This is good to call before a bulk operation.
		/// </summary>
		void ResetError();
		Document Eval(string javascript);
		Document Eval(string javascript, Document scope);
		Document Eval(CodeWScope cw);
		Document SendCommand(string command);
		Document SendCommand(Document cmd);
	}
}
