namespace AzurePlayground.Providers {
    public interface IAuthenticationProvider {
        string GetIdentity();
        void SignIn(string identity);
        void SignOut();
    }
}