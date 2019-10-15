namespace AzurePlayground.Commands.Security {
    public class ChangeUserPasswordCommand : ICommand {
        public string Email { get; }
        public string CurrentPassword { get; }
        public string NewPassword { get; }
        public string ConfirmNewPassword { get; }

        public ChangeUserPasswordCommand(string email, string currentPassword, string newPassword, string confirmNewPassword) {
            Email = email;
            CurrentPassword = currentPassword;
            NewPassword = newPassword;
            ConfirmNewPassword = confirmNewPassword;
        }
    }
}