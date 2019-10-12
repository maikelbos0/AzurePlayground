using AzurePlayground.Database;
using AzurePlayground.Models.Security;
using AzurePlayground.Utilities.Container;

namespace AzurePlayground.Commands.Security {
    [Injectable]
    public class DeactivateUserCommand : IDeactivateUserCommand {
        private readonly IPlaygroundContextFactory _playgroundContextFactory;

        public DeactivateUserCommand(IPlaygroundContextFactory playgroundContextFactory) {
            _playgroundContextFactory = playgroundContextFactory;
        }

        public CommandResult<UserDeactivation> Execute(UserDeactivation parameter) {
            throw new System.NotImplementedException();
        }
    }
}