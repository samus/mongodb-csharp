
using System;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;

namespace MongoDB.Driver
{
    public class OidGenerator
    {
        private int inc;
        private object inclock =  new object();
        
        private DateTime epoch = new DateTime(1970,1,1,0,0,0,DateTimeKind.Utc);
        private byte[] machineHash;
        private byte[] procID;
        
        public OidGenerator(){
            GenerateConstants();
        }
        
        public Oid Generate(){
            byte[] oid = new byte[12];
            int copyidx = 0;

            Array.Copy(BitConverter.GetBytes(GenerateTime()),0,oid,copyidx,4);
            copyidx += 4;

            Array.Copy(machineHash,0,oid,copyidx,3);
            copyidx += 3;
                       
            Array.Copy(this.procID,0,oid,copyidx,2);
            copyidx += 2;
            
            Array.Copy(BitConverter.GetBytes(GenerateInc()),0,oid,copyidx,3);
            
            return new Oid(oid);            
        }
        
        private int GenerateTime(){
            DateTime now = DateTime.Now.ToUniversalTime();;
            DateTime nowtime = new DateTime(epoch.Year, epoch.Month, epoch.Day, now.Hour, now.Minute, now.Second, now.Millisecond);
            TimeSpan diff = nowtime - epoch;
            return Convert.ToInt32(Math.Floor(diff.TotalMilliseconds));            
        }
        
        private int GenerateInc(){
            lock(this.inclock){
                return inc++;    
            }
        }
        
        private void GenerateConstants(){
            this.machineHash = GenerateHostHash();
            this.procID = BitConverter.GetBytes(GenerateProcId());
        }
        
        private byte[] GenerateHostHash(){            
            MD5 md5 = MD5.Create();            
            string host = System.Net.Dns.GetHostName();
            return md5.ComputeHash(Encoding.Default.GetBytes(host));            
        }
        
        private int GenerateProcId(){
            Process proc = Process.GetCurrentProcess();
            return proc.Id;            
        }

    }
}
