

using System;
using NUnit.Framework;

namespace MongoDB.Driver
{
    [TestFixture]
    public class TestOidGenerator
    {
        [Test]
        public void TestGenerate(){
            OidGenerator ogen = new OidGenerator();
            Oid oid = ogen.Generate();
            Console.WriteLine(oid.ToString());
        }
    }
}
