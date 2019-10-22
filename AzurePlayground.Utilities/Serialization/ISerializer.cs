namespace AzurePlayground.Utilities.Serialization {
    public interface ISerializer {
        string SerializeToJson(object obj);
    }
}