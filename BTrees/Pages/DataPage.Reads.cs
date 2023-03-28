using BTrees.Types;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;

namespace BTrees.Pages
{
    internal readonly partial struct DataPage<TKey, TValue>
        : IComparable<DataPage<TKey, TValue>>
        where TKey : ISizeable, IComparable<TKey>
        where TValue : ISizeable, IComparable<TValue>
    {
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

        public IEnumerable<(TKey Key, TValue Value)> Read(Range range)
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
    }
}
