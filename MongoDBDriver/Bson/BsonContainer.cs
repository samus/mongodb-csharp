/*
 * User: scorder
 */
using System;
using System.Collections.Generic;

namespace MongoDB.Driver.Bson
{
	/// <summary>
	/// Description of BsonContainer.
	/// </summary>
	public class BsonContainer<T>
	{
		private List<T> orderedKeys = new List<T>();
//		private SortedList<TKey, TValue>
		public BsonContainer()
		{
		}
	}
}
