using System;

namespace MongoDB.Driver.Serialization
{
    /// <summary>
    /// 
    /// </summary>
    public class QualifiedNameTypeNameProvider : ITypeNameProvider
    {
        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public object GetName(Type type){
            return type.AssemblyQualifiedName;
        }

        /// <summary>
        /// Gets the type.
        /// </summary>
        /// <param name="typeName">Name of the type.</param>
        /// <returns></returns>
        public Type GetType(object typeName){
            if(typeName == null)
                throw new ArgumentNullException("typeName");

            var stringName = Convert.ToString(typeName);

            try{
                return Type.GetType(stringName);
            }
            catch(SystemException exception){
                throw new MongoException(string.Format("Type for type name \"{0}\" could not found", typeName),exception);
            }
        }
    }
}