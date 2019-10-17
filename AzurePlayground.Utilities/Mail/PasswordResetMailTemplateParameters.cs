namespace AzurePlayground.Utilities.Mail {
    public class PasswordResetMailTemplateParameters {
        public string Email { get; }
        public string Token { get; }

        public PasswordResetMailTemplateParameters(string email, string token) {
            Email = email;
            Token = token;
        }
    }
}