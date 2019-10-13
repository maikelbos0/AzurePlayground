using AzurePlayground.Models.Security;

namespace AzurePlayground.Commands.Security {
    public interface IDeactivateUserCommandHandler : ICommandHandler<UserDeactivation> {
    }
}