using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Selene.Collections
{
    public class ReferenceList<T> : IReferenceList<T>
    {
        public ref T this[int index] => ref _data[index];

        private int _count = 0;
        public int Count => _count;

        public bool IsReadOnly => false;

        public void Add(T item)
        {
            var count = _count;
            int newCount;
            if (!((newCount = count + 1) < _size))
            {
                EnsureSize(newCount);
            }
            _data[count] = item;
            _count = newCount;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public void Insert(int index, T item)
        {
            var count = _count;
            ReorderItems(index, index, index + 1, count - index, count + 1);
            _data[index] = item;
            _count = count + 1;
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

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public void RemoveAt(int index)
        {
            var count = _count;
            ReorderItems(index, index + 1, index, count - index, count);
            _count = count - 1;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public void Clear()
        {
            new Span<T>(_data, 0, _count).Clear();
            _count = 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public bool Contains(T item) => IndexOf(item) >= 0;

        public int IndexOf(T item)
        {
            for (var index = 0; index < _count; ++index)
            {
                if (Equals(_data[index], item))
                {
                    return index;
                }
            }
        
            return -1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public void CopyTo(T[] array, int arrayIndex) => new Span<T>(_data, 0, _count).CopyTo(new Span<T>(array, arrayIndex, _count));


        public IEnumerator<T> GetEnumerator() => _data.Take(_count).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        private void ReorderItems(
            int leadingCount, 
            int trailingSourceIndex, 
            int trailingTargetIndex, 
            int trailingCount, 
            int size)
        {
            int newSize;
            var newData = (newSize = CalculateSize(size)) <= _size ? _data : new T[_size = newSize];
            if (newData != _data)
                new Span<T>(_data, 0, leadingCount).CopyTo(new Span<T>(newData, 0, leadingCount));
            new Span<T>(_data, trailingSourceIndex, trailingCount).CopyTo(new Span<T>(newData, trailingTargetIndex, trailingCount));
            _data = newData;
        }
       
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        private void EnsureSize(int count)
        {
            var nextSize = CalculateSize(count);
            var currentSize = _size;
            if (currentSize == nextSize) return;
            var buffer = new T[nextSize];
            new Span<T>(_data, 0, _count).CopyTo(new Span<T>(buffer, 0, _count));
            _data = buffer;
            _size = nextSize;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        private int CalculateSize(int proposedSize) => _size >= proposedSize ? _size : _size * 2;

        private const int InitialSize = 8;
        private int _size = InitialSize;
        private T[] _data = new T[InitialSize];
    }
}