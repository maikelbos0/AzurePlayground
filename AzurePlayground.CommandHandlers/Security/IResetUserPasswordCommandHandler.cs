using AzurePlayground.Models.Security;

namespace AzurePlayground.CommandHandlers.Security {
    public interface IResetUserPasswordCommandHandler : ICommandHandler<UserPasswordReset> {
    }
}