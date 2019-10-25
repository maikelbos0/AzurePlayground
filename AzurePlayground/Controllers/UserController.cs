using AzurePlayground.Queries.Security;
using AzurePlayground.Services;
using System.Web.Mvc;

namespace AzurePlayground.Controllers {
    [Authorize]
    [RoutePrefix("User")]
    public sealed class UserController : BaseController {
        public UserController(IAuthenticationService authenticationService, IMessageService messageService) : base(messageService, authenticationService) {
        }

        [Route]
        [Route("Index")]
        [HttpGet]

        public ActionResult Index() {
            return View(_messageService.Dispatch(new GetUsersQuery()));
        }
    }
}