using System.Web.Mvc;

namespace AzurePlayground.Controllers {
    [RoutePrefix("Home")]
    public class HomeController : Controller {
        [Route("~/")]
        [Route]
        [Route("Index")]
        public ActionResult Index() {
            return View();
        }
    }
}