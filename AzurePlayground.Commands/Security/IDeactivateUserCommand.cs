using AzurePlayground.Models.Security;

namespace AzurePlayground.Commands.Security {
    public interface IDeactivateUserCommand : ICommand<UserDeactivation> {
    }
}