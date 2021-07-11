using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Selene.Collections
{
    public class ReferenceList<T> : IReferenceList<T>
    {
        public ref T this[int index] => ref _data[index];

        public int Count { get; private set; }
        
        public bool IsReadOnly => throw new NotImplementedException();

        public void Add(T item)
        {
            var index = Count;
            EnsureSize(index + 1);
            _data[index] = item;
            Count = index + 1;
        }

        public void Insert(int index, T item)
        {
            throw new System.NotImplementedException();
        }

        public bool Remove(T item)
        {
            var index = IndexOf(item);

            if (index >= 0)
            {
                RemoveAt(index);
                return true;
            }

            return false;
        }

        public void RemoveAt(int index)
        {
            var count = Count;
            var newData = new T[_data.Length];
            if(index > 0)
                Array.Copy(_data, newData, index);
            if (count - index > 1)
                Array.Copy(_data, index + 1, newData, index, count - index - 1);
            _data = newData;
            Count = count - 1;
        }

        public void Clear()
        {
            _data = new T[_data.Length];
            Count = 0;
        }

        public bool Contains(T item)
        {
            throw new System.NotImplementedException();
        }

        public int IndexOf(T item)
        {
            var count = Count;

            for (var index = 0; index < count; ++index)
            {
                if (Equals(_data[index], item))
                {
                    return index;
                }
            }

            return -1;
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerator<T> GetEnumerator() => Enumerate().GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        private IEnumerable<T> Enumerate()
        {
            var count = Count;
            for (var index = 0; index < count; ++index)
                yield return _data[index];
        }

        private void EnsureSize(int count)
        {
            var currentSize = _data.Length;
            if (currentSize > count) return;
            var newData = new T[currentSize * 2];
            Array.Copy(_data, newData, currentSize);
            _data = newData;
        }

        private T[] _data = new T[4];
    }
}