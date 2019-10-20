using System;
using System.Linq.Expressions;

namespace AzurePlayground.CommandHandlers {
    public sealed class CommandError<TParameter> {
        public Expression<Func<TParameter, object>> Expression { get; set; }
        public string Message { get; set; }
    }
}
