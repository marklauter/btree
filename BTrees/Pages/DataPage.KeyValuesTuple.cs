using BTrees.Types;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;

namespace BTrees.Pages
{
    internal readonly partial struct DataPage<TKey, TValue> where TKey : IDbType, IComparable<TKey>
        where TValue : IDbType, IComparable<TValue>
    {
        private readonly record struct KeyValuesTuple
            : IComparable<KeyValuesTuple>
            , IComparable<TKey>
        {
            public KeyValuesTuple(TKey key, ImmutableArray<TValue> values)
            {
                this.Key = key;
                this.Values = values;
                this.Size = key.Size + values.Sum(v => v.Size);
                this.IsEmpty = values.IsEmpty;
                this.Length = values.Length;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public KeyValuesTuple(TKey key, TValue value)
                : this(key, ImmutableArray<TValue>.Empty.Add(value))
            {
            }

            public TKey Key { get; }
            public ImmutableArray<TValue> Values { get; }
            public bool IsEmpty { get; }
            public int Length { get; }
            public int Size { get; }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public int IndexOf(TValue value)
            {
                return ImmutableArray.BinarySearch(this.Values, value);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool Contains(TValue value)
            {
                return this.IndexOf(value) >= 0;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public KeyValuesTuple Insert(int index, TValue value)
            {
                return new KeyValuesTuple(this.Key, this.Values.Insert(index, value));
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public KeyValuesTuple Remove(TValue value)
            {
                var index = this.IndexOf(value);
                return index < 0
                    ? this
                    : new KeyValuesTuple(this.Key, this.Values.RemoveAt(index));
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public int CompareTo(KeyValuesTuple other)
            {
                return this.Key.CompareTo(other.Key);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public int CompareTo(TKey? other)
            {
                return this.Key.CompareTo(other);
            }
        }
    }
}
