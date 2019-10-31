using AzurePlayground.Utilities.Container;
using System.Web;
using System.Web.Security;

namespace AzurePlayground.Services {
    [InterfaceInjectable]
    public sealed class AuthenticationService : IAuthenticationService {
        public bool IsAuthenticated {
            get {
                return HttpContext.Current.User.Identity.IsAuthenticated;
            }
        }

        public string Identity {
            get {
                return HttpContext.Current.User.Identity.Name;
            }
        }

        public void SignIn(string identity) {
            FormsAuthentication.SetAuthCookie(identity, false);
        }

        public void SignOut() {
            FormsAuthentication.SignOut();
        }
    }
}