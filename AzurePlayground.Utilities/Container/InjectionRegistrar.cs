using System;
using System.Linq;
using System.IO;
using System.Reflection;
using Unity;

namespace AzurePlayground.Utilities.Container {
    public sealed class InjectionRegistrar {
        public void RegisterTypes(IUnityContainer container) {
            // Ensure that all present solution assemblies are loaded
            foreach (var assemblyFile in Directory.GetFiles(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "*AzurePlayground*.dll")) {
                Assembly.LoadFile(assemblyFile);
            }

            var mappedTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .Where(type => Attribute.IsDefined(type, typeof(InjectableAttribute)))
                .Select(type => new {
                    Type = type,
                    Interface = type.GetInterfaces().SingleOrDefault(i => i.Name == $"I{type.Name}") ?? type.GetInterfaces().SingleOrDefault()
                })
                .Where(type => type.Interface != null);

            foreach (var mappedType in mappedTypes) {
                var decoratorTypes = mappedType.Type.GetCustomAttributes()
                    .Where(a => a.GetType().IsSubclassOf(typeof(DecoratorAttribute)))
                    .Select(a => ((DecoratorAttribute)a).DecoratorType.MakeGenericType(mappedType.Interface.GetGenericArguments()));

                if (!decoratorTypes.Any()) {
                    container.RegisterType(mappedType.Interface, mappedType.Type);
                }
                else {
                    if (decoratorTypes.Any(t => !t.GetInterfaces().Contains(mappedType.Interface))) {
                        throw new InvalidOperationException($"Type registration for {mappedType.Interface.FullName} failed; invalid decorator found.");
                    }

                    // If we have decorators, we chain the calls
                    Func<IUnityContainer, object> action = c => {
                        dynamic obj = c.Resolve(mappedType.Type);

                        foreach (var decoratorType in decoratorTypes) {
                            dynamic decoratorObj = c.Resolve(decoratorType);

                            decoratorObj.Handler = obj;
                            obj = decoratorObj;
                        }

                        return obj;
                    };

                    container.RegisterFactory(mappedType.Interface, action);
                }
            }
        }
    }
}