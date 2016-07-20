using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ParkingLot.Web.Data;
using ParkingLot.Web.Models;
using Should;

namespace ParkingLot.Web.Tests.Data.Builders {

    [TestClass]
    public class ChartDataBuilderTests {
        private ChartDataBuilder _builder;

        [TestInitialize]
        public void SetUp() {
            _builder = new ChartDataBuilder();
        }

        [TestMethod]
        public void Build_should_create_chartData() {
            var parkingRecords = GetParkingRecords();
            var expect = new ChartData(GetExpectChartPoints());

            var result = _builder.Build(parkingRecords);

            Assert.AreEqual(expect.StartDate, result.StartDate);
            Assert.AreEqual(expect.EndDate, result.EndDate);
            Assert.AreEqual(expect.MaxOccupationRate, result.MaxOccupationRate);
            Assert.AreEqual(expect.MaxOccupationDate, result.MaxOccupationDate);
            expect.Data.Count().ShouldEqual(result.Data.Count());
            Assert.IsTrue(expect.Data.All(e => result.Data.Any(r => r.Cars == e.Cars && r.Date == e.Date)));
    }

        private static IEnumerable<ParkingRecord> GetParkingRecords() {
            yield return new ParkingRecord { Id = 0, ArrivalTime = DateTime.Parse("5/5/2016 5:49:00 PM"), LeaveTime = DateTime.Parse("5/6/2016 2:56:00 PM") };
            yield return new ParkingRecord { Id = 1, ArrivalTime = DateTime.Parse("5/2/2016 9:34:00 AM"), LeaveTime = DateTime.Parse("5/2/2016 3:36:00 PM") };
            yield return new ParkingRecord { Id = 2, ArrivalTime = DateTime.Parse("5/3/2016 10:50:00 PM"), LeaveTime = DateTime.Parse("5/4/2016 6:14:00 AM") };
            yield return new ParkingRecord { Id = 3, ArrivalTime = DateTime.Parse("5/1/2016 1:30:00 AM"), LeaveTime = DateTime.Parse("5/1/2016 12:46:00 PM") };
            yield return new ParkingRecord { Id = 4, ArrivalTime = DateTime.Parse("5/4/2016 8:58:00 PM"), LeaveTime = DateTime.Parse("5/5/2016 1:46:00 PM") };
            yield return new ParkingRecord { Id = 5, ArrivalTime = DateTime.Parse("5/5/2016 7:12:00 PM"), LeaveTime = DateTime.Parse("5/5/2016 11:15:00 PM") };
            yield return new ParkingRecord { Id = 6, ArrivalTime = DateTime.Parse("5/3/2016 6:36:00 PM"), LeaveTime = DateTime.Parse("5/4/2016 5:34:00 AM") };
        }

        private static IEnumerable<ChartPoint> GetExpectChartPoints() {
            yield return new ChartPoint { Cars = 1, Date = DateTime.Parse("05/01/2016 01:30:00") };
            yield return new ChartPoint { Cars = 0, Date = DateTime.Parse("05/01/2016 12:47:00") };
            yield return new ChartPoint { Cars = 1, Date = DateTime.Parse("05/02/2016 09:34:00") };
            yield return new ChartPoint { Cars = 0, Date = DateTime.Parse("05/02/2016 15:37:00") };
            yield return new ChartPoint { Cars = 1, Date = DateTime.Parse("05/03/2016 18:36:00") };
            yield return new ChartPoint { Cars = 2, Date = DateTime.Parse("05/03/2016 22:50:00") };
            yield return new ChartPoint { Cars = 1, Date = DateTime.Parse("05/04/2016 05:35:00") };
            yield return new ChartPoint { Cars = 0, Date = DateTime.Parse("05/04/2016 06:15:00") };
            yield return new ChartPoint { Cars = 1, Date = DateTime.Parse("05/04/2016 20:58:00") };
            yield return new ChartPoint { Cars = 0, Date = DateTime.Parse("05/05/2016 13:47:00") };
            yield return new ChartPoint { Cars = 1, Date = DateTime.Parse("05/05/2016 17:49:00") };
            yield return new ChartPoint { Cars = 2, Date = DateTime.Parse("05/05/2016 19:12:00") };
            yield return new ChartPoint { Cars = 1, Date = DateTime.Parse("05/05/2016 23:16:00") };
            yield return new ChartPoint { Cars = 0, Date = DateTime.Parse("05/06/2016 14:57:00") };
        }

    }

}
