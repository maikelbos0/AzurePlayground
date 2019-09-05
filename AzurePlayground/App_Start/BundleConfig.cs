﻿using System.Web.Optimization;

namespace AzurePlayground.App_Start {
    public class BundleConfig {
        public static void RegisterBundles(BundleCollection bundles) {
            bundles.Add(new ScriptBundle("~/Scripts/jquery").Include(
                "~/Scripts/jquery-{version}.js",
                "~/Scripts/jquery.validate.js",
                "~/Scripts/jquery.validate.unobtrusive.js",
                "~/Scripts/toastr.js"));

            bundles.Add(new ScriptBundle("~/Scripts/bootstrap").Include(
                "~/Scripts/bootstrap.js"));

            bundles.Add(new ScriptBundle("~/Scripts/application").Include(
                "~/Scripts/application/jquery-ajaxform.js",
                "~/Scripts/application/toastr-defaults.js"));

            bundles.Add(new StyleBundle("~/Content/css-bootstrap").Include(
                "~/Content/bootstrap.css"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                "~/Content/Site.css",
                "~/Content/toastr.css"));
        }
    }
}