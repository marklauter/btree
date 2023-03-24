using BTrees.Types;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;

namespace BTrees.Pages
{
    internal readonly partial struct DataPage<TKey, TValue>
        : IComparable<DataPage<TKey, TValue>>
        where TKey : IDbType, IComparable<TKey>
        where TValue : IDbType, IComparable<TValue>
    {
        private readonly ImmutableArray<KeyValuesTuple> tuples;
        private readonly int searchHigh;
        private readonly TKey? minKey;

        #region ctor
        public static DataPage<TKey, TValue> Empty { get; } = new DataPage<TKey, TValue>(ImmutableArray<KeyValuesTuple>.Empty);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private DataPage(ImmutableArray<KeyValuesTuple> tuples)
        {
            this.tuples = tuples;
            this.Size = tuples.Sum(t => t.Size);
            this.IsEmpty = tuples.IsEmpty;
            this.Length = tuples.Length;
            this.searchHigh = tuples.Length - 1;
            this.minKey = tuples.IsEmpty ? default : tuples[0].Key;
        }
        #endregion

        #region properties
        public bool IsEmpty { get; }
        public int Length { get; }
        public int Size { get; }
        public TKey MinKey => this.IsEmpty || this.minKey is null
            ? throw new InvalidOperationException("Empty DataPage doesn't have MinKey")
            : this.minKey;
        #endregion

        #region structural modifications
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public SplitResult Split()
        {
            if (this.tuples.Length < 2)
            {
                throw new InvalidOperationException("Can't split page with less than two elements.");
            }

            var length = this.Length;
            var middle = length >> 1;

            return new SplitResult(
                new DataPage<TKey, TValue>(this.tuples[..middle]),
                new DataPage<TKey, TValue>(this.tuples[middle..length]));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public DataPage<TKey, TValue> Merge(DataPage<TKey, TValue> otherPage)
        {
            return this.tuples[0].CompareTo(otherPage.tuples[0]) < 0
                ? new DataPage<TKey, TValue>(this.tuples.AddRange(otherPage.tuples).Sort())
                : new DataPage<TKey, TValue>(otherPage.tuples.AddRange(this.tuples).Sort());
        }
        #endregion

        #region read operations
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int Count()
        {
            return this.tuples.Sum(t => t.Length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal int IndexOf(TKey key)
        {
            var low = 0;
            var high = this.searchHigh;
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

                high = comparison > 0 ? middle - 1 : high;
                low = comparison < 0 ? middle + 1 : low;
            }

            return ~low;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool ContainsKey(TKey key)
        {
            return this.IndexOf(key) >= 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ImmutableArray<TValue> Read(TKey key)
        {
            if (this.IsEmpty)
            {
                return ImmutableArray<TValue>.Empty;
            }

            var index = this.IndexOf(key);
            return index >= 0
                ? this.tuples[index].Values
                : ImmutableArray<TValue>.Empty;
        }

        public IEnumerable<(TKey, TValue)> Read(Range range)
        {
            return this.tuples[range]
               .SelectMany(
                tuple => tuple.Values,
                (tuple, value) => (tuple.Key, value));
        }

        public int CompareTo(DataPage<TKey, TValue> other)
        {
#pragma warning disable CS8602 // Dereference of a possibly null reference. - this.minKey is not null if this.IsEmpty is false
            return this.tuples == other.tuples
                ? 0
                : this.IsEmpty && !other.IsEmpty
                    ? -1
                    : !this.IsEmpty && other.IsEmpty
                        ? 1
                        : this.minKey.CompareTo(other.minKey);
#pragma warning restore CS8602 // Dereference of a possibly null reference.
        }
        #endregion

        #region write operations
        public DataPage<TKey, TValue> Remove(TKey key)
        {
            var page = this;
            var keyIndex = this.IndexOf(key);
            var containsKey = keyIndex >= 0;
            if (containsKey)
            {
                page = new DataPage<TKey, TValue>(this.tuples.RemoveAt(keyIndex));
            }

            return page;
        }

        public DataPage<TKey, TValue> Remove(TKey key, TValue value)
        {
            var page = this;
            var keyIndex = this.IndexOf(key);
            var containsKey = keyIndex >= 0;
            if (containsKey)
            {
                var tuple = this.tuples[keyIndex];
                var removeResult = tuple.Remove(value);
                if (tuple != removeResult)
                {
                    page = removeResult.IsEmpty
                        ? new DataPage<TKey, TValue>(this.tuples.RemoveAt(keyIndex))
                        : new DataPage<TKey, TValue>(this.tuples.SetItem(keyIndex, removeResult));
                }
            }

            return page;
        }

        public DataPage<TKey, TValue> Insert(TKey key, TValue value)
        {
            var page = this;
            var keyIndex = this.IndexOf(key);
            var containsKey = keyIndex >= 0;
            if (containsKey)
            {
                var tuple = this.tuples[keyIndex];
                var valueIndex = tuple.IndexOf(value);
                var constainsValue = valueIndex >= 0;
                if (!constainsValue)
                {
                    page = new DataPage<TKey, TValue>(this.tuples.SetItem(keyIndex, tuple.Insert(~valueIndex, value)));
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
