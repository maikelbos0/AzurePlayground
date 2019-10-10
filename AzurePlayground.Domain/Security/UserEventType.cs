﻿namespace AzurePlayground.Domain.Security {
    public enum UserEventType : byte {
        LoggedIn = 1,
        FailedLogIn = 2,
        Registered = 3,
        Activated = 4,
        FailedActivation = 5,
        ActivationCodeSent = 6,
        LoggedOut = 7,
        PasswordChanged = 8,
        FailedPasswordChange = 9,
        PasswordResetRequested = 10,
        PasswordReset = 11,
        FailedPasswordReset = 12
    }
}