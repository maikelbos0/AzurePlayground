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
        public UserController(IAuthenticationService authenticationService, IMessageService messageService) : base(messageService, authenticationService) {
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

            metaData.totalRows = data.Count();

            if (metaData.rowsPerPage == 0) {
                metaData.rowsPerPage = 25;
            }

            // Sorting
            if (metaData.sortColumn != null) {
                var property = typeof(UserViewModel).GetProperty(metaData.sortColumn);

                if (property != null) {
                    if (metaData.sortDescending) {
                        data = data.OrderByDescending(i => property.GetValue(i));
                    }
                    else {
                        data = data.OrderBy(i => property.GetValue(i));
                    }
                }
            }

            // Paging
            data = data.Skip(metaData.page * metaData.rowsPerPage).Take(metaData.rowsPerPage);

            return Json(new {
                metaData,
                data
            });
        }
    }
}