using BTrees.Types;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace BTrees.Pages
{
    /// <summary>
    /// data page optimized for right side insertions
    /// though it supports left side insertions, these require copy of the entire page
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    internal sealed class RightOptimizedDataPage<TKey, TValue>
        where TKey : ISizeable, IComparable<TKey>
        where TValue : ISizeable, IComparable<TValue>
    {
        public int Count => Volatile.Read(ref this.tuples).Count;

        private KeyValueCollection<TKey, TValue> tuples;

        public static RightOptimizedDataPage<TKey, TValue> Empty { get; } = new RightOptimizedDataPage<TKey, TValue>();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private RightOptimizedDataPage()
        {
            this.tuples = KeyValueCollection<TKey, TValue>.Empty;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public RightOptimizedDataPage(int size)
        {
            this.tuples = new KeyValueCollection<TKey, TValue>(size);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private RightOptimizedDataPage(KeyValueCollection<TKey, TValue> tuples)
        {
            this.tuples = tuples;
        }

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal int BinarySearch(TKey key)
        {
            var low = 0;
            var high = this.Count - 1;
            var keys = Volatile
                .Read(ref this.tuples)
                .Items
                .AsSpan(..high);

            while (low <= high)
            {
                var middle = (low + high) >> 1;
                var comparison = keys[middle]
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

        public void Insert(TKey key, TValue value)
        {
            lock (this)
            {
                var tuples = Volatile.Read(ref this.tuples);

                // find the key insertion point
                var keyIndex = this.BinarySearch(key);
                keyIndex = keyIndex < 0 ? ~keyIndex : keyIndex;

                // pick an insertion strategy
                tuples = keyIndex == tuples.Count
                    ? this.InsertRight(tuples, keyIndex, new(key, value))
                    : this.InsertLeft(tuples, keyIndex, tuples.Count, new(key, value));

                // update tuples
                Volatile.Write(ref this.tuples, tuples);
            }
        }

        // this is the optimal case - just insert at the end without clone
        private KeyValueCollection<TKey, TValue> InsertRight(
            KeyValueCollection<TKey, TValue> tuples,
            int index,
            KeyValueTuple<TKey, TValue> tuple)
        {
            tuples = tuples.Fork(index + 1);
            tuples.Items[index] = tuple;
            return tuples;
        }

        // this is the suboptimal case - clone, grow if required, set new count and shift tuples to the right for insert
        private KeyValueCollection<TKey, TValue> InsertLeft(
            KeyValueCollection<TKey, TValue> tuples,
            int index,
            int count,
            KeyValueTuple<TKey, TValue> tuple)
        {
            tuples = tuples.Clone(count + 1);
            var end = count - 1;
            var items = tuples.Items.AsSpan(..end);
            items[index..end].CopyTo(items[(index + 1)..count]);
            items[index] = tuple;
            return tuples;
        }

        public void Delete(TKey key)
        {
            // todo: when deleting a key,
            // binary search then scan left and right to find the first and last matching keys
            // and then delete the whole range
            throw new NotImplementedException();
        }
    }
}
