using System;

namespace MongoDB.Driver.Util
{
    /// <summary>
    /// Translates an error returned from Mongo into the proper exception.
    /// </summary>
    internal class ErrorTranslator
    {
        public static MongoException Translate(Document error){
            string msg = (string)error["err"];
            string errnum = GetErrorNum(msg);
            return BuildException(errnum, msg, error);
        }
        
        public static bool IsError(Document doc){
            if(doc.Contains("err") && doc["err"] != DBNull.Value)return true;
            return false;
        }
        
        private static string GetErrorNum(string msg){
            if(msg.StartsWith("E")){
                return msg.Substring(1,5);
            }else{
                return "00000";
            }
        }
        
        private static MongoException BuildException(string errnum, string msg, Document error){
            switch(errnum){
                case "11000":{
                    return new MongoDuplicateKeyException(msg, error);
                }
                case "11001":{
                    return new MongoDuplicateKeyUpdateException(msg, error);
                }
                //General exceptions, just pass the message back on.
                case "10003":
                case "12000":
                case "12001":
                case "12010":
                case "12011":
                case "12012":
                default:{
                    return new MongoOperationException(msg, error);
                }                
            }
        }
        
    }
}
