using AzurePlayground.Models.Security;

namespace AzurePlayground.Commands.Security {
    public interface IRequestUserPasswordResetCommand : ICommand<UserPasswordResetRequest> {
    }
}
