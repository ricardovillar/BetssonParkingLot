using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Mvc;
using ParkingLot.Web.Data;
using ParkingLot.Web.Models;
using ParkingLot.Web.Extensions;

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
                GetDataUrl = Url.Action("GetData", "Home", new {  apiUrl = "(apiUrl)" }),
                GetPartialDataUrl = Url.Action("GetPartialData", "Home", new { start = "(start)", end = "(end)" }),
                GetFullDataUrl = Url.Action("GetFullData")
            };
            return View(config);
        }

        public ContentResult GetData(string apiUrl) {
            var tasks = new List<Task<IEnumerable<ParkingRecord>>> { GetDataTask(apiUrl) };
            Task.WaitAll(tasks.ToArray());
            var records = tasks[0].Result;
            var chartData = _chartDataBuilder.Build(records);
            StoreChartData(chartData);
            return CreateLargeJsonResponse(chartData);
        }

        public ContentResult GetPartialData(DateTime start, DateTime end) {
            var chartData = GetStoredChartData();
            var partialChartData = chartData.Slice(start, end);
            return CreateLargeJsonResponse(partialChartData);
        }
        
        public ContentResult GetFullData() {
            var chartData = GetStoredChartData();
            return CreateLargeJsonResponse(chartData);
        }

        private void StoreChartData(ChartData chartData) {
            Session[ChartDataSessionKey] = chartData;
        }

        private ChartData GetStoredChartData() {
            return (ChartData) Session[ChartDataSessionKey];
        }

        private string RequestData(string url) {
            var request = WebRequest.Create(url);
            var stream = request.GetResponse().GetResponseStream();
            var reader = new StreamReader(stream);
            return reader.ReadToEnd();
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