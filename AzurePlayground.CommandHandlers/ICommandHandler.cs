
namespace AzurePlayground.CommandHandlers {
    public interface ICommandHandler<TParameter> {
        CommandResult<TParameter> Execute(TParameter parameter);
    }
}
