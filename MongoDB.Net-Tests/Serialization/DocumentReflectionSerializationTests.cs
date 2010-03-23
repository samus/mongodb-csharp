using System;

using System.IO;

using NUnit.Framework;

using MongoDB.Driver.Bson;

namespace MongoDB.Driver.Serialization
{


    /// <summary>
    /// Not really unit tests but it is a good sanity check to make sure that equivalent documents and
    /// objects produce the same output.
    /// </summary>
    [TestFixture()]
    public class DocumentReflectionSerializationTests
    {
        public class SimpleObject
        {
            public string A { get; set; }
            public string B { get; set; }
            public SimpleObjectC C { get; set; }
        }

        public class SimpleObjectC
        {
            public string D { get; set; }
        }
        
        
        [Test()]
        public void TestSimpleObjectSimpleDoc(){
            var s = new SimpleObject(){A = "A", B = "B"};
            var d = new Document(){{"A", "A"},{"B", "B"},{"C",null}};
            
            Assert.AreEqual(SerializeDocument(d), SerializeObject(s));
        }
        
        [Test()]
        public void TestSimpleObjectSimpleDocWithC(){
            var s = new SimpleObject(){A = "A", B = "B", C = new SimpleObjectC(){D = "D"}};
            var d = new Document(){{"A", "A"},{"B", "B"},{"C",new Document(){{"D", "D"}}}};
            
            Assert.AreEqual(SerializeDocument(d), SerializeObject(s));
        }        
        
        [Test]
        public void TestAnonymousType(){
            var o = new {A = "A", B = "B", C = "C"};
            var d = new Document(){{"A", "A"},{"B", "B"},{"C","C"}};
            
            Assert.AreEqual(SerializeDocument(d), SerializeObject(o));
        }
        
        protected string SerializeDocument(Document source){
            using(var ms = new MemoryStream()){
                BsonWriter writer = new BsonWriter(ms, new BsonDocumentDescriptor());
    
                writer.WriteObject(source);
                writer.Flush();
                return BitConverter.ToString(ms.ToArray());
            }
        }
        
        protected string SerializeObject(object instance){
            return SerializeObject(instance, instance.GetType());
        }

        protected string SerializeObject(object instance,Type rootType){
            using(var ms = new MemoryStream())
            {
                var writer = new BsonWriter(ms, new BsonReflectionDescriptor(SerializationFactory.Default, rootType));
                writer.WriteObject(instance);
                writer.Flush();
                return BitConverter.ToString(ms.ToArray());
            }
        }        
    }
}
