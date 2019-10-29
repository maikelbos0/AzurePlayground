using AzurePlayground.Models;
using AzurePlayground.Models.Security;
using AzurePlayground.Queries.Security;
using AzurePlayground.Services;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace AzurePlayground.Controllers {
    [Authorize]
    [RoutePrefix("User")]
    public sealed class UserController : BaseController {
        private readonly IDataGridViewService _dataGridViewService;

        public UserController(IAuthenticationService authenticationService, IMessageService messageService, IDataGridViewService dataGridViewService) : base(messageService, authenticationService) {
            _dataGridViewService = dataGridViewService;
        }

        [Route]
        [Route("Index")]
        [HttpGet]
        public ActionResult Index() {
            return View();
        }
        
        [Route("GetUsers")]
        [HttpPost]
        public ActionResult GetUsers(DataGridViewMetaData metaData) {
            IEnumerable<UserViewModel> data = _messageService.Dispatch(new GetUsersQuery());

            data = _dataGridViewService.ApplyMetaData(data, ref metaData);

            return Json(new {
                metaData,
                data
            });
        }
    }
}