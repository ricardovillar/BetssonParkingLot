using System;
using System.Linq;
using System.Collections.Generic;
using ParkingLot.Web.Models;
using System.Collections.Concurrent;

namespace ParkingLot.Web.Data {
    public class ChartDataBuilder : IChartDataBuilder {
        public ChartData Build(IEnumerable<ParkingRecord> records) {
            var parkData = MapRecordsToMinutes(records);

            var points = parkData
                            .Where(d => d.Value >= 0)
                            .OrderBy(d => d.Key)
                            .Select(d => new ChartPoint { Date = d.Key, Cars = d.Value })
                            .Distinct()
                            .ToList();

            var topOcuppationCars = parkData.Values.Max();
            var topOccupationTime = parkData.First(x => x.Value == topOcuppationCars).Key;

            return new ChartData(points);
        }        

        private IDictionary<DateTime, int> MapRecordsToMinutes(IEnumerable<ParkingRecord> records) {
            var minutesMap = new ConcurrentDictionary<DateTime, int>();

            records.AsParallel()
                    .ForAll(record => {
                        minutesMap.AddOrUpdate(record.ArrivalTime, 1, (id, count) => count++);
                        minutesMap.AddOrUpdate(record.LeaveTime.AddMinutes(1), -1, (id, count) => count--);
                    });

            var result = new Dictionary<DateTime, int>();
            var occupation = 0;
            foreach(var movement in minutesMap.OrderBy(m => m.Key)) {
                occupation += movement.Value;
                result.Add(movement.Key, occupation);
            }

            return result;
        }

    }
}