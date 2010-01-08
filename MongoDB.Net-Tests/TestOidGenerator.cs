

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
            
            String hex = BitConverter.ToString(oid.Value).Replace("-","");
            Assert.IsTrue(hex.EndsWith("000001"), "Increment didn't start with 1.");

            oid = ogen.Generate();
            hex = BitConverter.ToString(oid.Value).Replace("-","");
            Assert.IsTrue(hex.EndsWith("000002"), "Next increment should have been 2");

        }
    }
}
