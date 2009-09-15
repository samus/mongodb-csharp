using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;

using MongoDB.Driver.Bson;
using MongoDB.Driver.IO;

namespace MongoDB.Driver
{
    public enum ConnectionState{
        Closed = 0,
        Opened = 1,
    }
    
    /// <summary>
    /// Description of Connection.
    /// </summary>
    public class Connection
    {       
        public const string DEFAULTHOST = "localhost";
        public const int DEFAULTPORT = 27017;
        
        protected TcpClient tcpclnt;
        #if DEBUG
        public TcpClient Tcpclnt {
            get { return tcpclnt; }
        }
        #endif
        
        protected String host;    
        public string Host {
            get { return host; }
        }
        
        protected int port;       
        public int Port {
            get { return port; }
        }
                            
        private ConnectionState state;      
        public ConnectionState State {
            get { return state; }
        }
        
        public Connection():this(DEFAULTHOST,DEFAULTPORT){
        }
        
        public Connection(String host):this(host,DEFAULTPORT){
        }
        
        public Connection(String host, int port){
            this.host = host;
            this.port = port;
            this.state = ConnectionState.Closed;
        }

        /// <summary>
        /// Used for sending a message that gets a reply such as a query.
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        /// <exception cref="IOException">A reconnect will be issued but it is up to the caller to handle the error.</exception>
        public ReplyMessage SendTwoWayMessage(RequestMessage msg){
            try{
                msg.Write(tcpclnt.GetStream());
                
                ReplyMessage reply = new ReplyMessage();                
                reply.Read(tcpclnt.GetStream());
                return reply;
            }catch(IOException ioe){
                this.Reconnect();
                throw;
            }
            
        }

        /// <summary>
        /// Used for sending a message that gets no reply such as insert or update.
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        /// <exception cref="IOException">A reconnect will be issued but it is up to the caller to handle the error.</exception>        
        public void SendMessage(RequestMessage msg){
            try{
                msg.Write(tcpclnt.GetStream()); 
                
            }catch(IOException ioe){//Sending doesn't seem to always trigger the detection of a closed socket.
                this.Reconnect();
                throw;
            }
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
        
        public void Reconnect(){
            Debug.WriteLine("Reconnecting", "Connection");
            this.Open();
        }
        
        public virtual void Open(){
            tcpclnt = new TcpClient();
            tcpclnt.Connect(this.Host, this.Port);
            this.state = ConnectionState.Opened;
        }
        
        public void Close(){
            tcpclnt.Close();
            this.state = ConnectionState.Closed;
        }
    }
}


