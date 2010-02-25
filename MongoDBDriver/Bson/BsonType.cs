
using System;

namespace MongoDB.Driver.Bson
{
    public interface BsonType{
        Int32 Size{get;}
        Byte TypeNum{get;}      
        int Read(BsonReader reader);
        void Write(BsonWriter writer);
        Object ToNative();
    }
}
