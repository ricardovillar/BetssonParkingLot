using System.Web;
using System.Web.Optimization;

namespace ParkingLot.Web {
    public class BundleConfig {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles) {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            bundles.Add(new ScriptBundle("~/bundles/angular")
                .Include("~/Scripts/angular.min.js")
                .Include("~/Scripts/angular-animate.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/ParkingLotApp")
                .IncludeDirectory("~/Scripts/Controllers", "*.js")
                .Include("~/Scripts/moment.min.js")
                .Include("~/Scripts/datetimepicker.js")
                .Include("~/Scripts/datetimepicker.templates.js")
                .Include("~/Scripts/toaster.min.js")
                .Include("~/Scripts/ParkingLotApp.js"));

            bundles.Add(new ScriptBundle("~/bundles/chart").Include(
            "~/Scripts/chart.bundle.min.js"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));
            
            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/datetimepicker.css",
                      "~/Content/toaster.min.css",
                      "~/Content/mobile.css",
                      "~/Content/screen.css"));
        }
    }
}
