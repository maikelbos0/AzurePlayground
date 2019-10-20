using AzurePlayground.Services;

namespace AzurePlayground.Test.Utilities {
    public sealed class FakeAuthenticationService : IAuthenticationService {
        public string Identity { get; set; }

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