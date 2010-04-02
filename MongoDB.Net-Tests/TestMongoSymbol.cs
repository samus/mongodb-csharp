
using System;
using System.Text;

using NUnit.Framework;

namespace MongoDB.Driver
{


    [TestFixture()]
    public class TestMongoSymbol{

        [Test]
        public void TestValue(){
            Assert.IsTrue(string.IsInterned("s") != null);
            
            MongoSymbol s = new MongoSymbol("s");
            Assert.IsNotNull(s.Value);
            Assert.IsTrue(string.IsInterned(s.Value) != null, "First value was not interned");
            
            string val = new StringBuilder().Append('s').ToString();
            Assert.IsFalse(string.IsInterned(val) == null);
            MongoSymbol s2 = new MongoSymbol(val);
            Assert.IsTrue(string.IsInterned(s2.Value) != null, "Second value was not interned");
            
            Assert.IsTrue(System.Object.ReferenceEquals(s.Value, s2.Value));
                                
        }
        
        [Test]
        public void TestNullSymbolValue(){
            bool thrown = false;
            try{
                MongoSymbol s = new MongoSymbol(null);
            }catch(ArgumentNullException){
                thrown = true;
            }
            Assert.IsTrue(thrown);
        }
        
        [Test]
        public void TestComparing(){
            MongoSymbol a = new MongoSymbol("a");
            MongoSymbol a2 = new MongoSymbol("a");
            MongoSymbol b = new MongoSymbol("b");
            
            Assert.AreEqual(0, a.CompareTo(a2));
            Assert.AreEqual(0, a2.CompareTo(a2));
            
            Assert.AreEqual(-1, a.CompareTo(b));
            Assert.AreEqual(1, b.CompareTo(a));
            
        }
        
        [Test]
        public void TestEquals(){
            MongoSymbol a = new MongoSymbol("a");
            MongoSymbol a2 = new MongoSymbol("a");
            string astr = "a";
            
            MongoSymbol b = new MongoSymbol("b");
            string bstr = "b";
            
            Assert.IsTrue(a.Equals(a2));
            Assert.IsTrue(a2.Equals(a));
            Assert.IsTrue(a.Equals(astr));
            
            Assert.IsFalse(a.Equals(b));
            Assert.IsFalse(b.Equals(a));
            Assert.IsFalse(a.Equals(bstr));
            
            
            
        }
        
        [Test]
        public void TestEqOperator(){
            MongoSymbol a = new MongoSymbol("a");
            MongoSymbol a2 = new MongoSymbol("a");
            string astr = "a";
            
            MongoSymbol b = new MongoSymbol("b");
            string bstr = "b";
            
            Assert.IsTrue(a == a);
            Assert.IsTrue(a == a2);
            Assert.IsTrue(a2 == a);
            Assert.IsTrue(a == astr);
            Assert.IsTrue(astr == a);
            
            Assert.IsTrue(a == new StringBuilder().Append('a').ToString()); //Not interned like the hard coded ones above.
            
            Assert.IsFalse(a == b);
            Assert.IsFalse(a == bstr);
            Assert.IsFalse(bstr == a);
            
            Assert.IsFalse(a == null);
        }
        
        [Test]
        public void TestNotEqOperator(){
            MongoSymbol a = new MongoSymbol("a");
            MongoSymbol a2 = new MongoSymbol("a");
            string astr = "a";
            
            MongoSymbol b = new MongoSymbol("b");
            string bstr = "b";
            
            Assert.IsFalse(a != a);
            Assert.IsFalse(a != a2);
            Assert.IsFalse(a2 != a);
            Assert.IsFalse(a != astr);
            Assert.IsFalse(astr != a);
            
            Assert.IsTrue(a != b);
            Assert.IsTrue(a != bstr);
            Assert.IsTrue(bstr != a);
            
            Assert.IsTrue(a != null);
        }
        
        [Test]
        public void TestEmpty(){
            MongoSymbol empty = MongoSymbol.Empty;
            Assert.IsTrue(MongoSymbol.IsEmpty(empty));
            Assert.IsFalse(MongoSymbol.IsEmpty(new MongoSymbol("a")));
        }
        
        [Test]
        public void TestToString(){
            string val = "symbol";
            MongoSymbol sym = new MongoSymbol(val);
            
            string str = sym;
            Assert.AreEqual(val, str);
            Assert.IsTrue(str == sym);
            Assert.AreEqual(str, sym.ToString());
            
        }
            
    }
}
