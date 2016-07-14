using System.Web.Mvc;
using ParkingLot.Web.Extensions;

namespace ParkingLot.Web.Controllers {
    public class BaseController : Controller {

        protected ContentResult CreateLargeJsonResponse(object content) {
            return new ContentResult {
                Content = content.ToJson(),
                ContentType = "application/json"
            };
        }

    }
}