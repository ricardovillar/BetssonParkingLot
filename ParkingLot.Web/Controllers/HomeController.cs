using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using ParkingLot.Web.Data;
using ParkingLot.Web.Filters;
using ParkingLot.Web.Models;

namespace ParkingLot.Web.Controllers {
    public class HomeController : BaseController {
        private static string ChartDataSessionKey = "ChartData";
        private readonly IChartDataBuilder _chartDataBuilder;

        public HomeController(IChartDataBuilder chartDataBuilder) {
            _chartDataBuilder = chartDataBuilder;
        }

        public ActionResult Index() {
            var config = new Config {
                ApiUrl = ConfigurationManager.AppSettings["ApiUrl"],
                GetDataUrl = Url.Action("GetData", "Home", new { apiUrl = "(apiUrl)" }),
                GetPartialDataUrl = Url.Action("GetPartialData", "Home", new { start = "(start)", end = "(end)" }),
                GetFullDataUrl = Url.Action("GetFullData")
            };
            return View(config);
        }

        [JsonHandleError]
        public ContentResult GetData(string apiUrl) {
            var tasks = new List<Task<IEnumerable<ParkingRecord>>> { GetDataTask(apiUrl) };
            Task.WaitAll(tasks.ToArray());
            var records = tasks[0].Result;
            var chartData = _chartDataBuilder.Build(records);
            StoreChartData(chartData);
            return CreateLargeJsonResponse(chartData);
        }

        [JsonHandleError]
        public ContentResult GetPartialData(DateTime start, DateTime end) {
            try {
                if (start >= end) {
                    throw new ArgumentOutOfRangeException("start", "Start Date must be before than End Date.");
                }
                var chartData = GetStoredChartData();
                var partialChartData = chartData.Slice(start, end);
                return CreateLargeJsonResponse(partialChartData);
            }
            catch (ArgumentOutOfRangeException ex) {
                throw new HttpException((int) HttpStatusCode.BadRequest, ex.Message);
            }
            catch (InvalidDataException ex) {
                throw new HttpException((int) HttpStatusCode.InternalServerError, ex.Message);
            }
            catch (Exception) {
                throw new HttpException((int) HttpStatusCode.InternalServerError, "Something went wrong, sorry!");
            }
        }

        [JsonHandleError]
        public ContentResult GetFullData() {
            try {
                var chartData = GetStoredChartData();
                return CreateLargeJsonResponse(chartData);
            }
            catch (InvalidDataException ex) {
                throw new HttpException((int) HttpStatusCode.InternalServerError, ex.Message);
            }
            catch (Exception) {
                throw new HttpException((int) HttpStatusCode.InternalServerError, "Something went wrong, sorry!");
            }
        }

        private void StoreChartData(ChartData chartData) {
            Session[ChartDataSessionKey] = chartData;
        }

        private ChartData GetStoredChartData() {
            var chartData = (ChartData) Session[ChartDataSessionKey];
            if (chartData == null) {
                throw new InvalidDataException("Data not found.");
            }
            return chartData;
        }

        static async Task<IEnumerable<ParkingRecord>> GetDataTask(string url) {
            IEnumerable<ParkingRecord> result = new List<ParkingRecord>();
            using (var client = new HttpClient()) {
                var uri = new Uri(url);
                client.BaseAddress = new Uri(uri.AbsoluteUri.TrimEnd(uri.Query.ToCharArray()));
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = client.GetAsync(uri.Query).Result;
                if (response.IsSuccessStatusCode) {
                    result = await response.Content.ReadAsAsync<IEnumerable<ParkingRecord>>();
                }
            }
            return result;
        }


    }
}