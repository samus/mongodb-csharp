namespace MongoDB.Driver.Bson
{
    public struct BsonObjectProperty
    {
        public string Name { get; set; }
        public object Value { get; set; }

        public BsonObjectProperty(string name, object value)
            : this()
        {
            Name = name;
            Value = value;
        }
    }
}