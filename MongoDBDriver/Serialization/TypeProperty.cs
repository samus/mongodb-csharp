using System;
using System.Reflection;

namespace MongoDB.Driver.Serialization
{
    public class TypeProperty
    {
        public delegate object GetValueFunc(object instance);
        public delegate void SetValueAction(object instance, object value);

        private readonly GetValueFunc _getValue;
        private readonly SetValueAction _setValue;

        /// <summary>
        /// Initializes a new instance of the <see cref="TypeProperty"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="ownerType">Type of the owner.</param>
        /// <param name="propertyInfo">The property info.</param>
        public TypeProperty(string name, Type ownerType, PropertyInfo propertyInfo ){
            if(name == null)
                throw new ArgumentNullException("name");
            if(ownerType == null)
                throw new ArgumentNullException("ownerType");
            if(propertyInfo == null)
                throw new ArgumentNullException("propertyInfo");

            MongoName = name;
            OwnerType = ownerType;
            PropertyType = propertyInfo.PropertyType;
            //Todo: replace with reflection emit one
            _getValue = i=>propertyInfo.GetValue(i,null);
            //Todo: replace with reflection emit one
            _setValue = (i, o) => SetAndConvertPropertyValue(propertyInfo,i,o);
        }

        /// <summary>
        /// Gets or sets the type of the property.
        /// </summary>
        /// <value>The type of the property.</value>
        public Type PropertyType { get; private set; }

        /// <summary>
        /// Gets or sets the name of the mongo.
        /// </summary>
        /// <value>The name of the mongo.</value>
        public string MongoName { get; private set; }

        /// <summary>
        /// Gets or sets the type of the owner.
        /// </summary>
        /// <value>The type of the owner.</value>
        public Type OwnerType { get; private set; }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <returns></returns>
        public object GetValue(object instance){
            return _getValue(instance);
        }

        /// <summary>
        /// Sets the value.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <param name="value">The value.</param>
        public void SetValue(object instance, object value){
            _setValue(instance, value);
        }

        /// <summary>
        /// Sets the and convert property value.
        /// </summary>
        /// <param name="propertyInfo">The property info.</param>
        /// <param name="instance">The instance.</param>
        /// <param name="value">The value.</param>
        private void SetAndConvertPropertyValue(PropertyInfo propertyInfo,object instance,object value){
            if(value!=null){
                var type = value.GetType();
                if(PropertyType!=type){
                    var code = Convert.GetTypeCode(value);

                    if(code != TypeCode.Object)
                        value = Convert.ChangeType(value, PropertyType);
                }
            }

            propertyInfo.SetValue(instance, value, null);
        }
    }
}