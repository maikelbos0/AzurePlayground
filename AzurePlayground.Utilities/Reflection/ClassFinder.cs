using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace AzurePlayground.Utilities.Reflection {
    public sealed class ClassFinder : IClassFinder {
        public IEnumerable<Type> FindAllClasses() {
            // Ensure that all present solution assemblies are loaded
            foreach (var assemblyFile in Directory.GetFiles(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "*AzurePlayground*.dll")) {
                Assembly.LoadFile(assemblyFile);
            }

            return AppDomain.CurrentDomain.GetAssemblies()
                .Where(assembly => assembly.FullName.Contains("AzurePlayground"))
                .SelectMany(assembly => assembly.GetTypes());
        }
    }
}