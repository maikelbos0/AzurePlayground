using AzurePlayground.Extensions;
using AzurePlayground.Models.Security;
using AzurePlayground.Commands.Security;
using AzurePlayground.Services;
using System.Web.Mvc;

namespace AzurePlayground.Controllers {
    [RoutePrefix("Home")]
    public class HomeController : Controller {
        private readonly IAuthenticationService _authenticationService;
        private readonly IMessageService _messageService;

        public HomeController(IAuthenticationService authenticationService, IMessageService messageService) {
            _authenticationService = authenticationService;
            _messageService = messageService;
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
                ModelState.Merge<RegisterUserModel, RegisterUserCommand>(_messageService.Dispatch(new RegisterUserCommand(model.Email, model.Password)));
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
                && _messageService.Dispatch(new ActivateUserCommand(model.Email, model.ActivationCode)).Success) {

                return View("Activated");
            }

            return View(model);
        }

        [Route("Activate")]
        [HttpPost]
        public ActionResult Activate(ActivateUserModel model) {
            if (ModelState.IsValid) {
                ModelState.Merge<ActivateUserModel, ActivateUserCommand>(_messageService.Dispatch(new ActivateUserCommand(model.Email, model.ActivationCode)));
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
                ModelState.Merge<SendUserActivationModel, SendUserActivationCommand>(_messageService.Dispatch(new SendUserActivationCommand(model.Email)));
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
                ModelState.Merge<LogInUserModel, LogInUserCommand>(_messageService.Dispatch(new LogInUserCommand(model.Email, model.Password)));
            }

            if (ModelState.IsValid) {
                _authenticationService.SignIn(model.Email);
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
            _messageService.Dispatch(new LogOutUserCommand(_authenticationService.GetIdentity()));
            _authenticationService.SignOut();
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
                ModelState.Merge<ChangeUserPasswordModel, ChangeUserPasswordCommand>(_messageService.Dispatch(new ChangeUserPasswordCommand(
                    _authenticationService.GetIdentity(), model.CurrentPassword, model.NewPassword)));
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
                ModelState.Merge<ForgotUserPasswordModel, ForgotUserPasswordCommand>(_messageService.Dispatch(new ForgotUserPasswordCommand(model.Email)));
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
        public ActionResult ResetPassword() {
            return View(new ResetUserPasswordModel());
        }

        [Route("ResetPassword")]
        [HttpPost]
        public ActionResult ResetPassword(string email, string token, ResetUserPasswordModel model) {
            if (ModelState.IsValid) {
                ModelState.Merge<ResetUserPasswordModel, ResetUserPasswordCommand>(_messageService.Dispatch(new ResetUserPasswordCommand(
                    email, token, model.NewPassword)));
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
                ModelState.Merge<DeactivateUserModel, DeactivateUserCommand>(_messageService.Dispatch(new DeactivateUserCommand(
                    _authenticationService.GetIdentity(), model.Password)));
            }

            if (ModelState.IsValid) {
                _authenticationService.SignOut();
                return RedirectToAction("Index");
            }
            else {
                return View(model);
            }
        }
    }
}