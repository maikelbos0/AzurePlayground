using AzurePlayground.Utilities.Container;

namespace AzurePlayground.QueryHandlers.Decorators {
    public sealed class AuditAttribute : DecoratorAttribute {
        public AuditAttribute() : base(typeof(AuditDecorator<,>)) {
        }
    }
}