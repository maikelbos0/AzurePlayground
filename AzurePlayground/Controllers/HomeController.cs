using AzurePlayground.Commands.Security;
using AzurePlayground.Extensions;
using AzurePlayground.Models.Security;
using System.Web.Mvc;

namespace AzurePlayground.Controllers {
    [RoutePrefix("Home")]
    public class HomeController : Controller {
        private readonly IRegisterUserCommand _registerUserCommand;

        public HomeController (IRegisterUserCommand registerUserCommand) {
            _registerUserCommand = registerUserCommand;
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
    }
}