using AzurePlayground.Models.Security;

namespace AzurePlayground.Commands.Security {
    public interface IRegisterUserCommand : ICommand<UserRegistration> {
    }
}