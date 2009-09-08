using System;
using System.Text.RegularExpressions;

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
			set { 
				this.ValidateValue(value);
				this.value = value; 
			}
		}
		
		public Oid(){}
		
		public Oid(string value){
			this.Value = value;
		}
		
		protected void ValidateValue(string val){
			if(val == null || val.Length != 24) throw new ArgumentException("Oid strings should be 24 characters");
			
			Regex notHexChars = new Regex(@"[^A-Fa-f0-9]", RegexOptions.None);
		    if(notHexChars.IsMatch(val)){
				throw new ArgumentOutOfRangeException("value","Value contains invalid characters");
		    }
		}
	}
}
