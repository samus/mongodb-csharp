using System;
using System.Collections.Generic;
using MongoDB.Configuration.Mapping.Model;
using NUnit.Framework;

namespace MongoDB.UnitTests.Configuration.Mapping.Model
{
    public class CollectionMemberMapTests
    {
        [Test]
        public void SetValue_Null_ReturnsNullAndSkipCollectionAdapterCall()
        {
            object setterValue = string.Empty;
            Action<object, object> setter = (instance, value) => setterValue=value;
            var map = new CollectionMemberMap("Test",typeof(IEnumerable<string>),o => null  ,setter,"Test",false,null,typeof(string));

            map.SetValue(null,null);

            Assert.IsNull(setterValue);
        }

        [Test]
        public void GetValue_Null_ReturnsNullAndSkipCollectionAdapterCall()
        {
            Func<object, object> getter = instance => null;
            var map = new CollectionMemberMap("Test", typeof(IEnumerable<string>), getter, (i, v) => {}, "Test", false, null, typeof(string));

            var value = map.GetValue(null);

            Assert.IsNull(value);
        }
    }
}