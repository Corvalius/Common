using System.Collections.Generic;

namespace Corvalius.Collections
{
    /// <summary>
    /// Represents a node within a dependency list.
    /// </summary>
    /// <typeparam name="TModel">The model in the list.</typeparam>
    /// <typeparam name="TKey">The identifier type.</typeparam>
    internal class DependencyListNode<TModel, TKey>
    {
        #region Constructor

        /// <summary>
        /// Initialises a new instance of <see cref="DependencyListNode{TModel,TKey}"/>
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="identifier">The identifier of the item.</param>
        public DependencyListNode(TModel item, TKey identifier)
        {
            Item = item;
            Identifier = identifier;

            Dependencies = new List<DependencyListNode<TModel, TKey>>();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the list of dependancies.
        /// </summary>
        public List<DependencyListNode<TModel, TKey>> Dependencies { get; private set; }

        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        public TKey Identifier { get; set; }

        /// <summary>
        /// Gets or sets the model instance.
        /// </summary>
        public TModel Item { get; set; }

        /// <summary>
        /// Gets the root item.
        /// </summary>
        public DependencyListNode<TModel, TKey> Root { get; set; }

        /// <summary>
        /// Gets or sets whether this item has been visited.
        /// </summary>
        public bool Visited { get; set; }

        #endregion
    }
}