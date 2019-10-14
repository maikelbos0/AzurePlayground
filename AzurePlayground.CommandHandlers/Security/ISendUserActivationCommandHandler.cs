using AzurePlayground.Models.Security;

namespace AzurePlayground.CommandHandlers.Security {
    public interface ISendUserActivationCommandHandler : ICommandHandler<UserSendActivation> {
    }
}