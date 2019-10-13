using AzurePlayground.Models.Security;

namespace AzurePlayground.Commands.Security {
    public interface IResetUserPasswordCommandHandler : ICommandHandler<UserPasswordReset> {
    }
}