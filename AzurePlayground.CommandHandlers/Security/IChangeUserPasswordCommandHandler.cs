using AzurePlayground.Models.Security;

namespace AzurePlayground.CommandHandlers.Security {
    public interface IChangeUserPasswordCommandHandler : ICommandHandler<UserChangePassword> {
    }
}