namespace AzurePlayground.Domain.Security {
    public enum UserEventType {
        PasswordChanged = 0,
        LoggedIn = 1,
        FailedLogin = 2,
        Registered = 3,
        Activated = 4,
        FailedActivation = 5,
        ActivationCodeSent = 6
    }
}