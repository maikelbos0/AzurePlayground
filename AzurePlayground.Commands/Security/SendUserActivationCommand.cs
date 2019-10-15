namespace AzurePlayground.Commands.Security {
    public class SendUserActivationCommand {
        public string Email { get; }

        public SendUserActivationCommand(string email) {
            Email = email;
        }
    }
}