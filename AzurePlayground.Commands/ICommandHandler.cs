
namespace AzurePlayground.Commands {
    public interface ICommandHandler<TParameter> {
        CommandResult<TParameter> Execute(TParameter parameter);
    }
}
