using Microsoft.ApplicationInsights.Extensibility;
using System;
using System.Configuration;

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
        }
    }
}