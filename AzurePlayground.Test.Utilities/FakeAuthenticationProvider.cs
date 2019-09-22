using AzurePlayground.Providers;

namespace AzurePlayground.Test.Utilities {
    public class FakeAuthenticationProvider : IAuthenticationProvider {
        public string Identity { get; private set; }

        public string GetIdentity() {
            return Identity;
        }

        public void SignIn(string identity) {
            Identity = identity;
        }

        public void SignOut() {
            Identity = null;
        }
    }
}