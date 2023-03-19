using System.Collections.Immutable;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace BTrees.Pages
{
    [DebuggerDisplay(nameof(DataPage<TKey, TValue>))]
    internal sealed class DataPage<TKey, TValue>
        where TKey : IComparable<TKey>
        where TValue : IComparable<TValue>
    {
        private readonly record struct KeyValueTuple(TKey Key, ImmutableArray<TValue> Values)
            : IComparable<KeyValueTuple>
        {
#pragma warning disable CS8604 // Possible null reference argument.
            public static KeyValueTuple Undefined => new(default, ImmutableArray<TValue>.Empty);
#pragma warning restore CS8604 // Possible null reference argument.

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public KeyValueTuple(TKey key)
                : this(key, ImmutableArray<TValue>.Empty)
            {
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public KeyValueTuple(TKey key, TValue value)
                : this(key, ImmutableArray<TValue>.Empty.Add(value))
            {
            }

            public bool IsEmpty => this.Values.IsEmpty;
            public int Length => this.Values.Length;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public int IndexOfValue(TValue value)
            {
                return ImmutableArray.BinarySearch(this.Values, value);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool ContainsValue(TValue value)
            {
                return this.IndexOfValue(value) >= 0;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public KeyValueTuple InsertValue(int index, TValue value)
            {
                return new KeyValueTuple(this.Key, this.Values.Insert(index, value));
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool TryRemoveValue(TValue value, out KeyValueTuple tuple)
            {
                tuple = KeyValueTuple.Undefined;

                if (this.IsEmpty)
                {
                    return false;
                }

                var index = this.IndexOfValue(value);
                if (index < 0)
                {
                    return false;
                }

                tuple = new KeyValueTuple(
                    this.Key,
                    this.Values.RemoveAt(index));

                return true;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public int CompareTo(KeyValueTuple other)
            {
                return this.Key.CompareTo(other.Key);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static explicit operator TKey(KeyValueTuple tuple)
            {
                return tuple.Key;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static explicit operator KeyValueTuple(TKey key)
            {
                return new KeyValueTuple(key);
            }
        }

        public readonly record struct SplitResult(
            DataPage<TKey, TValue> LeftPage,
            DataPage<TKey, TValue> RightPage,
            TKey NewpivotKey)
        {
        }

        private readonly ImmutableArray<KeyValueTuple> tuples;

        #region ctor
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static DataPage<TKey, TValue> Empty()
        {
            return new DataPage<TKey, TValue>();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private DataPage(ImmutableArray<KeyValueTuple> tuples)
        {
            this.tuples = tuples;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private DataPage()
            : this(ImmutableArray<KeyValueTuple>.Empty)
        {
        }
        #endregion

        #region properties
        public bool IsEmpty => this.tuples.IsEmpty;
        public int Length => this.tuples.Length;
        #endregion

        #region structural modifications
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public SplitResult Split()
        {
            var length = this.tuples.Length;
            var middle = length >> 1;

            return new SplitResult(
                new DataPage<TKey, TValue>(this.tuples[..middle]),
                new DataPage<TKey, TValue>(this.tuples[middle..length]),
                this.tuples[middle].Key);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public DataPage<TKey, TValue> Merge(DataPage<TKey, TValue> otherPage)
        {
            return new DataPage<TKey, TValue>(this.tuples
                .AddRange(otherPage.tuples)
                .Sort());
        }
        #endregion

        #region read operations
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int Count()
        {
            return this.tuples.Sum(t => t.Length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int IndexOfKey(TKey key)
        {
            return ImmutableArray.BinarySearch(this.tuples, (KeyValueTuple)key);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool ContainsKey(TKey key)
        {
            return this.IndexOfKey(key) >= 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ImmutableArray<TValue> Read(TKey key)
        {
            if (this.IsEmpty)
            {
                return ImmutableArray<TValue>.Empty;
            }

            var index = this.IndexOfKey(key);
            return index >= 0
                ? this.tuples[index].Values
                : ImmutableArray<TValue>.Empty;
        }
        #endregion

        #region writes
        public bool TryDelete(TKey key, TValue value, out DataPage<TKey, TValue>? page)
        {
            page = null;

            var keyIndex = this.IndexOfKey(key);
            var containsKey = keyIndex >= 0;
            if (containsKey)
            {
                var removed = this.tuples[keyIndex].TryRemoveValue(value, out var tuple);
                if (removed)
                {
                    page = tuple.IsEmpty
                        ? new DataPage<TKey, TValue>(this.tuples.RemoveAt(keyIndex))
                        : new DataPage<TKey, TValue>(this.tuples.SetItem(keyIndex, tuple));

                    return true;
                }
            }

            return false;
        }

        public bool TryInsert(TKey key, TValue value, out DataPage<TKey, TValue>? page)
        {
            page = null;

            var keyIndex = this.IndexOfKey(key);
            var containsKey = keyIndex >= 0;
            if (containsKey)
            {
                var tuple = this.tuples[keyIndex];
                var valueIndex = tuple.IndexOfValue(value);
                var constainsValue = valueIndex >= 0;
                if (constainsValue)
                {
                    return false;
                }

                page = new DataPage<TKey, TValue>(this.tuples.SetItem(keyIndex, tuple.InsertValue(~valueIndex, value)));
            }
            else
            {
                page = new DataPage<TKey, TValue>(this.tuples.Insert(~keyIndex, new KeyValueTuple(key, value)));
            }

            return true;
        }
        #endregion
    }
}
