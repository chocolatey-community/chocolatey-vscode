using System;
using System.Collections.Generic;
using System.Reflection;
using Chocolatey.Language.Server.Extensions;

namespace Chocolatey.Language.Server.Registration
{
    public class TypeLocator : ITypeLocator
    {
        public IEnumerable<Type> GetTypesThatInheritOrImplement<T>()
        {
            return GetTypesThatInheritOrImplement<T>(GetType().Assembly);
        }

        public IEnumerable<Type> GetTypesThatInheritOrImplement<T>(params Assembly[] assemblies)
        {
            var list = new List<Type>();

            if (assemblies == null || assemblies.Length == 0)
            {
               throw new ApplicationException("TypeLocator cannot locate types without assemblies");
            }

            foreach (var assembly in assemblies.OrEmptyListIfNull())
            {
                foreach (var t in assembly.GetTypes())
                {
                    if (!typeof(T).IsAssignableFrom(t) || t.IsInterface || t.IsAbstract) continue;

                    list.Add(t);
                }
            }

            return list;
        }
    }
}
