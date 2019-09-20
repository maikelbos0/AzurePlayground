using AzurePlayground.Models.Security;

namespace AzurePlayground.Commands.Security {
    public interface IChangeUserPasswordCommand : ICommand<UserChangePassword> {
    }
}