using System.Web;
using System.Web.Optimization;

namespace NWTradersWeb
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            #region Scripts
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Data Tables
            bundles.Add(new Bundle("~/bundles/DataTables").Include(
                      "~/Scripts/DataTables/jquery.dataTables.*"));

            bundles.Add(new Bundle("~/bundles/DataTableButtons").Include(
                      "~/Scripts/DataTables/dataTables.buttons.*"));

            bundles.Add(new Bundle("~/bundles/Buttons").Include(
                      "~/Scripts/DataTables/buttons.html5.*"));

            bundles.Add(new Bundle("~/bundles/ButtonsPrint").Include(
                      "~/Scripts/DataTables/buttons.print.*"));

            // PDFMake
            bundles.Add(new Bundle("~/bundles/PDFMake").Include(
                      "~/Scripts/pdfmake/pdfmake.min.js",
                      "~/Scripts/pdfmake/vfs_fonts.js"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at https://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js"));

            bundles.Add(new ScriptBundle("~/bundles/Rainbow").Include(
                      "~/Scripts/RainbowVis.js"));
            #endregion

            bundles.Add(new StyleBundle("~/Content/css").Include(
                                "~/Content/bootstrap.css",
                                "~/Content/w3.css",
                                "~/Content/font-awesome/css/all.css",
                                "~/Content/font-awesome/css/all.min.css",
                                "~/Content/material-icons.css",
                                "~/Content/PagedList.css",
                                "~/Content/NWTraders.css"
                    ));
           

        }
    }
}
