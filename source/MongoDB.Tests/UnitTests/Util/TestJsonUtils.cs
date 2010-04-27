using System;
using System.Globalization;
using System.Threading;
using MongoDB.Util;
using NUnit.Framework;

namespace MongoDB.UnitTests.Util
{
    [TestFixture]
    public class TestJsonUtils
    {
        [Test]
        public void TestNothingToEscape(){
            string str = "There is nothing here to escape";
            Assert.AreEqual(str, JsonFormatter.Escape(str));
        }
        
        [Test]
        public void TestAsciiToEscape(){
            string str = "\b \f \n \r \t \v \' \" \\";
            string expected = @"\b \f \n \r \t \v \' \"" \\";
            Assert.AreEqual(expected, JsonFormatter.Escape(str));
        }
        
        [Test]
        public void TestPrintableUnicode(){
            string str = "pi (π)";
            Assert.AreEqual(str, JsonFormatter.Escape(str));
        }
        
        [Test]
        public void TestNonPrintableUnicode(){
            string str = "\u0007 bell";
            string expected = @"\u0007 bell";
            Assert.AreEqual(expected, JsonFormatter.Escape(str));
        }
        
        [Test]
        public void TestSerializeDocWithSingleNullField() {
            var doc = new Document().Add("foo", null);
            Assert.AreEqual(@"{ ""foo"": null }", JsonFormatter.Serialize(doc));
        }

        [Test]
        public void TestSerializeDocWithSingleTrueField() {
            var doc = new Document().Add("foo", true);
            Assert.AreEqual(@"{ ""foo"": true }", JsonFormatter.Serialize(doc));
        }

        [Test]
        public void TestSerializeDocWithSingleFalseField() {
            var doc = new Document().Add("foo", false);
            Assert.AreEqual(@"{ ""foo"": false }", JsonFormatter.Serialize(doc));
        }

        [Test]
        public void TestSerializeDocWithSingleStringField() {
            var doc = new Document().Add("foo", "bar");
            Assert.AreEqual(@"{ ""foo"": ""bar"" }", JsonFormatter.Serialize(doc));
        }
        
        [Test]
        public void TestSerializeDocWithLowUnicodeValues(){
            var doc = new Document(){{"foo", "\u0007 bell"}};
            Assert.AreEqual(@"{ ""foo"": ""\u0007 bell"" }", JsonFormatter.Serialize(doc));
        }
        [Test]
        public void TestSerializeDocWithHighUnicodeValues(){
            var doc = new Document(){{"foo", "pi (π)"}};
            Assert.AreEqual(@"{ ""foo"": ""pi (π)"" }", JsonFormatter.Serialize(doc));
        }        

        [Test]
        public void TestSerializeDocWithSingleIntField() {
            var doc = new Document().Add("foo", 10);
            Assert.AreEqual(@"{ ""foo"": 10 }", JsonFormatter.Serialize(doc));
        }

        [Test]
        public void TestSerializeDocWithSingleDoubleField() {
            var doc = new Document().Add("foo", 10.1);
            Assert.AreEqual(@"{ ""foo"": 10.1 }", JsonFormatter.Serialize(doc));
        }

        [Test]
        public void TestSerializeCultureInvariantNumbers() {
            var cultureBackup = Thread.CurrentThread.CurrentCulture;

            Thread.CurrentThread.CurrentCulture = new CultureInfo("de-DE");
            var doc = new Document().Add("foo", 10.1);
            Assert.AreEqual(@"{ ""foo"": 10.1 }", JsonFormatter.Serialize(doc));
            
            Thread.CurrentThread.CurrentCulture = cultureBackup;
        }

        [Test]
        public void TestSerializeDocWithSingleDateTimeField() {
            var doc = new Document().Add("foo", DateTime.Parse("2009-10-10T07:00:00.0000000Z"));
            Assert.AreEqual(@"{ ""foo"": ""2009-10-10T07:00:00.0000000Z"" }", JsonFormatter.Serialize(doc));
        }

        [Test]
        public void TestSerializeDocWithSingleOidField() {
            var doc = new Document().Add("foo", new Oid("4ac7ee91f693066f1c96b649"));
            Assert.AreEqual(@"{ ""foo"": ""4ac7ee91f693066f1c96b649"" }", JsonFormatter.Serialize(doc));
        }
        [Test]
        public void TestSerializeDocWithMultipleFields() {
            var doc = new Document().Add("foo", "bar").Add("baz", 42);
            Assert.AreEqual(@"{ ""foo"": ""bar"", ""baz"": 42 }", JsonFormatter.Serialize(doc));
        }

        [Test]
        public void TestSerializeDocWithSubDocField() {
            var doc = new Document().Add("foo", "bar").Add("baz", new Document().Add("a", 1));
            Assert.AreEqual(@"{ ""foo"": ""bar"", ""baz"": { ""a"": 1 } }", JsonFormatter.Serialize(doc));
        }
        [Test]
        public void TestSerializeDocWithArrayOfInts() {
            var doc = new Document().Add("foo", new[] { 1, 2, 3, 4 });
            Assert.AreEqual(@"{ ""foo"": [ 1, 2, 3, 4 ] }", JsonFormatter.Serialize(doc));
        }

        [Test]
        public void TestSerializeDocWithArrayOfDocs() {
            var doc = new Document().Add("foo", new[] {
                new Document().Add("a", 1),
                new Document().Add("b", 2),
                new Document().Add("c", 3),
            });
            Assert.AreEqual(@"{ ""foo"": [ { ""a"": 1 }, { ""b"": 2 }, { ""c"": 3 } ] }", JsonFormatter.Serialize(doc));
        }
        
        [Test]
        public void TestSerializeDocWithBinary(){
            var doc = new Document(){{"b", new Binary(){Bytes = new byte[]{0,1,2,3,4}, 
                                                            Subtype = Binary.TypeCode.General}}};
            Assert.AreEqual(@"{ ""b"": { ""$binary"": ""AAECAwQ="", ""$type"" : 2 } }", 
                            JsonFormatter.Serialize(doc));
        }
        
        [Test]
        public void TestSerializeDocWithGUID(){
            var doc = new Document(){{"guid", new Guid("936da01f-9abd-4d9d-80c7-02af85c822a8")}};
            Assert.AreEqual(@"{ ""guid"": { ""$uid"": ""936da01f-9abd-4d9d-80c7-02af85c822a8"" } }", 
                            JsonFormatter.Serialize(doc));
        }
        
        [Test]
        public void TestSerializeDocWithDBRef(){
            var ostr = "000102030405060708090001";
            var doc = new Document(){{"d", new DBRef("smallreads", new Oid(ostr))}};
            Assert.AreEqual(String.Format(@"{{ ""d"": {{ ""$ref"": ""smallreads"", ""$id"": ""{0}"" }} }}", ostr), 
                            JsonFormatter.Serialize(doc));
        }
        
        [Test]
        public void TestSerializeDocWithMinMax(){
            var doc = new Document(){{"mn", MongoMinKey.Value}, {"mx", MongoMaxKey.Value}};
            Assert.AreEqual(@"{ ""mn"": { ""$minkey"": 1 }, ""mx"": { ""$maxkey"": 1 } }", 
                            JsonFormatter.Serialize(doc));
        }
        
        [Test]
        public void TestSerializeDocWithCode(){
            var c = new Code("function add(x, y){\n" +
                             "  return x + y;\n" +
                             "}\n");
            var doc = new Document(){{"c", c}};
            Assert.AreEqual(@"{ ""c"": { ""$code"": ""function add(x, y){\n  return x + y;\n}\n"" } }",
                            JsonFormatter.Serialize(doc));
        }
            
    }
}
