using AzurePlayground.Utilities.Container;
using Newtonsoft.Json;

namespace AzurePlayground.Utilities.Serialization {
    [InterfaceInjectable]
    public sealed class Serializer : ISerializer {
        public string SerializeToJson(object obj) {
            return JsonConvert.SerializeObject(obj);
        }
    }
}