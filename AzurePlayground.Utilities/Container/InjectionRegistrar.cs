using System;
using System.Linq;
using System.IO;
using System.Reflection;
using Unity;

namespace AzurePlayground.Utilities.Container {
    public class InjectionRegistrar {
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
                container.RegisterType(mappedType.Interface, mappedType.Type);
            }
        }
    }
}