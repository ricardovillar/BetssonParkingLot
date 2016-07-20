using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ParkingLot.Web.Models;
using Should;

namespace ParkingLot.Web.Tests.Models {
    [TestClass]
    public class ChartDataTests {

        [TestMethod]
        public void ChartData_should_be_empty() {
            var chartData = new ChartData(new List<ChartPoint>());

            chartData.Data.Count().ShouldEqual(0);
            chartData.StartDate.ShouldBeNull();
            chartData.EndDate.ShouldBeNull();
            chartData.MaxOccupationDate.ShouldBeNull();
            chartData.MaxOccupationRate.ShouldEqual(0);
        }

        [TestMethod]
        public void ChartData_should_have_calculated_values() {
            var chartPoints = GetExpectChartPoints();
            var chartData = new ChartData(chartPoints);

            chartData.Data.Count().ShouldEqual(chartPoints.Count());
            Assert.IsTrue(chartData.Data.All(e => chartPoints.Any(r => r.Cars == e.Cars && r.Date == e.Date)));
            chartData.StartDate.ShouldEqual(DateTime.Parse("05/01/2016 01:30:00"));
            chartData.EndDate.ShouldEqual(DateTime.Parse("05/06/2016 14:57:00"));
            chartData.MaxOccupationDate.ShouldEqual(DateTime.Parse("05/03/2016 22:50:00"));
            chartData.MaxOccupationRate.ShouldEqual(2);
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
