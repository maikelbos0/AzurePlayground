namespace AzurePlayground.Domain.Security {
    public enum UserEventType {
        PasswordChanged = 0,
        SuccessfulLogin = 1,
        FailedLogin = 2,
        Registered = 3
    }
}