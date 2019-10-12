using AzurePlayground.Commands.Security;
using AzurePlayground.Extensions;
using AzurePlayground.Models.Security;
using AzurePlayground.Providers;
using System.Web.Mvc;

namespace AzurePlayground.Controllers {
    [RoutePrefix("Home")]
    public class HomeController : Controller {
        private readonly IAuthenticationProvider _authenticationProvider;
        private readonly IRegisterUserCommand _registerUserCommand;
        private readonly IActivateUserCommand _activateUserCommand;
        private readonly ISendUserActivationCommand _sendUserActivationCommand;
        private readonly ILogInUserCommand _logInUserCommand;
        private readonly ILogOutUserCommand _logOutUserCommand;
        private readonly IChangeUserPasswordCommand _changeUserPasswordCommand;
        private readonly IForgotUserPasswordCommand _requestUserPasswordResetCommand;
        private readonly IResetUserPasswordCommand _resetUserPasswordCommand;
        private readonly IDeactivateUserCommand _deactivateUserCommand;

        public HomeController(IAuthenticationProvider authenticationProvider,
            IRegisterUserCommand registerUserCommand, 
            IActivateUserCommand activateUserCommand, 
            ISendUserActivationCommand sendUserActivationCommand, 
            ILogInUserCommand logInUserCommand,
            ILogOutUserCommand logOutUserCommand,
            IChangeUserPasswordCommand changeUserPasswordCommand,
            IForgotUserPasswordCommand requestUserPasswordResetCommand,
            IResetUserPasswordCommand resetUserPasswordCommand,
            IDeactivateUserCommand deactivateUserCommand) {

            _authenticationProvider = authenticationProvider;
            _registerUserCommand = registerUserCommand;
            _activateUserCommand = activateUserCommand;
            _sendUserActivationCommand = sendUserActivationCommand;
            _logInUserCommand = logInUserCommand;
            _logOutUserCommand = logOutUserCommand;
            _changeUserPasswordCommand = changeUserPasswordCommand;
            _requestUserPasswordResetCommand = requestUserPasswordResetCommand;
            _resetUserPasswordCommand = resetUserPasswordCommand;
            _deactivateUserCommand = deactivateUserCommand;
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
                _authenticationProvider.SignIn(model.Email);
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
            _logOutUserCommand.Execute(new UserLogOut() { Email = _authenticationProvider.GetIdentity() });
            _authenticationProvider.SignOut();
            return RedirectToAction("Index");
        }

        [Route("ChangePassword")]
        [HttpGet]
        [Authorize]
        public ActionResult ChangePassword() {
            return View(new UserChangePassword());
        }

        [Route("ChangePassword")]
        [HttpPost]
        [Authorize]
        public ActionResult ChangePassword(UserChangePassword model) {
            if (ModelState.IsValid) {
                model.Email = _authenticationProvider.GetIdentity();

                ModelState.Merge(_changeUserPasswordCommand.Execute(model));
            }

            if (ModelState.IsValid) {
                return View("PasswordChanged");
            }
            else {
                return View(model);
            }
        }

        [Route("ForgotPassword")]
        [HttpGet]
        public ActionResult ForgotPassword() {
            return View(new UserForgotPassword());
        }

        [Route("ForgotPassword")]
        [HttpPost]
        public ActionResult ForgotPassword(UserForgotPassword model) {
            if (ModelState.IsValid) {
                ModelState.Merge(_requestUserPasswordResetCommand.Execute(model));
            }

            if (ModelState.IsValid) {
                return View("PasswordResetSent");
            }
            else {
                return View(model);
            }
        }

        [Route("ResetPassword")]
        [HttpGet]
        public ActionResult ResetPassword(string token, string email) {
            var model = new UserPasswordReset() {
                PasswordResetToken = token,
                Email = email
            };

            return View(model);
        }

        [Route("ResetPassword")]
        [HttpPost]
        public ActionResult ResetPassword(UserPasswordReset model) {
            if (ModelState.IsValid) {
                ModelState.Merge(_resetUserPasswordCommand.Execute(model));
            }

            if (ModelState.IsValid) {
                return View("PasswordReset");
            }
            else {
                return View(model);
            }
        }

        [Route("Deactivate")]
        [HttpGet]
        [Authorize]
        public ActionResult Deactivate() {
            return View(new UserDeactivation());
        }

        [Route("Deactivate")]
        [HttpPost]
        [Authorize]
        public ActionResult Deactivate(UserDeactivation model) {
            if (ModelState.IsValid) {
                model.Email = _authenticationProvider.GetIdentity();

                ModelState.Merge(_deactivateUserCommand.Execute(model));
            }

            if (ModelState.IsValid) {
                _authenticationProvider.SignOut();
                return RedirectToAction("Index");
            }
            else {
                return View(model);
            }
        }
    }
}