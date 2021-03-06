﻿using AzurePlayground.App_Start;
using Microsoft.ApplicationInsights.Extensibility;
using System;
using System.Configuration;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace AzurePlayground {
    public class Global : System.Web.HttpApplication {
        protected void Application_Start(object sender, EventArgs e) {
            var instrumentationKey = ConfigurationManager.AppSettings["ApplicationInsights.InstrumentationKey"];

            if (string.IsNullOrEmpty(instrumentationKey)) {
                TelemetryConfiguration.Active.DisableTelemetry = true;
            }
            else {
                TelemetryConfiguration.Active.InstrumentationKey = instrumentationKey;
            }

            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
    }
}