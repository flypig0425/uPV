using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace uPV.ImageView.MediaBrowser.MediaFileStore
{
    public abstract class CollectionBase<T> : IList<T>
        where T : IKeyProvider
    {
        private const string Tips = "Collection is read only.";
        private readonly IList<T> _list;
        private readonly IDictionary<string, T> _dictionary;

        protected CollectionBase()
        {
            this._list = new List<T>();
            this._dictionary = new Dictionary<string, T>();
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(T item)
        {
            _dictionary.Add(item.Key, item);
            _list.Add(item);
        }

        public void Clear()
        {
            _list.Clear();
            _dictionary.Clear();
        }
        
        public bool Contains(T item)
        {
            return ContainsKey(item.Key);
        }

        public bool ContainsKey(string key)
        {
            return _dictionary.ContainsKey(key);
        }

        public bool TryGetItem(string key, out T item)
        {
            return _dictionary.TryGetValue(key, out item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            _list.CopyTo(array, arrayIndex);
        }

        public bool Remove(T item)
        {
            _dictionary.Remove(item.Key);
            _list.Remove(item);

            return true;
        }

        public int Count
        {
            get { return _list.Count; }
        }

        public bool IsReadOnly { get { return true; } }

        public int IndexOf(T item)
        {
            return _list.IndexOf(item);
        }

        public void Insert(int index, T item)
        {
            throw new NotImplementedException();
        }

        public void RemoveAt(int index)
        {
            throw new NotImplementedException();
        }

        public T this[int index]
        {
            get { return _list[index]; }
            set { throw new NotImplementedException(Tips); }
        }

    }
}
