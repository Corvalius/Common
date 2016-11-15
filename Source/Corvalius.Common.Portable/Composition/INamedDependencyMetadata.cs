namespace Corvalius.Composition
{
    /// <summary>
    /// Defines the required contract for implementing named dependency metadata.
    /// </summary>
    public interface INamedDependencyMetadata : INamedMetadata
    {
        /// <summary>
        /// Gets the dependencies.
        /// </summary>
        string[] Dependencies { get; }
    }
}