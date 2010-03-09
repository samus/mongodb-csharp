namespace MongoDB.Driver.Bson
{
    public interface IBsonObjectBuilder
    {
        object BeginObject();
        object EndObject(object instance);
        object BeginArray();
        object EndArray(object instance);
        void BeginProperty(object instance, string name);
        void EndProperty(object instance, object value);
    }
}