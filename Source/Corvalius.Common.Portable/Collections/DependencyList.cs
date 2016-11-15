using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;

namespace Corvalius.Collections
{
    /// <summary>
    /// Represents a list whose items can be dependent on each other, and enumerating the list will sort the items
    /// as per their dependency requirements.
    /// </summary>
    /// <typeparam name="TModel">The model in the list.</typeparam>
    /// <typeparam name="TKey">The identifier type.</typeparam>
    public class DependencyList<TModel, TKey> : IList<TModel>
    {
        #region Fields

        private readonly Func<TModel, IEnumerable<TKey>> DependancyFunc;
        private readonly Func<TModel, TKey> IdentifierFunc;
        private readonly List<DependencyListNode<TModel, TKey>> Nodes;

        private bool modified;
        private List<TModel> lastSort;

        #endregion

        #region Constructor

        /// <summary>
        /// Initialises a new instance of <see cref="DependencyList{TModel,TKey}"/>.
        /// </summary>
        /// <param name="identifierFunc">A delegate used to get the identifier for an item.</param>
        /// <param name="dependancyFunc">A delegate used to get dependancies for an item.</param>
        public DependencyList(Func<TModel, TKey> identifierFunc, Func<TModel, IEnumerable<TKey>> dependancyFunc)
        {
            if (identifierFunc == null)
                throw new ArgumentNullException("identifierFunc");

            if (dependancyFunc == null)
                throw new ArgumentNullException("dependancyFunc");

            IdentifierFunc = identifierFunc;
            DependancyFunc = dependancyFunc;

            Nodes = new List<DependencyListNode<TModel, TKey>>();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets a count of the number of items in the list.
        /// </summary>
        public int Count { get { return Nodes.Count; } }

        /// <summary>
        /// Gets whether the list is read-only.
        /// </summary>
        public bool IsReadOnly { get { return false; } }

        #endregion

        #region Methods

        /// <summary>
        /// Adds a new item to the list.
        /// </summary>
        /// <param name="item">The item to add to the list.</param>
        [DebuggerStepThrough]
        public void Add(TModel item)
        {
            var identifier = IdentifierFunc(item);
            var node = new DependencyListNode<TModel, TKey>(item, identifier);

            if (item is INotifyPropertyChanged)
                ((INotifyPropertyChanged)item).PropertyChanged += (s, e) => { modified = true; };

            Nodes.Add(node);
            modified = true;
        }

        /// <summary>
        /// Adds any dependancies required by the specified node.
        /// </summary>
        /// <param name="node">The node to add dependancies to.</param>
        [DebuggerStepThrough]
        private void AddDependancies(DependencyListNode<TModel, TKey> node)
        {
            var dependancies = DependancyFunc(node.Item);
            node.Dependencies.Clear();

            if (dependancies == null)
                return;

            foreach (var dependancy in dependancies)
            {
                var dependantNode = Nodes
                    .Where(n => n.Identifier.Equals(dependancy))
                    .FirstOrDefault();

                if (dependantNode == null)
                    throw new InvalidOperationException("The dependency {0} hasn't been found.".With(dependancy));

                node.Dependencies.Add(dependantNode);
            }
        }

        /// <summary>
        /// Clears the list.
        /// </summary>
        [DebuggerStepThrough]
        public void Clear()
        {
            Nodes.Clear();
            modified = true;
        }

        /// <summary>
        /// Determines if the list contains the specified item.
        /// </summary>
        /// <param name="item">The item to find.</param>
        /// <returns>True if the list contains the item, otherwise false.</returns>
        [DebuggerStepThrough]
        public bool Contains(TModel item)
        {
            return Nodes.Any(n => n.Item.Equals(item));
        }

        /// <summary>
        /// Copies the items to the specified array.
        /// </summary>
        /// <param name="array">The target array.</param>
        /// <param name="index">The index at which to start copying.</param>
        [DebuggerStepThrough]
        public void CopyTo(TModel[] array, int index)
        {
            var items = Sort();

            items.CopyTo(array, index);
        }

        /// <summary>
        /// Gets an enumerator for enumerating over items in the list.
        /// </summary>
        /// <returns>An enumerator for enumerating over items in the list.</returns>
        [DebuggerStepThrough]
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Gets an enumerator for enumerating over items in the list.
        /// </summary>
        /// <returns>An enumerator for enumerating over items in the list.</returns>
        [DebuggerStepThrough]
        public IEnumerator<TModel> GetEnumerator()
        {
            var list = Sort();
            return list.GetEnumerator();
        }

        /// <summary>
        /// Gets the index of the specified item in the list.
        /// </summary>
        /// <param name="item">The item to find.</param>
        /// <returns>The index of the item in the list.</returns>
        [DebuggerStepThrough]
        public int IndexOf(TModel item)
        {
            var list = Sort();
            return list.IndexOf(item);
        }

        /// <summary>
        /// Inserts an item into the collection.  This operation is not supported.
        /// </summary>
        /// <param name="index">The index at which to insert the item.</param>
        /// <param name="item">The item to insert.</param>
        [DebuggerStepThrough]
        public void Insert(int index, TModel item)
        {
            throw new NotSupportedException("The operation Insert is not supported");
        }

        /// <summary>
        /// Removes an item from the list.
        /// </summary>
        /// <param name="item">The item to remove.</param>
        /// <returns>True if the item was removed, otherwise false.</returns>
        [DebuggerStepThrough]
        public bool Remove(TModel item)
        {
            var node = Nodes.Where(n => n.Item.Equals(item)).FirstOrDefault();
            if (node == null)
                return false;

            modified = true;
            return Nodes.Remove(node);
        }

        /// <summary>
        /// Removes the item at the specified index. This operation is not supported.
        /// </summary>
        /// <param name="index">The index of the item to remove.</param>
        [DebuggerStepThrough]
        public void RemoveAt(int index)
        {
            throw new NotSupportedException("The operation RemoveAt is not supported");
        }

        /// <summary>
        /// Returns the sorted collection of items.
        /// </summary>
        /// <returns>The sorted collection of items.</returns>
        [DebuggerStepThrough]
        internal List<TModel> Sort()
        {
            if (modified || lastSort == null)
                lastSort = SortInternal();

            return lastSort;
        }

        /// <summary>
        /// Returns the sorted collection of items.
        /// </summary>
        /// <returns>The sorted collection of items.</returns>
        [DebuggerStepThrough]
        private List<TModel> SortInternal()
        {
            var sort = new List<DependencyListNode<TModel, TKey>>();

            foreach (var n in Nodes)
            {
                n.Visited = false;
                AddDependancies(n);
            }

            foreach (var n in Nodes)
                Visit(n, sort, n);

            modified = false;

            return sort.Select(n => n.Item)
                       .ToList();
        }

        /// <summary>
        /// Gets or sets the item at the specified index. Set operations are not supported.
        /// </summary>
        /// <param name="index">The index of the item.</param>
        /// <returns>The instance at the specified index.</returns>
        public TModel this[int index]
        {
            get { return Sort()[index]; }
            set { throw new NotSupportedException("The operation this.Set is not supported"); }
        }

        /// <summary>
        /// Performs a visit on a node.
        /// </summary>
        /// <param name="node">The current node being visited.</param>
        /// <param name="list">The dependancy sorted list of items.</param>
        /// <param name="root">The root node.</param>
        /// <returns>True if the node has not previously been visited and is not the root node.</returns>
        [DebuggerStepThrough]
        private static bool Visit( DependencyListNode<TModel, TKey> node, List<DependencyListNode<TModel, TKey>> list, DependencyListNode<TModel, TKey> root)
        {
            if (node.Visited)
                return (node.Root != root);

            node.Visited = true;
            node.Root = root;

            foreach (var dependancy in node.Dependencies)
            {
                if (!Visit(dependancy, list, root) && node != root)
                    throw new InvalidOperationException("Cannot load cyclic dependency with {0}".With(dependancy.Identifier));
            }

            list.Add(node);

            return true;
        }
        #endregion
    }
}