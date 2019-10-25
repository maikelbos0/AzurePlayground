namespace AzurePlayground.Commands.Security {
    public sealed class EditUserInformationCommand : ICommand {
        public string Email { get; private set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }
        public bool ShowEmail { get; set; }

        public EditUserInformationCommand(string email) {
            Email = email;
        }
    }
}