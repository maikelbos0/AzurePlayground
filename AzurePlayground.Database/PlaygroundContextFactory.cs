using AzurePlayground.Utilities.Container;

namespace AzurePlayground.Database {
    [Register]
    public class PlaygroundContextFactory : IPlaygroundContextFactory {
        public IPlaygroundContext GetContext() {
            return new PlaygroundContext();
        }
    }
}