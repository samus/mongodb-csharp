
using System;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using MongoDB.Bson;

namespace MongoDB
{
    /// <summary>
    /// 
    /// </summary>
    internal class OidGenerator
    {
        private int inc;
        private object inclock =  new object();
        private byte[] machineHash;
        private byte[] procID;

        /// <summary>
        /// Initializes a new instance of the <see cref="OidGenerator"/> class.
        /// </summary>
        public OidGenerator(){
            GenerateConstants();
        }

        /// <summary>
        /// Generates this instance.
        /// </summary>
        /// <returns></returns>
        public Oid Generate(){
            //FIXME Endian issues with this code.  
            //.Net runs in native endian mode which is usually little endian.  
            //Big endian machines don't need the reversing (Linux+PPC, XNA on XBox)
            byte[] oid = new byte[12];
            int copyidx = 0;

            byte[] time = BitConverter.GetBytes(GenerateTime());
            Array.Reverse(time);
            Array.Copy(time,0,oid,copyidx,4);
            copyidx += 4;

            Array.Copy(machineHash,0,oid,copyidx,3);
            copyidx += 3;
                       
            Array.Copy(this.procID,2,oid,copyidx,2);
            copyidx += 2;
            
            byte[] inc = BitConverter.GetBytes(GenerateInc());
            Array.Reverse(inc);
            Array.Copy(inc,1,oid,copyidx,3);
            
            return new Oid(oid);            
        }

        /// <summary>
        /// Generates the time.
        /// </summary>
        /// <returns></returns>
        private int GenerateTime(){
            DateTime now = DateTime.UtcNow;
            //DateTime nowtime = new DateTime(epoch.Year, epoch.Month, epoch.Day, now.Hour, now.Minute, now.Second, now.Millisecond);
            TimeSpan diff = now - BsonInfo.Epoch;
            return Convert.ToInt32(Math.Floor(diff.TotalSeconds));            
        }

        /// <summary>
        /// Generates the inc.
        /// </summary>
        /// <returns></returns>
        private int GenerateInc(){
            lock(this.inclock){
                return ++inc;    
            }
        }

        /// <summary>
        /// Generates the constants.
        /// </summary>
        private void GenerateConstants(){
            this.machineHash = GenerateHostHash();
            this.procID = BitConverter.GetBytes(GenerateProcId());
            Array.Reverse(this.procID);
        }

        /// <summary>
        /// Generates the host hash.
        /// </summary>
        /// <returns></returns>
        private byte[] GenerateHostHash(){            
            MD5 md5 = MD5.Create();            
            string host = System.Net.Dns.GetHostName();
            return md5.ComputeHash(Encoding.Default.GetBytes(host));            
        }

        /// <summary>
        /// Generates the proc id.
        /// </summary>
        /// <returns></returns>
        private int GenerateProcId(){
            Process proc = Process.GetCurrentProcess();
            return proc.Id;            
        }
    }
}
