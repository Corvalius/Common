using System.ComponentModel.Composition.Hosting;

namespace Corvalius.Composition
{
    /// <summary> 
    /// Defines the required contract for implementing a bootstrapper task. 
    /// </summary> 
    public interface IBootstrapperTask
    {
        /// <summary> 
        /// Runs the task. 
        /// </summary> 
        /// <param name="container"></param> 
        void Run(CompositionContainer container);
    }
}