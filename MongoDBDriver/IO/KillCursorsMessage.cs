/*
 * User: scorder
 */
using System;
using System.IO;

using MongoDB.Driver.Bson;

namespace MongoDB.Driver.IO
{
    /// <summary>
    /// Description of KillCursorsMessage.
    /// </summary>
    public class KillCursorsMessage:RequestMessage
    {
//      struct {
//          MsgHeader header;                 // standard message header
//          int32     ZERO;                   // 0 - reserved for future use
//          int32     numberOfCursorIDs;      // number of cursorIDs in message
//          int64[]   cursorIDs;                // array of cursorIDs to close
//      }
        private long[] cursorIDs;
        public long[] CursorIDs {
            get { return cursorIDs; }
            set { cursorIDs = value; }
        }
        public KillCursorsMessage(){
            this.Header = new MessageHeader(OpCode.KillCursors);
        }
        public KillCursorsMessage(long cursorID):this(){
            this.CursorIDs = new long[]{cursorID};
        }
        public KillCursorsMessage(long[] cursorIDs):this(){
            this.CursorIDs = cursorIDs;
        }
        
        protected override void WriteBody (Stream stream){
            BsonWriter writer = new BsonWriter(stream);     
            writer.Write(0);
            writer.Write(this.CursorIDs.Length);
            foreach(long id in this.CursorIDs){
                writer.Write(id);               
            }
            writer.Flush();
        }           
    }
}
