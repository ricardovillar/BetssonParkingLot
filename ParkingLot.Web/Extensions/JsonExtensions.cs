using System.Web.Script.Serialization;

namespace ParkingLot.Web.Extensions {
    public static class JsonExtensions {
        public static string ToJson(this object obj) {
            var serializer = new JavaScriptSerializer { MaxJsonLength = int.MaxValue };
            return serializer.Serialize(obj);
        }
    }
}