namespace AzurePlayground.Commands.Security {
    public class ForgotUserPasswordCommand : ICommand {
        public string Email { get; }

        public ForgotUserPasswordCommand(string email) {
            Email = email;
        }
    }
}