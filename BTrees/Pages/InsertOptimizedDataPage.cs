using BTrees.Types;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace BTrees.Pages
{
    /// <summary>
    /// data page optimized for random access insertions into sorted page
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public sealed class InsertOptimizedDataPage<TKey, TValue>
        where TKey : ISizeable, IComparable<TKey>
        where TValue : ISizeable, IComparable<TValue>
    {
        public int Count => Volatile.Read(ref this.tuples).Count;

        private int startOffset;
        private readonly int midpoint;
        private KeyValueCollection<TKey, TValue> tuples;

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static InsertOptimizedDataPage<TKey, TValue> Empty(int size)
        {
            return new InsertOptimizedDataPage<TKey, TValue>(size);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public InsertOptimizedDataPage(int size)
        {
            this.tuples = new KeyValueCollection<TKey, TValue>(size);
            this.midpoint = (size >> 1) - 1;
            this.startOffset = this.midpoint;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private InsertOptimizedDataPage(KeyValueCollection<TKey, TValue> tuples)
        {
            this.tuples = tuples;
        }

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal int BinarySearch(TKey key)
        {
            var tuples = Volatile.Read(ref this.tuples);
            return this.BinarySearch(tuples, key);
        }

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int BinarySearch(KeyValueCollection<TKey, TValue> tuples, TKey key)
        {
            var count = tuples.Count;
            var start = this.startOffset;
            var end = start + tuples.Count;
            var keys = tuples.Items.AsSpan(start..end);

            var low = 0;
            var high = count - 1;
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

        public void Add(TKey key, TValue value)
        {
            // todo: throw when size is exceeded as this datastructure can't allow underlying tuple collection to grow  
            lock (this)
            {
                var tuples = Volatile.Read(ref this.tuples);
                var startOffset = Volatile.Read(ref this.startOffset);

                var count = tuples.Count;
                if (count == 0)
                {
                    // set first value at midpoint left - ez mode
                    tuples = this.Append(tuples, startOffset, new(key, value));
                }
                else
                {
                    // todo: handle special case where there's room, but can't shift left anymore.
                    // uh.. something like startOffset == 0

                    // find the key insertion point
                    // if empty then skip binary search and insert at start offset which is the midpoint and don't move the start offset
                    var virtualAddress = count == 1
                        ? tuples.Items[startOffset].Key.CompareTo(key) > 0
                            ? 0 // will append left
                            : count // will append right
                        : this.BinarySearch(tuples, key);

                    virtualAddress = virtualAddress < 0
                        ? ~virtualAddress
                        : virtualAddress;

                    var physicalAddress = startOffset + virtualAddress;

                    // todo: perform range checks

                    // pick an insertion strategy
                    if (virtualAddress == 0)
                    {
                        // append left
                        var insertAddress = physicalAddress - 1;
                        tuples = this.Append(tuples, insertAddress, new(key, value));
                        startOffset = insertAddress;
                    }
                    else if (virtualAddress == count)
                    {
                        // append right
                        tuples = this.Append(tuples, physicalAddress, new(key, value));
                    }
                    else if (physicalAddress < this.midpoint)
                    {
                        tuples = InsertWithLeftShift(tuples, physicalAddress, new(key, value));
                        --startOffset;
                    }
                    else
                    {
                        tuples = InsertWithRightShift(tuples, physicalAddress, new(key, value));
                    }

                    Volatile.Write(ref this.startOffset, startOffset);
                }

                Volatile.Write(ref this.tuples, tuples);
            }
        }

        // this is the optimal case - just insert at the end without clone
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private KeyValueCollection<TKey, TValue> Append(
            KeyValueCollection<TKey, TValue> tuples,
            int index,
            KeyValueTuple<TKey, TValue> tuple)
        {
            tuples = tuples.Fork(index + 1);
            tuples.Items[index] = tuple;
            return tuples;
        }

        // this is the suboptimal case - clone, grow if required, set new count and shift tuples to the right for insert
        [Pure]
        private static KeyValueCollection<TKey, TValue> InsertWithLeftShift(
            KeyValueCollection<TKey, TValue> tuples,
            int index,
            KeyValueTuple<TKey, TValue> tuple)
        {
            tuples = tuples.Clone(tuples.Count + 1);
            var end = tuples.Count;
            var items = tuples.Items.AsSpan(..end);
            items[index..(end - 1)].CopyTo(items[(index + 1)..]);
            items[index] = tuple;
            return tuples;
        }

        [Pure]
        private static KeyValueCollection<TKey, TValue> InsertWithRightShift(
            KeyValueCollection<TKey, TValue> tuples,
            int index,
            KeyValueTuple<TKey, TValue> tuple)
        {
            tuples = tuples.Clone(tuples.Count + 1);
            var end = tuples.Count;
            var items = tuples.Items.AsSpan(..end);
            items[index..(end - 1)].CopyTo(items[(index + 1)..]);
            items[index] = tuple;
            return tuples;
        }

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool ContainsKey(TKey key)
        {
            var tuples = Volatile.Read(ref this.tuples);
            return tuples.Count != 0 && this.BinarySearch(tuples, key) >= 0;
        }

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Span<KeyValueTuple<TKey, TValue>> Read(TKey key)
        {
            var tuples = Volatile.Read(ref this.tuples);
            if (tuples.Count == 0)
            {
                return Span<KeyValueTuple<TKey, TValue>>.Empty;
            }

            var index = this.BinarySearch(tuples, key);
            if (index < 0)
            {
                return Span<KeyValueTuple<TKey, TValue>>.Empty;
            }

            // find left edge
            var start = index;
            for (var i = index - 1; i >= 0; i--)
            {
                if (tuples.Items[i].Key.CompareTo(key) != 0)
                {
                    start = i + 1;
                    break;
                }
            }

            // find right edge
            var end = index;
            for (var i = index + 1; i < tuples.Count; i++)
            {
                if (tuples.Items[i].Key.CompareTo(key) != 0)
                {
                    end = i - 1;
                    break;
                }
            }

            // return slice
            return tuples.Items.AsSpan(start..(end + 1));
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
