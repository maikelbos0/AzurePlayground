using AzurePlayground.Models.Security;

namespace AzurePlayground.Commands.Security {
    public interface IUserCommands {
        UserRegistration Register(UserRegistration userRegistration);
    }
}