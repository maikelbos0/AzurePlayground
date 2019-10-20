using AzurePlayground.Utilities.Container;
using System.Web;
using System.Web.Security;

namespace AzurePlayground.Services {
    [Injectable]
    public sealed class AuthenticationService : IAuthenticationService {
        public string GetIdentity() {
            return HttpContext.Current.User.Identity.Name;
        }

        public void SignIn(string identity) {
            FormsAuthentication.SetAuthCookie(identity, false);
        }

        public void SignOut() {
            FormsAuthentication.SignOut();
        }
    }
}