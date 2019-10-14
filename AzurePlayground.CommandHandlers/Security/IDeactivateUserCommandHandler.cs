using AzurePlayground.Models.Security;

namespace AzurePlayground.CommandHandlers.Security {
    public interface IDeactivateUserCommandHandler : ICommandHandler<UserDeactivation> {
    }
}