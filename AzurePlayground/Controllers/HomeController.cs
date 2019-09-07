using AzurePlayground.Commands.Security;
using AzurePlayground.Extensions;
using AzurePlayground.Models.Security;
using System.Web.Mvc;

namespace AzurePlayground.Controllers {
    [RoutePrefix("Home")]
    public class HomeController : Controller {
        private readonly IRegisterUserCommand _registerUserCommand;
        private readonly IActivateUserCommand _activateUserCommand;

        public HomeController(IRegisterUserCommand registerUserCommand, IActivateUserCommand activateUserCommand) {
            _registerUserCommand = registerUserCommand;
            _activateUserCommand = activateUserCommand;
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
    }
}