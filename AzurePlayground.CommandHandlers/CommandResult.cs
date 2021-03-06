﻿using AzurePlayground.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace AzurePlayground.CommandHandlers {
    public sealed class CommandResult<TCommand> where TCommand : ICommand {
        public List<CommandError<TCommand>> Errors { get; set; } = new List<CommandError<TCommand>>();

        public bool Success {
            get {
                return !Errors.Any();
            }
        }

        public void AddError(string message) {
            AddError(null, message);
        }

        public void AddError(Expression<Func<TCommand, object>> expression, string message) {
            Errors.Add(new CommandError<TCommand>() {
                Expression = expression,
                Message = message
            });
        }
    }
}