/*
 * User: scorder
 * Date: 7/10/2009
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;

using MongoDB.Driver.Bson;
using MongoDB.Driver.IO;

namespace MongoDB.Driver
{
	/// <summary>
	/// Description of Connection.
	/// </summary>
	public class Connection
	{
		private static string DEFAULTHOST = "localhost";
		private static int DEFAULTPORT = 27017;
		
		private TcpClient tcpclnt = new TcpClient();
		
		private String host;	
		public string Host {
			get { return host; }
		}
		
		private int port;		
		public int Port {
			get { return port; }
		}
							
		private Boolean opened;		
		public bool Opened {
			get { return opened; }
		}
		
		public Connection():this(DEFAULTHOST,DEFAULTPORT){
		}
		
		public Connection(String host):this(host,DEFAULTPORT){
		}
		
		public Connection(String host, int port){
			this.host = host;
			this.port = port;
		}
			
		public ReplyMessage SendTwoWayMessage(RequestMessage msg){
			msg.Write(tcpclnt.GetStream());
			
			ReplyMessage reply = new ReplyMessage();
			reply.Read(tcpclnt.GetStream());
			
			return reply;
		}
		
		public void SendMessage(RequestMessage msg){
			msg.Write(tcpclnt.GetStream());
		}
		
		/// <summary>
		/// Just sends a simple message string to the database. 
		/// </summary>
		/// <param name="message">
		/// A <see cref="System.String"/>
		/// </param>
		public void SendMsgMessage(String message){
			MsgMessage msg = new MsgMessage();
			msg.Message = message;
			msg.Write(tcpclnt.GetStream());
		}
		
		public void Open(){
            tcpclnt.Connect(this.Host, this.Port);
            this.opened = true;
		}
		
		public void Close(){
			tcpclnt.Close();			
		}
	}
}
