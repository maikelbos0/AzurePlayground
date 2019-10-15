using AzurePlayground.CommandHandlers.Security;
using AzurePlayground.Extensions;
using AzurePlayground.Models.Security;
using AzurePlayground.Commands.Security;
using AzurePlayground.Providers;
using System.Web.Mvc;

namespace AzurePlayground.Controllers {
    [RoutePrefix("Home")]
    public class HomeController : Controller {
        private readonly IAuthenticationProvider _authenticationProvider;
        private readonly IRegisterUserCommandHandler _registerUserCommand;
        private readonly IActivateUserCommandHandler _activateUserCommand;
        private readonly ISendUserActivationCommandHandler _sendUserActivationCommand;
        private readonly ILogInUserCommandHandler _logInUserCommand;
        private readonly ILogOutUserCommandHandler _logOutUserCommand;
        private readonly IChangeUserPasswordCommandHandler _changeUserPasswordCommand;
        private readonly IForgotUserPasswordCommandHandler _requestUserPasswordResetCommand;
        private readonly IResetUserPasswordCommandHandler _resetUserPasswordCommand;
        private readonly IDeactivateUserCommandHandler _deactivateUserCommand;

        public HomeController(IAuthenticationProvider authenticationProvider,
            IRegisterUserCommandHandler registerUserCommand,
            IActivateUserCommandHandler activateUserCommand,
            ISendUserActivationCommandHandler sendUserActivationCommand,
            ILogInUserCommandHandler logInUserCommand,
            ILogOutUserCommandHandler logOutUserCommand,
            IChangeUserPasswordCommandHandler changeUserPasswordCommand,
            IForgotUserPasswordCommandHandler requestUserPasswordResetCommand,
            IResetUserPasswordCommandHandler resetUserPasswordCommand,
            IDeactivateUserCommandHandler deactivateUserCommand) {

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
            return View(new RegisterUserModel());
        }

        [Route("Register")]
        [HttpPost]
        public ActionResult Register(RegisterUserModel model) {
            if (ModelState.IsValid) {
                ModelState.Merge(_registerUserCommand.Execute(new RegisterUserCommand(model.Email, model.Password, model.ConfirmPassword)));
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
            var model = new ActivateUserModel() {
                ActivationCode = activationCode,
                Email = email
            };

            if (!string.IsNullOrWhiteSpace(model.ActivationCode)
                && !string.IsNullOrWhiteSpace(model.Email)
                && _activateUserCommand.Execute(new ActivateUserCommand(model.Email, model.ActivationCode)).Success) {

                return View("Activated");
            }

            return View(model);
        }

        [Route("Activate")]
        [HttpPost]
        public ActionResult Activate(ActivateUserModel model) {
            if (ModelState.IsValid) {
                ModelState.Merge(_activateUserCommand.Execute(new ActivateUserCommand(model.Email, model.ActivationCode)));
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
            return View(new SendUserActivationModel());
        }

        [Route("SendActivation")]
        [HttpPost]
        public ActionResult SendActivation(SendUserActivationModel model) {
            if (ModelState.IsValid) {
                ModelState.Merge(_sendUserActivationCommand.Execute(new SendUserActivationCommand(model.Email)));
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
            return View(new LogInUserModel());
        }

        [Route("LogIn")]
        [HttpPost]
        public ActionResult LogIn(LogInUserModel model) {
            if (ModelState.IsValid) {
                ModelState.Merge(_logInUserCommand.Execute(new LogInUserCommand(model.Email, model.Password)));
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
            _logOutUserCommand.Execute(new LogOutUserCommand(_authenticationProvider.GetIdentity()));
            _authenticationProvider.SignOut();
            return RedirectToAction("Index");
        }

        [Route("ChangePassword")]
        [HttpGet]
        [Authorize]
        public ActionResult ChangePassword() {
            return View(new ChangeUserPasswordModel());
        }

        [Route("ChangePassword")]
        [HttpPost]
        [Authorize]
        public ActionResult ChangePassword(ChangeUserPasswordModel model) {
            if (ModelState.IsValid) {
                ModelState.Merge(_changeUserPasswordCommand.Execute(new ChangeUserPasswordCommand(_authenticationProvider.GetIdentity(), model.CurrentPassword, model.NewPassword, model.ConfirmNewPassword)));
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
            return View(new ForgotUserPasswordModel());
        }

        [Route("ForgotPassword")]
        [HttpPost]
        public ActionResult ForgotPassword(ForgotUserPasswordModel model) {
            if (ModelState.IsValid) {
                ModelState.Merge(_requestUserPasswordResetCommand.Execute(new ForgotUserPasswordCommand(model.Email)));
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
            var model = new ResetUserPasswordModel() {
                PasswordResetToken = token,
                Email = email
            };

            return View(model);
        }

        [Route("ResetPassword")]
        [HttpPost]
        public ActionResult ResetPassword(ResetUserPasswordModel model) {
            if (ModelState.IsValid) {
                ModelState.Merge(_resetUserPasswordCommand.Execute(new ResetUserPasswordCommand( model.Email, model.PasswordResetToken, model.NewPassword, model.ConfirmNewPassword)));
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
            return View(new DeactivateUserModel());
        }

        [Route("Deactivate")]
        [HttpPost]
        [Authorize]
        public ActionResult Deactivate(DeactivateUserModel model) {
            if (ModelState.IsValid) {
                ModelState.Merge(_deactivateUserCommand.Execute( new DeactivateUserCommand(_authenticationProvider.GetIdentity(), model.Password)));
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