using AzurePlayground.Models.Security;

namespace AzurePlayground.Commands.Security {
    public interface ISendUserActivationCommandHandler : ICommandHandler<UserSendActivation> {
    }
}