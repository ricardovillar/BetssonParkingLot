using System;
using System.Linq;
using System.Collections.Generic;

namespace ParkingLot.Web.Models {
    public class ChartData {
        public DateTime? StartDate { get; private set; }
        public DateTime? EndDate { get; private set; }
        public DateTime? MaxOccupationDate { get; private set; }
        public int MaxOccupationRate { get; private set; }
        public IEnumerable<ChartPoint> Data { get; private set; }

        public ChartData(IEnumerable<ChartPoint> data) {
            Data = data;
            if (data.Any()) {
                StartDate = Data.Min(d => d.Date);
                EndDate = Data.Max(d => d.Date);
                MaxOccupationRate = Data.Max(d => d.Cars);
                MaxOccupationDate = Data.Where(d => d.Cars == MaxOccupationRate).First().Date;
            }
        }

        public ChartData Slice(DateTime start, DateTime end) {
            var partialData = Data.Where(d => d.Date >= start && d.Date <= end).ToList();
            return new ChartData(partialData);
        }
    }
}