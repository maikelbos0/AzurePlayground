using AzurePlayground.Utilities.Container;
using System.Web;
using System.Web.Security;

namespace AzurePlayground.Providers {
    [Injectable]
    public class AuthenticationProvider : IAuthenticationProvider {
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