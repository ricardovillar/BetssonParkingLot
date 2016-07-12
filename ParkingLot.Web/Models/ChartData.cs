using System;
using System.Collections.Generic;

namespace ParkingLot.Web.Models {
    public struct ChartData {
        //public DateTime StartDate;
        //public DateTime EndDate;
        public string StartDate;
        public string EndDate;
        public TimeSpan TopOccupationTime;
        public int MaxOccupation;
        public IEnumerable<ChartPoint> Data;
    }
}