using BTrees.Types;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace BTrees.Pages
{
    internal readonly record struct KeyValueTuple<TKey, TValue>(TKey Key, TValue Value)
        : IComparable<TKey>
        , ISizeable
        where TKey : ISizeable, IComparable<TKey>
        where TValue : ISizeable, IComparable<TValue>
    {
        int ISizeable.ByteSize => this.Key.ByteSize + this.Value.ByteSize;

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int CompareTo(TKey? other)
        {
            return other is null ? -1 : this.Key.CompareTo(other);
        }
    }
}
