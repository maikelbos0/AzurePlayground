using AzurePlayground.Models.Security;

namespace AzurePlayground.Commands.Security {
    public interface ISendUserActivationCommand : ICommand<UserSendActivation> {
    }
}