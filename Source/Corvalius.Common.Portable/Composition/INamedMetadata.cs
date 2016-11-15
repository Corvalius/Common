namespace Corvalius.Composition
{
    /// <summary>
    /// Defines the required contract for implementing named metadata.
    /// </summary>
    public interface INamedMetadata
    {
        /// <summary>
        /// Gets the name.
        /// </summary>
        string Name { get; }
    }
}