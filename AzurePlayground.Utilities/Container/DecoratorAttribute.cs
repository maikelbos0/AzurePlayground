using System;

namespace AzurePlayground.Utilities.Container {
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public abstract class DecoratorAttribute : Attribute {
        public Type DecoratorType { get; private set; }

        public DecoratorAttribute(Type decoratorType) {
            if (decoratorType.BaseType.GetGenericTypeDefinition() != typeof(Decorator<>)) {
                throw new InvalidOperationException("Decorator type must inherit from Decorator base class");
            }

            DecoratorType = decoratorType;
        }
    }
}