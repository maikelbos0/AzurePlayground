namespace AzurePlayground.Commands.Security {
    public class SendUserActivationCommand : ICommand {
        public string Email { get; }

        public SendUserActivationCommand(string email) {
            Email = email;
        }
    }
}