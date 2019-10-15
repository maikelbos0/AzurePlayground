namespace AzurePlayground.Commands.Security {
    public class LogOutUserCommand : ICommand {
        public string Email { get; }

        public LogOutUserCommand(string email) {
            Email = email;
        }
    }
}