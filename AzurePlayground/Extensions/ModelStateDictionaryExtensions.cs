using AzurePlayground.CommandHandlers;
using AzurePlayground.Commands;
using System.Web.Mvc;

namespace AzurePlayground.Extensions {
    public static class ModelStateDictionaryExtensions {
        public static void Merge<TModel, TCommand>(this ModelStateDictionary modelState, CommandResult<TCommand> commandResult) where TCommand : ICommand {
            foreach (var commandError in commandResult.Errors) {
                string propertyName = null;

                if (commandError.Expression != null) {
                    propertyName = ExpressionHelper.GetExpressionText(commandError.Expression);
                }
                
                if (propertyName == null || typeof(TModel).GetProperty(propertyName) == null) {
                    modelState.AddModelError(string.Empty, commandError.Message);
                }
                else {
                    modelState.AddModelError(propertyName, commandError.Message);
                }
            }
        }
    }
}