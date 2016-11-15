using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Corvalius.Common
{
    public sealed class Filter<T> : IEnumerable<Predicate<T>>
    {
        #region Fields

        private List<Predicate<T>> predicates;
        private Predicate<T> single;

        private Action<Predicate<T>> add;
        private Func<Predicate<T>, bool> remove;

        #endregion Fields

        public Filter()
            : this(null, null)
        {
        }

        internal Filter(Action<Predicate<T>> add, Func<Predicate<T>, bool, bool> remove)
        {
            this.predicates = new List<Predicate<T>>();
            this.single = (T t) => this.predicates.All(filter => filter(t));

            Action<Predicate<T>> baseAdd = predicate => this.predicates.Add(predicate);
            Func<Predicate<T>, bool> baseRemove = predicate => this.predicates.Remove(predicate);

            if (add != null)
            {
                this.add = predicate =>
                    {
                        baseAdd(predicate);
                        add(predicate);
                    };
            }
            else
            {
                this.add = baseAdd;
            }

            if (remove != null)
            {
                this.remove = predicate =>
                    {
                        bool baseResult = baseRemove(predicate);
                        return remove(predicate, baseResult);
                    };
            }
            else
            {
                this.remove = baseRemove;
            }
        }

        #region Core

        /// <summary>
        /// Add the predicate to the filter
        /// </summary>
        /// <param name="predicate"></param>
        public void Add(Predicate<T> predicate)
        {
            this.add(predicate);
        }

        /// <summary>
        /// Remove the predicate from the filter
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public bool Remove(Predicate<T> predicate)
        {
            return this.remove(predicate);
        }

        /// <summary>
        /// Collapse all predicates into one
        /// </summary>
        public Predicate<T> Single
        {
            get { return this.single; }
        }

        /// <summary>
        /// Evaluate a single value against all the current filters
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool Evaluate(T value)
        {
            return this.single(value);
        }

        #endregion Core

        /// <summary>
        /// Optional method re-evaluate the filter. 
        /// Defaults to null
        /// </summary>
        public Action Refresh { get; internal set; }

        #region IEnumerable<Predicate<T>> Members

        public IEnumerator<Predicate<T>> GetEnumerator()
        {
            return this.predicates.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        #endregion
    }
}
