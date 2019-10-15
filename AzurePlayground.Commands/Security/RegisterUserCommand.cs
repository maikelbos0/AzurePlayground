namespace AzurePlayground.Commands.Security {
    public class RegisterUserCommand {
        public string Email { get; }
        public string Password { get; }
        public string ConfirmPassword { get; }

        public RegisterUserCommand(string email, string password, string confirmPassword) {
            Email = email;
            Password = password;
            ConfirmPassword = confirmPassword;
        }
    }
}