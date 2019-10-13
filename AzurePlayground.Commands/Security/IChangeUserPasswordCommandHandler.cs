using AzurePlayground.Models.Security;

namespace AzurePlayground.Commands.Security {
    public interface IChangeUserPasswordCommandHandler : ICommandHandler<UserChangePassword> {
    }
}