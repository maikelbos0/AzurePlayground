﻿namespace AzurePlayground.Services {
    public interface IAuthenticationService {
        bool IsAuthenticated { get; }
        string Identity { get; }
        string GetUserName();
        void SignIn(string identity);
        void SignOut();
    }
}