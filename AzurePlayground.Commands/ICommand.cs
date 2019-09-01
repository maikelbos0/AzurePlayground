namespace AzurePlayground.Commands {
    public interface ICommand<TParameter> {
        void Execute(TParameter parameter);
    }

    public interface ICommand<TParameter, TReturnValue> {
        TReturnValue Execute(TParameter parameter);
    }
}
