using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using ParkingLot.Web.Models;

namespace ParkingLot.Web.Data {
    public class ApiService : IApiService {        

        public IEnumerable<ParkingRecord> LoadData(string url) {
            var tasks = new List<Task<IEnumerable<ParkingRecord>>> { GetDataTask(url) };
            Task.WaitAll(tasks.ToArray());
            return tasks[0].Result;
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