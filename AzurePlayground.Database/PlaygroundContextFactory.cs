using AzurePlayground.Utilities.Container;

namespace AzurePlayground.Database {
    [Injectable]
    public class PlaygroundContextFactory : IPlaygroundContextFactory {
        public IPlaygroundContext GetContext() {
            return new PlaygroundContext();
        }
    }
}