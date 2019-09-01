namespace AzurePlayground.Database {
    public interface IPlaygroundContextFactory {
        IPlaygroundContext GetContext();
    }
}