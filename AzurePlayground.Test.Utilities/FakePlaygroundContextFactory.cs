using AzurePlayground.Database;

namespace AzurePlayground.Test.Utilities {
    public class FakePlaygroundContextFactory : IPlaygroundContextFactory {
        public IPlaygroundContext Context { get; private set; } = new FakePlaygroundContext();

        public IPlaygroundContext GetContext() {
            return Context;
        }
    }
}