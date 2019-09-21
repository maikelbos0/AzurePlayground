using AzurePlayground.Models.Security;

namespace AzurePlayground.Commands.Security {
    public interface IResetUserPasswordCommand : ICommand<UserPasswordReset> {
    }
}
