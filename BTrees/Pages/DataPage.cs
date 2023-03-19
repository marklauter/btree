using System.Collections.Immutable;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace BTrees.Pages
{
    [DebuggerDisplay(nameof(DataPage<TKey, TValue>))]
    internal sealed class DataPage<TKey, TValue>
        where TKey : struct, IComparable<TKey>
        where TValue : IComparable<TValue>
    {
        private readonly record struct KeyValuesTuple(TKey Key, ImmutableArray<TValue> Values)
            : IComparable<KeyValuesTuple>
            , IComparable<TKey>
        {
            public static KeyValuesTuple Undefined { get; } = new(default, ImmutableArray<TValue>.Empty);

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public KeyValuesTuple(TKey key)
                : this(key, ImmutableArray<TValue>.Empty)
            {
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public KeyValuesTuple(TKey key, TValue value)
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
            public KeyValuesTuple InsertValue(int index, TValue value)
            {
                return new KeyValuesTuple(this.Key, this.Values.Insert(index, value));
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool TryRemoveValue(TValue value, out KeyValuesTuple tuple)
            {
                tuple = Undefined;

                if (this.IsEmpty)
                {
                    return false;
                }

                var index = this.IndexOfValue(value);
                if (index < 0)
                {
                    return false;
                }

                tuple = new KeyValuesTuple(
                    this.Key,
                    this.Values.RemoveAt(index));

                return true;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public int CompareTo(KeyValuesTuple other)
            {
                return this.Key.CompareTo(other.Key);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public int CompareTo(TKey other)
            {
                return this.Key.CompareTo(other);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static explicit operator TKey(KeyValuesTuple tuple)
            {
                return tuple.Key;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static explicit operator KeyValuesTuple(TKey key)
            {
                return new KeyValuesTuple(key);
            }
        }

        public readonly record struct SplitResult(
            DataPage<TKey, TValue> LeftPage,
            DataPage<TKey, TValue> RightPage,
            TKey PivotKey)
        {
        }

        private readonly ImmutableArray<KeyValuesTuple> tuples;

        #region ctor
        public static DataPage<TKey, TValue> Empty { get; } = new DataPage<TKey, TValue>();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private DataPage(ImmutableArray<KeyValuesTuple> tuples)
        {
            this.tuples = tuples;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private DataPage()
            : this(ImmutableArray<KeyValuesTuple>.Empty)
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
            if (this.tuples.Length < 2)
            {
                throw new InvalidOperationException("Can't split page with less than two elements.");
            }

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
        internal int IndexOfKey(TKey key)
        {
            var low = 0;
            var high = this.tuples.Length - 1;
            var tuples = this.tuples;

            while (low <= high)
            {
                var middle = (low + high) >> 1;
                var comparison = tuples[middle]
                    .CompareTo(key);

                if (comparison == 0)
                {
                    return middle;
                }

                if (comparison < 0)
                {
                    low = middle + 1;
                    continue;
                }

                high = middle - 1;
                continue;
            }

            return ~low;
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
        public DataPage<TKey, TValue> Delete(TKey key)
        {
            var page = this;
            var keyIndex = this.IndexOfKey(key);
            var containsKey = keyIndex >= 0;
            if (containsKey)
            {
                page = new DataPage<TKey, TValue>(this.tuples.RemoveAt(keyIndex));
            }

            return page;
        }

        public DataPage<TKey, TValue> Delete(TKey key, TValue value)
        {
            var page = this;
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
                }
            }

            return page;
        }

        public DataPage<TKey, TValue> Insert(TKey key, TValue value)
        {
            var page = this;
            var keyIndex = this.IndexOfKey(key);
            var containsKey = keyIndex >= 0;
            if (containsKey)
            {
                var tuple = this.tuples[keyIndex];
                var valueIndex = tuple.IndexOfValue(value);
                var constainsValue = valueIndex >= 0;
                if (!constainsValue)
                {
                    page = new DataPage<TKey, TValue>(this.tuples.SetItem(keyIndex, tuple.InsertValue(~valueIndex, value)));
                }
            }
            else
            {
                page = new DataPage<TKey, TValue>(this.tuples.Insert(~keyIndex, new KeyValuesTuple(key, value)));
            }

            return page;
        }
        #endregion
    }
}
