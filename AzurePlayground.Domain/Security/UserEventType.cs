namespace AzurePlayground.Domain.Security {
    public enum UserEventType : byte {
        PasswordChanged = 0,
        LoggedIn = 1,
        FailedLogIn = 2,
        Registered = 3,
        Activated = 4,
        FailedActivation = 5,
        ActivationCodeSent = 6,
        LoggedOut = 7,
        FailedPasswordChange = 8,
        PasswordResetRequested = 9,
        PasswordReset = 10,
        FailedPasswordReset = 11
    }
}