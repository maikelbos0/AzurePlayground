using System;

namespace AzurePlayground.CommandHandlers.Security {
    public abstract class BaseUserCommandHandler {

        protected int GetNewActivationCode() {
            return new Random().Next(10000, int.MaxValue);
        }

    }
}