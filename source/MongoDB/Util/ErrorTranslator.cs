using System;

namespace MongoDB.Util
{
    /// <summary>
    ///   Translates an error returned from Mongo into the proper exception.
    /// </summary>
    internal class ErrorTranslator
    {
        /// <summary>
        /// Translates the specified error.
        /// </summary>
        /// <param name="error">The error.</param>
        /// <returns></returns>
        public static MongoException Translate(Document error){
            if(error == null)
                throw new ArgumentNullException("error");

            var errorMessage = (string)error["err"];
            var errorNumber = GetErrorNumber(errorMessage);
            
            return BuildException(errorNumber, errorMessage, error);
        }

        /// <summary>
        /// Determines whether the specified document is error.
        /// </summary>
        /// <param name="document">The document.</param>
        /// <returns>
        /// 	<c>true</c> if the specified document is error; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsError(Document document){
            if(document.Contains("err") && document["err"] != null)
                return true;
            return false;
        }

        /// <summary>
        /// Gets the error number.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns></returns>
        private static string GetErrorNumber(string message){
            if(message.StartsWith("E"))
                return message.Substring(1, 5);
            
            return "00000";
        }

        /// <summary>
        /// Builds the exception.
        /// </summary>
        /// <param name="errorNumber">The error number.</param>
        /// <param name="message">The message.</param>
        /// <param name="error">The error.</param>
        /// <returns></returns>
        private static MongoException BuildException(string errorNumber, string message, Document error){
            switch(errorNumber){
                case "11000":{
                    return new MongoDuplicateKeyException(message, error);
                }
                case "11001":{
                    return new MongoDuplicateKeyUpdateException(message, error);
                }
                //General exceptions, just pass the message back on.
                case "10003":
                case "12000":
                case "12001":
                case "12010":
                case "12011":
                case "12012":
                goto default;
                default:{
                    return new MongoOperationException(message, error);
                }
            }
        }
    }
}