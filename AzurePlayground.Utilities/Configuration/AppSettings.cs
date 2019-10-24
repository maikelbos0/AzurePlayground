using AzurePlayground.Utilities.Container;
using System.Configuration;

namespace AzurePlayground.Utilities.Configuration {
    [InterfaceInjectable]
    public sealed class AppSettings : IAppSettings {
        public string this[string name] {
            get {
                return ConfigurationManager.AppSettings[name];
            }
        }
    }
}
