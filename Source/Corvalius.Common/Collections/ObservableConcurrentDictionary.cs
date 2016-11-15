using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;

namespace Corvalius.Collections
{
    public class ObservableConcurrentDictionary<TKey, TValue> : IDictionary<TKey, TValue>, INotifyCollectionChanged, INotifyPropertyChanged
    {
        private readonly PropertyChangedEventArgs countChangedEvent = new PropertyChangedEventArgs("Count");
        private readonly PropertyChangedEventArgs keysChangedEvent = new PropertyChangedEventArgs("Keys");
        private readonly PropertyChangedEventArgs valuesChangedEvent = new PropertyChangedEventArgs("Values");

        private readonly ConcurrentDictionary<TKey, TValue> storage;

        #region Constructors

        public ObservableConcurrentDictionary(IEnumerable<KeyValuePair<TKey, TValue>> collection)
        {
            storage = new ConcurrentDictionary<TKey, TValue>(collection);
        }

        public ObservableConcurrentDictionary(IEqualityComparer<TKey> comparer)
        {
            storage = new ConcurrentDictionary<TKey, TValue>(comparer);
        }

        public ObservableConcurrentDictionary(IEnumerable<KeyValuePair<TKey, TValue>> collection, IEqualityComparer<TKey> comparer)
        {
            storage = new ConcurrentDictionary<TKey, TValue>(collection, comparer);
        }

        public ObservableConcurrentDictionary(int concurrencyLevel, IEnumerable<KeyValuePair<TKey, TValue>> collection, IEqualityComparer<TKey> comparer)
        {
            storage = new ConcurrentDictionary<TKey, TValue>(concurrencyLevel, collection, comparer);
        }

        public ObservableConcurrentDictionary(int concurrencyLevel, int capacity, IEqualityComparer<TKey> comparer)
        {
            storage = new ConcurrentDictionary<TKey, TValue>(concurrencyLevel, capacity, comparer);
        }

        public ObservableConcurrentDictionary(int concurrencyLevel, int capacity)
        {
            storage = new ConcurrentDictionary<TKey, TValue>(concurrencyLevel, capacity);
        }

        public ObservableConcurrentDictionary()
        {
            storage = new ConcurrentDictionary<TKey, TValue>();
        }

        #endregion        

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return storage.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item)
        {
            var collection = (ICollection<KeyValuePair<TKey, TValue>>)storage;
            collection.Add(item);

            CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item));
            PropertyChanged(this, countChangedEvent);
            PropertyChanged(this, keysChangedEvent);
            PropertyChanged(this, valuesChangedEvent);
        }

        void ICollection<KeyValuePair<TKey, TValue>>.Clear()
        {
            var collection = (ICollection<KeyValuePair<TKey, TValue>>)storage;
            collection.Clear();

            CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            PropertyChanged(this, countChangedEvent);
            PropertyChanged(this, keysChangedEvent);
            PropertyChanged(this, valuesChangedEvent);
        }

        bool ICollection<KeyValuePair<TKey,TValue>>.Contains(KeyValuePair<TKey, TValue> item)
        {
            var collection = (ICollection<KeyValuePair<TKey, TValue>>)storage;
            return collection.Contains(item);
        }

        void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            var collection = (ICollection<KeyValuePair<TKey, TValue>>)storage;
            collection.CopyTo(array, arrayIndex);

            CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item)
        {
            var collection = (ICollection<KeyValuePair<TKey, TValue>>)storage;
            return collection.Remove(item);
        }

        public int Count
        {
            get { return storage.Count; }
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly
        {
            get
            {
                var collection = (ICollection<KeyValuePair<TKey, TValue>>) storage;
                return collection.IsReadOnly;
            }
        }

        public bool ContainsKey(TKey key)
        {
            return storage.ContainsKey(key);
        }

        void IDictionary<TKey, TValue>.Add(TKey key, TValue value)
        {
            var dictionary = (IDictionary<TKey, TValue>)storage;
            dictionary.Add(key, value);

            CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, new KeyValuePair<TKey, TValue>(key, value)));
            PropertyChanged(this, countChangedEvent);
            PropertyChanged(this, keysChangedEvent);
            PropertyChanged(this, valuesChangedEvent);
        }

        bool IDictionary<TKey, TValue>.Remove(TKey key)
        {
            var dictionary = (IDictionary<TKey, TValue>)storage;

            var item = dictionary[key];
            var result = dictionary.Remove(key);

            CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, new KeyValuePair<TKey, TValue>(key, item)));
            PropertyChanged(this, countChangedEvent);
            PropertyChanged(this, keysChangedEvent);
            PropertyChanged(this, valuesChangedEvent);

            return result;
        }

        bool IDictionary<TKey, TValue>.TryGetValue(TKey key, out TValue value)
        {
            return TryGetValue(key, out value);
        }

        public TValue this[TKey key]
        {
            get { return storage[key]; }
            set { this.storage[key] = value; }
        }

        public ICollection<TKey> Keys
        {
            get { return storage.Keys; }
        }

        public ICollection<TValue> Values
        {
            get { return storage.Values; }
        }

        public event NotifyCollectionChangedEventHandler CollectionChanged = (sender, args) => { };
        public event PropertyChangedEventHandler PropertyChanged = (sender, args) => { };

        #region Concurrent Dictionary Methods

        public virtual TValue AddOrUpdate(TKey key, TValue addValue, Func<TKey, TValue, TValue> updateValueFactory)
        {
            var result = storage.AddOrUpdate(key, addValue, updateValueFactory);
            CollectionChanged(this, result.Equals(addValue)
                                  ? new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, new KeyValuePair<TKey, TValue>(key, addValue))
                                  : new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, new KeyValuePair<TKey, TValue>(key, addValue), new KeyValuePair<TKey, TValue>(key, result)));

            return result;
        }

        public virtual TValue AddOrUpdate(TKey key, Func<TKey, TValue> addValueFactory, Func<TKey, TValue, TValue> updateValueFactory)
        {
            return AddOrUpdate(key, addValueFactory(key), updateValueFactory);
        }

        public virtual TValue GetOrAdd(TKey key, TValue value)
        {
            var result = storage.GetOrAdd(key, value);
            if (result.Equals(value))
            {
                CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, new KeyValuePair<TKey, TValue>(key, value)));
                PropertyChanged(this, countChangedEvent);
                PropertyChanged(this, keysChangedEvent);
                PropertyChanged(this, valuesChangedEvent);
            }

            return result;
        }

        public virtual TValue GetOrAdd(TKey key, Func<TKey, TValue> func)
        {
            return GetOrAdd(key, func(key));
        }

        public virtual bool TryAdd(TKey key, TValue value)
        {
            var result = storage.TryAdd(key, value);
            if (result)
            {
                CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, new KeyValuePair<TKey, TValue>(key, value)));
                PropertyChanged(this, countChangedEvent);
                PropertyChanged(this, keysChangedEvent);
                PropertyChanged(this, valuesChangedEvent);
            }
            return result;
        }

        public virtual bool TryGetValue(TKey key, out TValue value)
        {
            return storage.TryGetValue(key, out value);
        }

        public virtual bool TryRemove(TKey key, out TValue value)
        {
            var result = storage.TryRemove(key, out value);
            if (result)
            {
                CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, new KeyValuePair<TKey, TValue>(key, value)));
                PropertyChanged(this, countChangedEvent);
                PropertyChanged(this, keysChangedEvent);
                PropertyChanged(this, valuesChangedEvent);
            }
            return result;
        }

        public virtual bool TryUpdate(TKey key, TValue newValue, TValue comparisonValue)
        {
            var result = storage.TryUpdate(key, newValue, comparisonValue);
            if (result)
                CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, new KeyValuePair<TKey, TValue>(key, newValue)));

            return result;
        }

        #endregion
    }
}
