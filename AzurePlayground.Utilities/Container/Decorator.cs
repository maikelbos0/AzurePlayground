namespace AzurePlayground.Utilities.Container {
    public abstract class Decorator<THandler> {
        public THandler Handler { get; internal set; }
    }
}