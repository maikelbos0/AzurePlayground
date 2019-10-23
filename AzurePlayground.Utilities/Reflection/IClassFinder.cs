using System;
using System.Collections.Generic;

namespace AzurePlayground.Utilities.Reflection {
    public interface IClassFinder {
        IEnumerable<Type> FindAllClasses();
    }
}