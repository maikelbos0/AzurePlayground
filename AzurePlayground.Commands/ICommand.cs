
namespace AzurePlayground.Commands {
    public interface ICommand<TParameter> {
        CommandResult Execute(TParameter parameter);
    }
}
