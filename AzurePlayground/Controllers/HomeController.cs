using System.Web.Mvc;

namespace AzurePlayground.Controllers {
    [RoutePrefix("Home")]
    public class HomeController : Controller {
        [Route("~/")]
        [Route]
        [Route("Index")]
        public ActionResult Index() {
            var cmd = new Commands.Security.UserCommands();

            cmd.Register(new Models.Security.UserRegistration() {
                Email = "test@test.com",
                Password = "test"
            });

            return View();
        }
    }
}