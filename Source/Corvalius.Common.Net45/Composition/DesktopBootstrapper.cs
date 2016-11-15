using Corvalius.Collections;
using System;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Threading;

namespace Corvalius.Composition
{
    /// <summary>
    /// Performs bootstrapping operations.
    /// </summary>
    public static class DesktopBootstrapper
    {
        private static bool initialised;
        private static readonly Mutex mutex = new Mutex();

        /// <summary>
        /// Gets the current container.
        /// </summary>
        public static CompositionContainer Container { get; private set; }

        #region Methods

        /// <summary>
        /// Runs the bootstrapper.
        /// </summary>
        public static void Run()
        {
            if (Container == null)
                return;

            // Set the IDependencyResolver MVC uses to resolve types.
            DependencyResolver.SetResolver(new DesktopDependencyResolver(Container));

            // Run any bootstrapper tasks.
            RunTasks();
        }

        /// <summary>
        /// Runs any required bootstrapper tasks.
        /// </summary>
        private static void RunTasks()
        {
            var tasks = Container.GetExports<IBootstrapperTask, INamedDependencyMetadata>();
            var list = new DependencyList<Lazy<IBootstrapperTask, INamedDependencyMetadata>, string>(
                l => l.Metadata.Name,
                l => l.Metadata.Dependencies);

            foreach (var task in tasks)
                list.Add(task);

            foreach (var task in list)
                task.Value.Run(Container);
        }

        /// <summary>
        /// Sets the factory used to create a container.
        /// </summary>
        /// <param name="factory">The container factory.</param>
        public static void SetContainerFactory(ICompositionContainerFactory factory)
        {
            if (factory == null)
                throw new ArgumentNullException("factory");

            mutex.WaitOne();

            if (initialised)
                return;

            try
            {
                var container = factory.CreateContainer();

                if (container == null)
                    throw new InvalidOperationException("The container factory failed to create the container.");

                Container = container;

                // Add the container to itself so it can be resolved.
                var batch = new CompositionBatch();
                batch.AddExportedValue(container);
                container.Compose(batch);

                initialised = true;
            }
            finally
            {
                mutex.ReleaseMutex();
            }
        }

        #endregion Methods
    }
}