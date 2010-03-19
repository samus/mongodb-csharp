using System;
using System.Collections.Generic;
using System.Reflection;
using MongoDB.Driver.Serialization.Attributes;

namespace MongoDB.Driver.Serialization
{
    /// <summary>
    /// 
    /// </summary>
    public class TypeEntry : IObjectDescriptor
    {
        public delegate object CreateInstanceFunc(Type type);

        private const string DefaultIdProperty = "Id";
        private readonly Dictionary<string, TypeProperty> _mongoPropertys = new Dictionary<string, TypeProperty>();
        private readonly Dictionary<string, TypeProperty> _typePropertys = new Dictionary<string, TypeProperty>();
        private readonly CreateInstanceFunc _createInstance;

        /// <summary>
        /// Initializes a new instance of the <see cref="TypeEntry"/> class.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="typeName">Name of the type.</param>
        public TypeEntry(Type type,object typeName){
            if(type == null)
                throw new ArgumentNullException("type");
            if(typeName == null)
                throw new ArgumentNullException("typeName");

            Type = type;
            IdPropertyName = DefaultIdProperty;
            TypeName = typeName;
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
        public IEnumerable<TypeProperty> Propertys{
            get{return _mongoPropertys.Values;}
        }

        public TypeProperty GetPropertyFromMongoName(string mongoName){
            TypeProperty property;

            if(!_mongoPropertys.TryGetValue(mongoName, out property))
                //Todo: custom exception type
                throw new MongoException(string.Format("Mongo property \"{0}\" was not found on type \"{1}\"", mongoName, Type.FullName));

            return property;
        }

        public TypeProperty GetProperty(string propertyName){
            TypeProperty property;

            if(!_typePropertys.TryGetValue(propertyName, out property))
                //Todo: custom exception type
                throw new MongoException(string.Format("Type property \"{0}\" was not found on type \"{1}\"", propertyName, Type.FullName));

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
                var mongoName = GetPropertyName(propertyInfo);

                if(GetPropertyAttribute<MongoIgnoreAttribute>(propertyInfo)!=null)
                    continue;

                var typeProperty = new TypeProperty(mongoName, Type, propertyInfo);
                _mongoPropertys.Add(mongoName, typeProperty);
                _typePropertys.Add(typeProperty.PropertyName , typeProperty);
            }
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <param name="propertyInfo">The property info.</param>
        /// <returns></returns>
        private static string GetPropertyName(PropertyInfo propertyInfo){
            foreach(MongoNameAttribute nameAttribute in propertyInfo.GetCustomAttributes(typeof(MongoNameAttribute), true))
                return nameAttribute.Name;

            return propertyInfo.Name.Equals(DefaultIdProperty) ? "_id" : propertyInfo.Name;
        }

        /// <summary>
        /// Gets the property attribute.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="propertyInfo">The property info.</param>
        /// <returns></returns>
        private static T GetPropertyAttribute<T>(PropertyInfo propertyInfo) where T : Attribute
        {
            foreach(T attribute in propertyInfo.GetCustomAttributes(typeof(T), true))
                return attribute;
            return null;
        }

        /// <summary>
        /// Gets the property value.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <param name="mongoName">Name of the mongo.</param>
        /// <returns></returns>
        object IObjectDescriptor.GetPropertyValue(object instance, string mongoName){
            if(instance is Document){
                var document = (Document)instance;
                return document[mongoName];

            }

            var property = GetPropertyFromMongoName(mongoName);
            return property.GetValue(instance);
        }

        /// <summary>
        /// Sets the property value.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <param name="mongoName">Name of the mongo.</param>
        /// <param name="value">The value.</param>
        void IObjectDescriptor.SetPropertyValue(object instance, string mongoName, object value){
            if(instance is Document){
                var document = (Document)instance;
                document[mongoName] = value;
            }else{
                var property = GetPropertyFromMongoName(mongoName);
                property.SetValue(instance, value);
            }
        }

        /// <summary>
        /// Gets the mongo property names.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<string> GetMongoPropertyNames(){
            foreach(var typeProperty in Propertys){
                yield return typeProperty.MongoName;
            }
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString(){
            return Type.FullName;
        }
    }
}