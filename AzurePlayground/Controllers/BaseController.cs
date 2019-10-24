using AzurePlayground.Commands;
using AzurePlayground.Extensions;
using AzurePlayground.Services;
using System;
using System.Web.Mvc;

namespace AzurePlayground.Controllers {
    public abstract class BaseController : Controller {
        protected readonly IMessageService _messageService;

        public BaseController(IMessageService messageService) {
            _messageService = messageService;
        }

        protected ActionResult ValidatedCommandResult<TCommand>(object model, TCommand command, Func<ActionResult> onValid) where TCommand : ICommand {
            return ValidatedCommandResult(model, command, onValid, () => View(model));
        }

        protected ActionResult ValidatedCommandResult<TCommand>(object model, TCommand command, Func<ActionResult> onValid, Func<ActionResult> onInvalid) where TCommand : ICommand {
            if (ModelState.IsValid) {
                ModelState.Merge(model, _messageService.Dispatch(command));
            }

            if (ModelState.IsValid) {
                return onValid();
            }
            else {
                return onInvalid();
            }

        }
    }
}