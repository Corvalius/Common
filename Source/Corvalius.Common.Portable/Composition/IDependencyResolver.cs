using System;
using System.Collections.Generic;

namespace Corvalius.Composition
{
    public interface IDependencyResolver
    {
        object GetService(Type serviceType);

        IEnumerable<object> GetServices(Type serviceType);
    }
}