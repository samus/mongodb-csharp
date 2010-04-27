using System;
using NUnit.Framework;

namespace MongoDB.UnitTests
{
    [TestFixture]
    public class TestOidGenerator
    {
        [Test]
        public void TestGenerate(){
            OidGenerator ogen = new OidGenerator();
            Oid oid = ogen.Generate();
            
            String hex = BitConverter.ToString(oid.ToByteArray()).Replace("-","");
            Assert.IsTrue(hex.EndsWith("000001"), "Increment didn't start with 1.");

            oid = ogen.Generate();
            hex = BitConverter.ToString(oid.ToByteArray()).Replace("-","");
            Assert.IsTrue(hex.EndsWith("000002"), "Next increment should have been 2");

            
            DateTime created = oid.Created;
            DateTime now = DateTime.UtcNow;
            Console.Out.WriteLine(oid.Created);
            Assert.AreEqual(now.Year, created.Year);
            Assert.AreEqual(now.Month, created.Month);
        }
    }
}
