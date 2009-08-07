/*
 * User: scorder
 * Date: 7/7/2009
 */
using System;
using System.Text;

namespace MongoDB.Driver.IO
{
	
	public class Message
	{
		protected UTF8Encoding encoding = new UTF8Encoding();
		
		private MessageHeader header;		
		public MessageHeader Header {
			get { return header; }
			set { header = value; }
		}
		
	}

}
