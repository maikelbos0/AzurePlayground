namespace AzurePlayground.Commands.Security {
    public class LogInUserCommand : ICommand {
        public string Email { get; }
        public string Password { get; }

        public LogInUserCommand(string email, string password) {
            Email = email;
            Password = password;
        }
    }
}
