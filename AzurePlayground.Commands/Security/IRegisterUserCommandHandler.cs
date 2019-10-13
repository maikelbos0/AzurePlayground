using AzurePlayground.Models.Security;

namespace AzurePlayground.Commands.Security {
    public interface IRegisterUserCommandHandler : ICommandHandler<UserRegistration> {
    }
}