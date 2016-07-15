using System;
using System.Linq;
using System.Collections.Generic;

namespace ParkingLot.Web.Models {
    public class ChartData {
        public DateTime? StartDate { get; private set; }
        public DateTime? EndDate { get; private set; }
        public TimeSpan? TopOccupationTime { get; private set; }
        public int MaxOccupation { get; private set; }
        public IEnumerable<ChartPoint> Data { get; private set; }

        public ChartData(IEnumerable<ChartPoint> data) {
            Data = data;
            if (data.Any()) {
                StartDate = Data.Min(d => d.Date);
                EndDate = Data.Max(d => d.Date);
                MaxOccupation = Data.Max(d => d.Cars);
                TopOccupationTime = Data.Where(d => d.Cars == MaxOccupation).First().Date.TimeOfDay;
            }
        }

        public ChartData Slice(DateTime start, DateTime end) {
            var partialData = Data.Where(d => d.Date >= start && d.Date <= end).ToList();
            return new ChartData(partialData);
        }
    }
}