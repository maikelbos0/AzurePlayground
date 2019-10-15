namespace AzurePlayground.Commands.Security {
    public class ActivateUserCommand {
        public string Email { get; }
        public string ActivationCode { get; }

        public ActivateUserCommand(string email, string activationCode) {
            Email = email;
            ActivationCode = activationCode;
        }
    }
}