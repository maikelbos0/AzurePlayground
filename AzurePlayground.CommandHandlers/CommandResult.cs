using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace AzurePlayground.CommandHandlers {
    public class CommandResult<TParameter> {
        public List<CommandError<TParameter>> Errors { get; set; } = new List<CommandError<TParameter>>();
        public bool Success {
            get {
                return !Errors.Any();
            }
        }

        public void AddError(Expression<Func<TParameter, object>> expression, string message) {
            Errors.Add(new CommandError<TParameter>() {
                Expression = expression,
                Message = message
            });
        }
    }
}
