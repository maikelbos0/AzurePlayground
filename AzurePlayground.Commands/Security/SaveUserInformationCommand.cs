namespace AzurePlayground.Commands.Security {
    public sealed class SaveUserInformationCommand : ICommand {
        public string Email { get; private set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }
        public bool ShowEmail { get; set; }

        public SaveUserInformationCommand(string email) {
            Email = email;
        }
    }
}