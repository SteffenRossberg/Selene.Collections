using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Selene.Collections
{
    public interface IReferenceList<T> : ICollection<T>
    {
        ref T this[int index] { get; }
    }
    
    public class ReferenceList<T> : IReferenceList<T>
    {

        public ref T this[int index] => throw new System.NotImplementedException();

        public int Count { 
            get => throw new NotImplementedException();
            private set => throw new NotImplementedException(); 
        }
        
        public bool IsReadOnly => throw new NotImplementedException();

        public void Add(T item)
        {
            throw new System.NotImplementedException();
        }

        public void Clear()
        {
            throw new System.NotImplementedException();
        }

        public bool Contains(T item)
        {
            throw new System.NotImplementedException();
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            throw new System.NotImplementedException();
        }

        public bool Remove(T item)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerator<T> GetEnumerator()
        {
            throw new System.NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public int IndexOf(T item)
        {
            throw new System.NotImplementedException();
        }

        public void Insert(int index, T item)
        {
            throw new System.NotImplementedException();
        }

        public void RemoveAt(int index)
        {
            throw new System.NotImplementedException();
        }
    }
}