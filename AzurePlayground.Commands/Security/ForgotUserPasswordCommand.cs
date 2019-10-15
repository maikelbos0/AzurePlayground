namespace AzurePlayground.Commands.Security {
    public class ForgotUserPasswordCommand {
        public string Email { get; }

        public ForgotUserPasswordCommand(string email) {
            Email = email;
        }
    }
}