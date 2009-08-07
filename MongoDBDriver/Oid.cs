/*
 * User: scorder
 */
using System;

namespace MongoDB.Driver
{
	/// <summary>
	/// Description of Oid.
	/// </summary>
	public class Oid
	{
		private string value;		
		public string Value {
			get { return this.value; }
			set { this.value = value; }
		}
		
		public Oid(){}
		
		public Oid(string value){
			this.Value = value;
		}
	}
}
