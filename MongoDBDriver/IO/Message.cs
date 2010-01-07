using System;
using System.IO;
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
