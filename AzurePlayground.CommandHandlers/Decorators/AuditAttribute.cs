using AzurePlayground.Utilities.Container;

namespace AzurePlayground.CommandHandlers.Decorators {
    public sealed class AuditAttribute : DecoratorAttribute {
        public AuditAttribute() : base(typeof(AuditDecorator<>)) {
        }
    }
}