using AzurePlayground.Commands;
using System.Web.Mvc;

namespace AzurePlayground.Extensions {
    public static class ModelStateDictionaryExtensions {
        public static void Merge<TCommandParameter>(this ModelStateDictionary modelState, CommandResult<TCommandParameter> commandResult) {
            foreach (var commandError in commandResult.Errors) {
                var propertyName = ExpressionHelper.GetExpressionText(commandError.Expression);
                modelState.AddModelError(propertyName, commandError.Message);
            }
        }
    }
}