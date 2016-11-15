using System.ComponentModel.Composition.Hosting;

namespace Corvalius.Composition
{
    /// <summary>
    /// Defines the required contract for implementing a container factory.
    /// </summary>
    public interface ICompositionContainerFactory
    {
        /// <summary>
        /// Creates a <see cref="CompositionContainer"/>.
        /// </summary>
        /// <returns>A <see cref="CompositionContainer"/>.</returns>
        CompositionContainer CreateContainer();
    }
}