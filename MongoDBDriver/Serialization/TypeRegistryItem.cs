using System;
using System.Collections.Generic;
using System.Reflection;
using MongoDB.Driver.Serialization.Attributes;

namespace MongoDB.Driver.Serialization
{
    /// <summary>
    /// 
    /// </summary>
    public class TypeRegistryItem
    {
        public delegate object CreateInstanceDelegate(Type type);

        private const string DefaultIdProperty = "Id";
        private readonly Dictionary<string, TypeRegistryProperty> _propertys = new Dictionary<string, TypeRegistryProperty>();
        private readonly CreateInstanceDelegate _createInstance;

        /// <summary>
        /// Initializes a new instance of the <see cref="TypeRegistryItem"/> class.
        /// </summary>
        /// <param name="type">The type.</param>
        public TypeRegistryItem(Type type){
            if(type == null)
                throw new ArgumentNullException("type");
            
            Type = type;
            IdPropertyName = DefaultIdProperty;
            //Todo use factory to get name
            TypeName = type.AssemblyQualifiedName;
            //Todo: replace with reflection emit one
            _createInstance = t => Activator.CreateInstance(t);
            
            GeneratePropertys();
        }

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>The type.</value>
        public Type Type { get; private set; }

        /// <summary>
        /// Gets or sets the name of the id property.
        /// </summary>
        /// <value>The name of the id property.</value>
        public string IdPropertyName { get; set; }

        /// <summary>
        /// Gets or sets the name of the type.
        /// </summary>
        /// <remarks>
        /// This is used when UseTypeName is true to 
        /// save it to _type field into the server.
        /// </remarks>
        /// <value>The name of the type.</value>
        public object TypeName { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [use type name].
        /// </summary>
        /// <value><c>true</c> if [use type name]; otherwise, <c>false</c>.</value>
        public bool UseTypeName { get; set; }

        /// <summary>
        /// Gets the propertys.
        /// </summary>
        /// <value>The propertys.</value>
        public IEnumerable<TypeRegistryProperty> Propertys{
            get{return _propertys.Values;}
        }

        /// <summary>
        /// Gets the property.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public TypeRegistryProperty GetProperty(string name){
            TypeRegistryProperty property;

            if(!_propertys.TryGetValue(name, out property))
                //Todo: custom exception type
                throw new MongoException(string.Format("Mongo property \"{0}\" was not found on type \"{1}\"", name, Type.FullName));

            return property;
        }

        /// <summary>
        /// Creates the instance.
        /// </summary>
        /// <returns></returns>
        public object CreateInstance(){
            return _createInstance(Type);
        }

        /// <summary>
        /// Generates the propertys.
        /// </summary>
        private void GeneratePropertys(){
            foreach(var propertyInfo in Type.GetProperties()){
                var name = GetPropertyName(propertyInfo);

                _propertys.Add(propertyInfo.Name, new TypeRegistryProperty(name,Type,propertyInfo));
            }
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <param name="propertyInfo">The property info.</param>
        /// <returns></returns>
        private string GetPropertyName(PropertyInfo propertyInfo){
            foreach(MongoNameAttribute nameAttribute in propertyInfo.GetCustomAttributes(typeof(MongoNameAttribute), true))
                return nameAttribute.Name;

            return propertyInfo.Name.Equals(DefaultIdProperty) ? "_id" : propertyInfo.Name;
        }
    }
}