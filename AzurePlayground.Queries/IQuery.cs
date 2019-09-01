namespace AzurePlayground.Queries {
    public interface IQuery<TParameter, TReturnValue> {
        TReturnValue Execute(TParameter parameter);
    }
}
