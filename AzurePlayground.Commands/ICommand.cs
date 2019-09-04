
namespace AzurePlayground.Commands {
    public interface ICommand<TParameter> {
        CommandResult<TParameter> Execute(TParameter parameter);
    }
}
