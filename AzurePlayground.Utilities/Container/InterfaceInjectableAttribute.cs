﻿using System;

namespace AzurePlayground.Utilities.Container {
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class InterfaceInjectableAttribute : Attribute { }
}