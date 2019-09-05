using System;
using System.Linq;
using Unity;

namespace AzurePlayground.Utilities.Container {
    public class InjectionRegistrar {
        public void RegisterTypes(IUnityContainer container) {
            var mappedTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .Where(type => Attribute.IsDefined(type, typeof(InjectableAttribute)))
                .Select(type => new {
                    Type = type,
                    Interface = type.GetInterfaces().FirstOrDefault(i => i.Name == $"I{type.Name}")
                })
                .Where(type => type.Interface != null);

            foreach (var mappedType in mappedTypes) {
                container.RegisterType(mappedType.Interface, mappedType.Type);
            }
        }
    }
}
