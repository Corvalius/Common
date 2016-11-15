namespace Corvalius.Composition
{
    /// <summary>
    /// Defines the required contract for implementing ordered metadata.
    /// </summary>
    public interface IOrderedMetadata
    {
        /// <summary>
        /// Gets the order.
        /// </summary>
        int Order { get; }
    }
}