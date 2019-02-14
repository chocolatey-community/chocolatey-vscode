using System;
using System.Collections.Generic;
using System.Reflection;

namespace Chocolatey.Language.Server
{
    public interface ITypeLocator
    {
        IEnumerable<Type> GetTypesThatInheritOrImplement<T>();
        IEnumerable<Type> GetTypesThatInheritOrImplement<T>(params Assembly[] Assemblies);
    }
}
