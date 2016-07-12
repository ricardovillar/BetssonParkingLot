using System.Collections.Generic;
using ParkingLot.Web.Models;

namespace ParkingLot.Web.Data {
    public interface IChartDataBuilder {
        ChartData Build(IEnumerable<ParkingRecord> records);
    }
}