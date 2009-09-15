/*
 * User: scorder
 * Date: 7/7/2009
 */
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;


namespace MongoDB.Driver
{
    /// <summary>
    /// Description of Mongo.
    /// </summary>
    public class Mongo
    {
        private Connection connection;
        
        private String host;    
        public string Host {
            get { return host; }
            set { host = value; }
        }
        
        private int port;   
        public int Port {
            get { return port; }
            set { port = value; }
        }
        
        /// <summary>
        /// Creates the object with the default localhost:27017 parameters. The connection is
        /// created lazily.
        /// </summary>
        public Mongo():this(Connection.DEFAULTHOST,Connection.DEFAULTPORT)
        {
        }
        
        public Mongo(String host):this(host,Connection.DEFAULTPORT){
        }
        
        public Mongo(String host, int port){
            this.Host = host;
            this.port = port;
            connection = new Connection(host, port);
        }

        public Mongo(String leftHost, String rightHost):this(leftHost,Connection.DEFAULTPORT,rightHost,Connection.DEFAULTPORT,false){}        

        public Mongo(String leftHost, int leftPort, String rightHost, int rightPort):this(leftHost,leftPort,rightHost,rightPort,false){}
        
        public Mongo(String leftHost, int leftPort, String rightHost, int rightPort, bool slaveOk){
            this.Host = host;
            this.port = port;
            connection = new PairedConnection(leftHost,leftPort,rightHost,rightPort,slaveOk);
        }
        
        public Database getDB(String name){
            return new Database(connection, name);
        }
        public Database this[ String name ]  {
            get{
                return this.getDB(name);
            }
        }       
        
        public Boolean Connect(){
            connection.Open();
            return connection.State == ConnectionState.Opened;
        }
        
        public Boolean Disconnect(){
            connection.Close();
            return connection.State == ConnectionState.Closed;
        }
    }
}
