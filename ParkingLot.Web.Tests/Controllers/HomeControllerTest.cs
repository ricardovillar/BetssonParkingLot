using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Collections.Generic;
using Moq;
using Should;
using ParkingLot.Web.Controllers;
using ParkingLot.Web.Data;
using ParkingLot.Web.Models;
using ParkingLot.Web.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;


namespace ParkingLot.Web.Tests.Controllers {

    [TestClass]
    public class HomeControllerTest {
        private Mock<HttpContextBase> _httpContextBaseMock;
        private Mock<HttpSessionStateBase> _sessionMock;
        private Mock<ControllerContext> _controllerContextMock;
        private Mock<IChartDataBuilder> _chartDataBuilderMock;
        private Mock<IApiService> _apiServiceMock;
        private HomeController _controller;

        [TestInitialize]
        public void SetUp() {
            _httpContextBaseMock = new Mock<HttpContextBase>();
            _sessionMock = new Mock<HttpSessionStateBase>();
            _controllerContextMock = new Mock<ControllerContext>();
            _chartDataBuilderMock = new Mock<IChartDataBuilder>();
            _apiServiceMock = new Mock<IApiService>();
            _controller = new HomeController(_chartDataBuilderMock.Object, _apiServiceMock.Object);
        }

        private void MockControllerUrl() {
            var httpContext = Substitute.For<HttpContextBase>();
            var requestContext = new RequestContext(httpContext, new RouteData());
            _controller.Url = new UrlHelper(requestContext);
        }

        private void MockHttpContext() {
            _controllerContextMock.SetupGet(x => x.HttpContext).Returns(_httpContextBaseMock.Object);
            _controllerContextMock.SetupGet(x => x.HttpContext.Session).Returns(_sessionMock.Object);
            _controller.ControllerContext = _controllerContextMock.Object;
        }

        [TestMethod]
        public void Index_should_have_a_config_model() {
            MockControllerUrl();
            var result = _controller.Index();
            var model = (result as ViewResult).Model;

            model.ShouldBeType<Config>();
        }

        [TestMethod]
        public void Index_should_have_right_config_ApiUrl() {
            MockControllerUrl();
            var result = _controller.Index();
            var config = (Config) (result as ViewResult).Model;

            config.ApiUrl.ShouldEqual("api.url.from.the.app.settings");
        }

        [TestMethod]
        public void Index_should_have_right_config_GetDataUrl() {
            MockControllerUrl();
            var result = _controller.Index();
            var config = (Config) (result as ViewResult).Model;

            config.GetDataUrl.ShouldEqual(_controller.Url.Action("GetData", "Home", new { apiUrl = "(apiUrl)" }));
        }

        [TestMethod]
        public void Index_should_have_right_config_GetPartialDataUrl() {
            MockControllerUrl();
            var result = _controller.Index();
            var config = (Config) (result as ViewResult).Model;

            config.GetPartialDataUrl.ShouldEqual(_controller.Url.Action("GetPartialData", "Home", new { start = "(start)", end = "(end)" }));
        }

        [TestMethod]
        public void Index_should_have_right_config_GetData() {
            MockControllerUrl();
            var result = _controller.Index();
            var config = (Config) (result as ViewResult).Model;

            config.GetDataUrl.ShouldEqual(_controller.Url.Action("GetData", "Home", new { apiUrl = "(apiUrl)" }));
        }

        [TestMethod]
        public void GetData_should_return_built_data_loaded_from_api_service() {
            MockHttpContext();
            var apiUrl = Guid.NewGuid().ToString();
            var parkingRecords = GetParkingRecords();
            var chartData = new ChartData(parkingRecords.Select(r => new ChartPoint { Cars = r.Id, Date = r.ArrivalTime }));
            _apiServiceMock.Setup(s => s.LoadData(apiUrl)).Returns(parkingRecords);
            _chartDataBuilderMock.Setup(s => s.Build(parkingRecords)).Returns(chartData);

            var result = _controller.GetData(apiUrl);

            result.ContentType.ShouldEqual("application/json");
            result.Content.ShouldEqual(chartData.ToJson());
        }

        [TestMethod]
        public void GetData_should_store_chart_data() {
            MockHttpContext();
            var apiUrl = Guid.NewGuid().ToString();
            var parkingRecords = GetParkingRecords();
            var chartData = new ChartData(parkingRecords.Select(r => new ChartPoint { Cars = r.Id, Date = r.ArrivalTime }));
            _apiServiceMock.Setup(s => s.LoadData(apiUrl)).Returns(parkingRecords);
            _chartDataBuilderMock.Setup(s => s.Build(parkingRecords)).Returns(chartData);

            _controller.GetData(apiUrl);

            _sessionMock.VerifySet(Session => Session["ChartData"] = chartData);
        }

        [TestMethod]
        [ExpectedException(typeof(HttpException), "Something went wrong, sorry!")]
        public void GetData_should_handle_Exception() {
            var apiUrl = Guid.NewGuid().ToString();
            _apiServiceMock.Setup(s => s.LoadData(apiUrl)).Throws(new Exception());

            _controller.GetData(apiUrl);
        }

        [TestMethod]
        public void GetFullData_should_return_stored_chart_data() {
            MockHttpContext();
            var parkingRecords = GetParkingRecords();
            var chartData = new ChartData(parkingRecords.Select(r => new ChartPoint { Cars = r.Id, Date = r.ArrivalTime }));
            _sessionMock.SetupGet(Session => Session["ChartData"]).Returns(chartData);

            var result = _controller.GetFullData();

            result.ContentType.ShouldEqual("application/json");
            result.Content.ShouldEqual(chartData.ToJson());
        }

        [TestMethod]
        [ExpectedException(typeof(HttpException), "Data not found.")]
        public void GetFullData_should_handle_InvalidDataException() {
            MockHttpContext();
            _sessionMock.SetupGet(Session => Session["ChartData"]).Returns(null);

            var result = _controller.GetFullData();
        }

        [TestMethod]
        [ExpectedException(typeof(HttpException), "Something went wrong, sorry!")]
        public void GetFullData_should_handle_Exception() {
            MockHttpContext();
            _sessionMock.SetupGet(Session => Session["ChartData"]).Throws(new Exception());

            var result = _controller.GetFullData();
        }

        [TestMethod]
        [ExpectedException(typeof(HttpException), "Start Date must be before than End Date.")]
        public void GetPartialData_should_verify_if_start_is_less_then_end() {
            var start = DateTime.Now;
            var end = start.AddMilliseconds(-1);

            _controller.GetPartialData(start, end);
        }

        [TestMethod]
        public void GetPartialData_should_return_partial_stored_chart_data() {
            MockHttpContext();
            var parkingRecords = GetParkingRecords();
            var chartData = new ChartData(parkingRecords.Select(r => new ChartPoint { Cars = r.Id, Date = r.ArrivalTime }));
            _sessionMock.SetupGet(Session => Session["ChartData"]).Returns(chartData);
            var parkingRecord = parkingRecords.Single(p => p.Id == 2);
            var start = parkingRecord.ArrivalTime.AddMilliseconds(-1);
            var end = parkingRecord.LeaveTime.AddMilliseconds(1);

            var result = _controller.GetPartialData(start, end);

            result.ContentType.ShouldEqual("application/json");
            result.Content.ShouldEqual(chartData.Slice(start, end).ToJson());
        }

        [TestMethod]
        [ExpectedException(typeof(HttpException), "Data not found.")]
        public void GetPartialData_should_handle_InvalidDataException() {
            MockHttpContext();
            _sessionMock.SetupGet(Session => Session["ChartData"]).Returns(null);
            var start = DateTime.Now;
            var end = start.AddMilliseconds(1);

            _controller.GetPartialData(start, end);
        }

        [TestMethod]
        [ExpectedException(typeof(HttpException), "Something went wrong, sorry!")]
        public void GetPartialData_should_handle_Exception() {
            var start = DateTime.Now;
            var end = start.AddMilliseconds(1);

            _controller.GetPartialData(start, end);
        }

        private static List<ParkingRecord> GetParkingRecords() {
            return new List<ParkingRecord> {
                new ParkingRecord { Id = 1, ArrivalTime = DateTime.Now.AddDays(3), LeaveTime = DateTime.Now.AddDays(3).AddHours(3) },
                new ParkingRecord { Id = 2, ArrivalTime = DateTime.Now.AddDays(-2), LeaveTime = DateTime.Now.AddDays(-2).AddHours(5) },
                new ParkingRecord { Id = 3, ArrivalTime = DateTime.Now, LeaveTime = DateTime.Now.AddMinutes(30) }
            };
        }

    }
}
