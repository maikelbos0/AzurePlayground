namespace AzurePlayground.Services {
    public interface IAuthenticationService {
        string GetIdentity();
        void SignIn(string identity);
        void SignOut();
    }
}