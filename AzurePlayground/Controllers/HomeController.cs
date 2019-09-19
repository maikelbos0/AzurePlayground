using AzurePlayground.Commands.Security;
using AzurePlayground.Extensions;
using AzurePlayground.Models.Security;
using System.Web.Mvc;
using System.Web.Security;

namespace AzurePlayground.Controllers {
    [RoutePrefix("Home")]
    public class HomeController : Controller {
        private readonly IRegisterUserCommand _registerUserCommand;
        private readonly IActivateUserCommand _activateUserCommand;
        private readonly ISendUserActivationCommand _sendUserActivationCommand;
        private readonly ILogInUserCommand _logInUserCommand;
        private readonly ILogOutUserCommand _logOutUserCommand;

        public HomeController(IRegisterUserCommand registerUserCommand, 
            IActivateUserCommand activateUserCommand, 
            ISendUserActivationCommand sendUserActivationCommand, 
            ILogInUserCommand logInUserCommand,
            ILogOutUserCommand logOutUserCommand) {

            _registerUserCommand = registerUserCommand;
            _activateUserCommand = activateUserCommand;
            _sendUserActivationCommand = sendUserActivationCommand;
            _logInUserCommand = logInUserCommand;
            _logOutUserCommand = logOutUserCommand;
        }

        [Route("~/")]
        [Route]
        [Route("Index")]
        [HttpGet]
        public ActionResult Index() {
            return View();
        }

        [Route("Register")]
        [HttpGet]
        public ActionResult Register() {
            return View(new UserRegistration());
        }

        [Route("Register")]
        [HttpPost]
        public ActionResult Register(UserRegistration model) {
            if (ModelState.IsValid) {
                ModelState.Merge(_registerUserCommand.Execute(model));
            }
            
            if (ModelState.IsValid) {
                return View("Registered");
            }
            else {
                return View(model);
            }
        }

        [Route("Activate")]
        [HttpGet]
        public ActionResult Activate(string activationCode, string email) {
            var model = new UserActivation() {
                ActivationCode = activationCode,
                Email = email
            };

            if (!string.IsNullOrWhiteSpace(model.ActivationCode)
                && !string.IsNullOrWhiteSpace(model.Email)
                && _activateUserCommand.Execute(model).Success) {

                return View("Activated");
            }

            return View(model);
        }

        [Route("Activate")]
        [HttpPost]
        public ActionResult Activate(UserActivation model) {
            if (ModelState.IsValid) {
                ModelState.Merge(_activateUserCommand.Execute(model));
            }

            if (ModelState.IsValid) {
                return View("Activated");
            }
            else {
                return View(model);
            }
        }

        [Route("SendActivation")]
        [HttpGet]
        public ActionResult SendActivation() {
            return View(new UserSendActivation());
        }

        [Route("SendActivation")]
        [HttpPost]
        public ActionResult SendActivation(UserSendActivation model) {
            if (ModelState.IsValid) {
                ModelState.Merge(_sendUserActivationCommand.Execute(model));
            }

            if (ModelState.IsValid) {
                return View("ActivationSent");
            }
            else {
                return View(model);
            }
        }

        [Route("LogIn")]
        [HttpGet]
        public ActionResult LogIn() {
            return View(new UserLogIn());
        }

        [Route("LogIn")]
        [HttpPost]
        public ActionResult LogIn(UserLogIn model) {
            if (ModelState.IsValid) {
                ModelState.Merge(_logInUserCommand.Execute(model));
            }

            if (ModelState.IsValid) {
                FormsAuthentication.SetAuthCookie(model.Email, false);
                return RedirectToAction("LoggedIn");
            }
            else {
                return View(model);
            }
        }

        [Route("LoggedIn")]
        [HttpGet]
        [Authorize]
        public ActionResult LoggedIn() {
            return View();
        }

        [Route("LogOut")]
        [HttpPost]
        [Authorize]
        public ActionResult LogOut() {
            _logOutUserCommand.Execute(new UserLogOut() { Email = User.Identity.Name });
            FormsAuthentication.SignOut();
            return RedirectToAction("Index");
        }
    }
}