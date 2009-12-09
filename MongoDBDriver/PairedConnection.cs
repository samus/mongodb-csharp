using System;
using System.Net.Sockets;

namespace MongoDB.Driver
{
    /// <summary>
    /// Connection object that connects to a replica pair.
    /// </summary>
    public class PairedConnection : Connection
    {
        private String slaveHost;
        public string SlaveHost {
            get { return slaveHost; }
        }
        
        private int slavePort;       
        public int SlavePort {
            get { return slavePort; }
        }
        
        private bool slaveOk;        
        public bool SlaveOk {
            get { return slaveOk; }
            set { slaveOk = value; }
        }
        
        private Document masterInfo;
        public bool Paired {
            get { return this.masterInfo != null && (string)this.masterInfo["msg"] != "not paired"; }
        }        

        public bool ConnectedToMaster {
            get { return this.masterInfo != null && (int)this.masterInfo["ismaster"] == 1; }
        }        
                
        public PairedConnection(String leftHost, String rightHost):this(leftHost, DEFAULTPORT, rightHost, DEFAULTPORT){}
        
        public PairedConnection(String leftHost, int leftPort, String rightHost, int rightPort):this(leftHost,leftPort,rightHost,rightPort,false){}
        
        public PairedConnection(String leftHost, int leftPort, String rightHost, int rightPort, bool slaveOk):base(leftHost,leftPort){
            slaveHost = rightHost;
            slavePort = rightPort;
            this.SlaveOk = slaveOk;
        }
        
        public override void Open(){
            try{
                TryOpen();
            }catch(Exception ex){
                if (ex is SocketException || ex is System.IO.IOException){
                    //Couldn't connect.  Try to connect to the slave instance.
                    SwapHosts();
                    this.Close();
                    this.TryOpen();
                }else{
                    throw;
                }
            }
        }
        
        private void TryOpen(){
                base.Open();             
                this.masterInfo = QueryMaster();
                if(Paired && !ConnectedToMaster && !SlaveOk){
                    if(this.masterInfo.Contains("remote") == false){
                        //FIXME Is this the right exception?
                        throw new MongoCommException("No master found",this); 
                    }
                    SwapHosts();
                    this.Close();
                    this.TryOpen();
                }                    
        }
        
        public Document QueryMaster(){
            Database admin = new Database(this,"admin");
            Document master = admin["$cmd"].FindOne(new Document().Append("ismaster", 1.0));
            return master;
        }
        
        private void SwapHosts(){
            int tport = this.Port;
            string tHost = this.Host;
            port = this.SlavePort;
            host = this.slaveHost;
            slavePort = tport;
            slaveHost = tHost;
        }

    }
}
