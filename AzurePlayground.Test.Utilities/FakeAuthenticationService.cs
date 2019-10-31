using AzurePlayground.Services;

namespace AzurePlayground.Test.Utilities {
    public sealed class FakeAuthenticationService : IAuthenticationService {
        public bool IsAuthenticated {
            get {
                return Identity != null;
            }
        }
        public string Identity { get; set; }

        public void SignIn(string identity) {
            Identity = identity;
        }

        public void SignOut() {
            Identity = null;
        }
    }
}