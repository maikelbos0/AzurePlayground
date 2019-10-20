using AzurePlayground.Utilities.Configuration;
using System.Collections.Generic;

namespace AzurePlayground.Test.Utilities {
    public sealed class FakeAppSettings : IAppSettings {
        public Dictionary<string, string> Settings { get; set; }

        public string this[string name] {
            get {
                return Settings[name];
            }
            set {
                Settings[name] = value;
            }
        }
    }
}