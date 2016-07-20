using System.Collections.Generic;
using ParkingLot.Web.Models;

namespace ParkingLot.Web.Data {
    public interface IApiService {
        IEnumerable<ParkingRecord> LoadData(string url);
    }
}