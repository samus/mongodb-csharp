/*
 * User: scorder
 * Date: 7/8/2009
 */
using System;
using System.Collections;
using System.Collections.Generic;

namespace MongoDB.Driver
{
	/// <summary>
	/// Description of Document.
	/// </summary>
	public class Document : System.Collections.DictionaryBase
	{
		private List<String> orderedKeys = new List<String>();
		public Document(){
		}

		public Object this[ String key ]  {
			get{
				return Dictionary[key];
			}
			set{
				if(orderedKeys.Contains(key) == false){
					orderedKeys.Add(key);
				}
				Dictionary[key] = value;
			}
		}
		
		public ICollection Keys  {
			get  {
				return(orderedKeys);
			}
		}
		
		public ICollection Values  {
			get  {
				return( Dictionary.Values );
			}
		}

		public void Add( String key, Object value )  {
			Dictionary.Add( key, value );
			//Relies on ArgumentException from above if key already exists.
			orderedKeys.Add(key);
		}
		
		public Document Append(String key, Object value){
			this.Add(key,value);
			return this;
		}
		
		public Document Update(Document from){
			if(from == null) return this;
			foreach(String key in from.Keys){
				this[key] = from[key];
			}
			return this;
		}
		
		public bool Contains( String key )  {
			return( orderedKeys.Contains( key ) );
		}
		
		public void Remove( String key )  {
			Dictionary.Remove( key );
			orderedKeys.Remove(key);
		}
		
		/// <summary>
		/// TODO Fix any accidental reordering issues.
		/// </summary>
		/// <param name="dest"></param>
		public void CopyTo(Document dest){
			foreach(String key in orderedKeys){
				dest[key] = this[key];
			}
		}
	}
}
