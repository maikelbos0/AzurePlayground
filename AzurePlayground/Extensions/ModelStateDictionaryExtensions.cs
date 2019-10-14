﻿using AzurePlayground.CommandHandlers;
using System;
using System.Web.Mvc;

namespace AzurePlayground.Extensions {
    public static class ModelStateDictionaryExtensions {
        public static void Merge<TCommandParameter>(this ModelStateDictionary modelState, CommandResult<TCommandParameter> commandResult) {
            foreach (var commandError in commandResult.Errors) {
                var propertyName = ExpressionHelper.GetExpressionText(commandError.Expression);
                modelState.AddModelError(propertyName, commandError.Message);
            }
        }

        [Obsolete("Implementation to follow", true)]
        public static void Merge<TModel, TCommandParameter>(this ModelStateDictionary modelState, CommandResult<TCommandParameter> commandResult) {
            foreach (var commandError in commandResult.Errors) {
                var propertyName = ExpressionHelper.GetExpressionText(commandError.Expression);

                if (typeof(TModel).GetProperty(propertyName) == null) {
                    throw new InvalidOperationException($"Model type '{typeof(TModel)}' does not contain property '{propertyName}'");
                }

                modelState.AddModelError(propertyName, commandError.Message);
            }
        }
    }
}