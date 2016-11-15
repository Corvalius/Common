using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace Corvalius.Composition
{
    public class DependencyResolver
    {
        private static readonly DependencyResolver instance = new DependencyResolver();

        public static IDependencyResolver Current
        {
            get
            {
                return instance.InnerCurrent;
            }
        }

        public static void SetResolver(IDependencyResolver resolver)
        {
            instance.InnerSetResolver(resolver);
        }

        public static void SetResolver(object commonServiceLocator)
        {
            instance.InnerSetResolver(commonServiceLocator);
        }

        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is an appropriate nesting of generic types.")]
        public static void SetResolver(Func<Type, object> getService, Func<Type, IEnumerable<object>> getServices)
        {
            instance.InnerSetResolver(getService, getServices);
        }

        private IDependencyResolver current = new DefaultDependencyResolver();

        public IDependencyResolver InnerCurrent
        {
            get { return current; }
        }

        public void InnerSetResolver(IDependencyResolver resolver)
        {
            if (resolver == null)
                throw new ArgumentNullException("resolver");

            current = resolver;
        }

        public void InnerSetResolver(object commonServiceLocator)
        {
            if (commonServiceLocator == null)
                throw new ArgumentNullException("commonServiceLocator");

            Type locatorType = commonServiceLocator.GetType();
            MethodInfo getInstance = locatorType.GetMethod("GetInstance", new[] { typeof(Type) });
            MethodInfo getInstances = locatorType.GetMethod("GetAllInstances", new[] { typeof(Type) });

            if (getInstance == null || getInstance.ReturnType != typeof(object) ||
                getInstances == null || getInstances.ReturnType != typeof(IEnumerable<object>))
            {
                throw new ArgumentException(
                    String.Format(
                        CultureInfo.CurrentCulture,
                        "The object {0} does not implement ICommonServiceLocator",
                        locatorType.FullName
                        ),
                    "commonServiceLocator"
                    );
            }

            var getService = (Func<Type, object>)Delegate.CreateDelegate(typeof(Func<Type, object>), commonServiceLocator, getInstance);
            var getServices = (Func<Type, IEnumerable<object>>)Delegate.CreateDelegate(typeof(Func<Type, IEnumerable<object>>), commonServiceLocator, getInstances);

            current = new DelegateBasedDependencyResolver(getService, getServices);
        }

        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is an appropriate nesting of generic types.")]
        public void InnerSetResolver(Func<Type, object> getService, Func<Type, IEnumerable<object>> getServices)
        {
            if (getService == null)
                throw new ArgumentNullException("getService");

            if (getServices == null)
                throw new ArgumentNullException("getServices");

            current = new DelegateBasedDependencyResolver(getService, getServices);
        }

        // Helper classes

        private class DelegateBasedDependencyResolver : IDependencyResolver
        {
            private readonly Func<Type, object> getService;
            private readonly Func<Type, IEnumerable<object>> getServices;

            public DelegateBasedDependencyResolver(Func<Type, object> getService, Func<Type, IEnumerable<object>> getServices)
            {
                this.getService = getService;
                this.getServices = getServices;
            }

            [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "This method might throw exceptions whose type we cannot strongly link against; namely, ActivationException from common service locator")]
            public object GetService(Type type)
            {
                try
                {
                    return getService.Invoke(type);
                }
                catch
                {
                    return null;
                }
            }

            public IEnumerable<object> GetServices(Type type)
            {
                return getServices(type);
            }
        }

        private class DefaultDependencyResolver : IDependencyResolver
        {
            [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "This method might throw exceptions whose type we cannot strongly link against; namely, ActivationException from common service locator")]
            public object GetService(Type serviceType)
            {
                try
                {
                    return Activator.CreateInstance(serviceType);
                }
                catch
                {
                    return null;
                }
            }

            public IEnumerable<object> GetServices(Type serviceType)
            {
                return Enumerable.Empty<object>();
            }
        }
    }
}