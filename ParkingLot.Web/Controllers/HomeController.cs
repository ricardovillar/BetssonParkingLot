using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Web.Mvc;
using Newtonsoft.Json;
using ParkingLot.Web.Data;
using ParkingLot.Web.Models;


namespace ParkingLot.Web.Controllers {
    public class HomeController : BaseController {
        public ActionResult Index() {
            return View();
        }

        public ContentResult GetData(string url = "http://parkingapi.gear.host/v1/parking?days=60&items=200000") {
            var jsonData = RequestData(url);
            var records = JsonConvert.DeserializeObject<List<ParkingRecord>>(jsonData);
            var chartData = new ChartDataBuilder().Build(records);           
            return CreateLargeJsonResponse(chartData);
        }


        private string RequestData(string url) {
            var request = WebRequest.Create(url);
            var stream = request.GetResponse().GetResponseStream();
            var reader = new StreamReader(stream);
            return reader.ReadToEnd();
        }


    }
}