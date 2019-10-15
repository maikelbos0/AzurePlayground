namespace AzurePlayground.Commands.Security {
    public class LogOutUserCommand {
        public string Email { get; }

        public LogOutUserCommand(string email) {
            Email = email;
        }
    }
}