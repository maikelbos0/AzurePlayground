namespace AzurePlayground.Utilities.Mail {
    public class ActivationMailTemplateParameters {
        public string Email { get; }
        public int ActivationCode { get; }

        public ActivationMailTemplateParameters(string email, int activationCode) {
            Email = email;
            ActivationCode = activationCode;
        }
    }
}