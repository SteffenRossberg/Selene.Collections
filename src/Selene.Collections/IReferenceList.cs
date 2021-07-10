using System.Collections.Generic;

namespace Selene.Collections
{
    public interface IReferenceList<T> : ICollection<T>
    {
        ref T this[int index] { get; }
    }
}