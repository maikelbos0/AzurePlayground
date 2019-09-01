namespace AzurePlayground.Database {
    public class PlaygroundContextFactory : IPlaygroundContextFactory {
        public IPlaygroundContext GetContext() {
            return new PlaygroundContext();
        }
    }
}