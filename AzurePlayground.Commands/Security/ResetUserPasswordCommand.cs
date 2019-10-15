namespace AzurePlayground.Commands.Security {
    public class ResetUserPasswordCommand {
        public string Email { get; }
        public string PasswordResetToken { get; }
        public string NewPassword { get; }
        public string ConfirmNewPassword { get; }

        public ResetUserPasswordCommand(string email, string passwordResetToken, string newPassword, string confirmNewPassword) {
            Email = email;
            PasswordResetToken = passwordResetToken;
            NewPassword = newPassword;
            ConfirmNewPassword = confirmNewPassword;
        }
    }
}