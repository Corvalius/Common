using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Text;

namespace Corvalius.Composition
{
    /// <summary>
    /// Resolves types using the Managed Extensibility Framework.
    /// </summary>
    public class DesktopDependencyResolver : IDependencyResolver
    {
        private readonly CompositionContainer container;

        #region Constructor

        /// <summary>
        /// Initialises a new instance of <see cref="DesktopDependencyResolver"/>.
        /// </summary>
        /// <param name="container">The current container.</param>
        public DesktopDependencyResolver(CompositionContainer container)
        {
            if (container == null)
                throw new ArgumentNullException("container");

            this.container = container;
        }

        #endregion Constructor

        #region Methods

        /// <summary>
        /// Gets an instance of the service of the specified type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>An instance of the service of the specified type.</returns>
        public object GetService(Type type)
        {
            if (type == null)
                throw new ArgumentNullException("type");

            string name = AttributedModelServices.GetContractName(type);

            try
            {
                return container.GetExportedValue<object>(name);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Gets all instances of the services of the specified type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>An enumerable of all instances of the services of the specified type.</returns>
        public IEnumerable<object> GetServices(Type type)
        {
            if (type == null)
                throw new ArgumentNullException("type");

            string name = AttributedModelServices.GetContractName(type);

            try
            {
                return container.GetExportedValues<object>(name);
            }
            catch
            {
                return null;
            }
        }

        #endregion Methods
    }
}