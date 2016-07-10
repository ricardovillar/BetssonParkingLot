using NUnit.Framework;
using ParkingLot.Site.Extensions;

namespace ParkingLot.Tests.Site.Extensions {
    public class JsonHelperTests {
        [TestCaseSource("ObjectsAndJsons")]
        public void ToJson_should_parse_to_json(TestObject obj, string expected) {
            Assert.AreEqual(expected, obj.ToJson());
        }

        private static readonly object[] ObjectsAndJsons = {
            new object[] {new TestObject { Parameter1 = 1, Parameter2 = "Value1"}, "{\"Parameter1\":1,\"Parameter2\":\"Value1\"}"},
            new object[] {new TestObject { Parameter1 = 2, Parameter2 = "Value2"}, "{\"Parameter1\":2,\"Parameter2\":\"Value2\"}"},
            new object[] {new TestObject { Parameter1 = 3, Parameter2 = "Value3"}, "{\"Parameter1\":3,\"Parameter2\":\"Value3\"}"},
        };
    }

    public class TestObject {
        public int Parameter1 { get; set; }
        public string Parameter2 { get; set; }

        public override bool Equals(object obj) {
            if (!(obj is TestObject)) {
                return false;
            }
            var that = (TestObject) obj;
            return Parameter1 == that.Parameter1 && Parameter2 == that.Parameter2;
        }
    }
}
