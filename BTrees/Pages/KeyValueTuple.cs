using BTrees.Types;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace BTrees.Pages
{
    public readonly record struct KeyValueTuple<TKey, TValue>(TKey Key, TValue Value)
        : IComparable<TKey>
        , IComparable<KeyValueTuple<TKey, TValue>>
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

        public int CompareTo(KeyValueTuple<TKey, TValue> other)
        {
            return this.Key.CompareTo(other.Key);
        }

        public static bool operator <(KeyValueTuple<TKey, TValue> left, KeyValueTuple<TKey, TValue> right)
        {
            return left.CompareTo(right) < 0;
        }

        public static bool operator <=(KeyValueTuple<TKey, TValue> left, KeyValueTuple<TKey, TValue> right)
        {
            return left.CompareTo(right) <= 0;
        }

        public static bool operator >(KeyValueTuple<TKey, TValue> left, KeyValueTuple<TKey, TValue> right)
        {
            return left.CompareTo(right) > 0;
        }

        public static bool operator >=(KeyValueTuple<TKey, TValue> left, KeyValueTuple<TKey, TValue> right)
        {
            return left.CompareTo(right) >= 0;
        }
    }
}
