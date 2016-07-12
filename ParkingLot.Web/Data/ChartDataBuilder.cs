using System;
using System.Linq;
using System.Collections.Generic;
using ParkingLot.Web.Models;

namespace ParkingLot.Web.Data {
    public class ChartDataBuilder : IChartDataBuilder {
        public ChartData Build(IEnumerable<ParkingRecord> records) {
            var occupationData = MapRecordsToOccupation(records);

            var points = from occupation in occupationData
                         group occupation by occupation into g
                         select new ChartPoint {
                             Date = g.Key.ToString("dd/MM/yyyy HH:mm:ss"),
                             Cars = g.Count()
                         };
            return new ChartData {
                StartDate = occupationData.Min().ToString("dd/MM/yyyy HH:mm:ss"),
                EndDate = occupationData.Max().ToString("dd/MM/yyyy HH:mm:ss"),
                MaxOccupation = points.Max(p => p.Cars),
                Data = points
            };
        }

        private List<DateTime> MapRecordsToOccupation(IEnumerable<ParkingRecord> records) {
            return records
                    .AsParallel()
                    .SelectMany(record => MapIntervalToOccupation(record.ArrivalTime, record.LeaveTime))
                    .ToList();
        }

        private List<DateTime> MapIntervalToOccupation(DateTime arrivalTime, DateTime leaveTime) {
            var result = new List<DateTime>();
            var minute = arrivalTime;
            while (minute <= leaveTime) {
                result.Add(minute);
                minute = minute.AddMinutes(1);
            }
            return result;
        }
    }
}