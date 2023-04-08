﻿using BTrees.Types;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace BTrees.Pages
{
    /// <summary>
    /// data page optimized for right side appends, but capable of insertions
    /// though it supports left side insertions, these require copy of the entire page
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public sealed class RightOptimizedDataPage<TKey, TValue>
        where TKey : ISizeable, IComparable<TKey>
        where TValue : ISizeable, IComparable<TValue>
    {
        public int Count => Volatile.Read(ref this.tuples).Count;

        private KeyValueCollection<TKey, TValue> tuples;

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RightOptimizedDataPage<TKey, TValue> Empty()
        {
            return new RightOptimizedDataPage<TKey, TValue>();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private RightOptimizedDataPage()
        {
            this.tuples = KeyValueCollection<TKey, TValue>.Empty();
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
            var tuples = Volatile.Read(ref this.tuples);
            return BinarySearch(tuples, key);
        }

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int BinarySearch(KeyValueCollection<TKey, TValue> tuples, TKey key)
        {
            var low = 0;
            var high = tuples.Count - 1;
            var keys = tuples.Items.AsSpan(..tuples.Count);

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
            lock (this)
            {
                var tuples = Volatile.Read(ref this.tuples);

                // find the key insertion point
                var keyIndex = tuples.Count != 0
                    ? BinarySearch(tuples, key)
                    : 0;

                keyIndex = keyIndex < 0 ? ~keyIndex : keyIndex;

                // pick an insertion strategy
                tuples = keyIndex == tuples.Count
                    ? Append(tuples, keyIndex, new(key, value))
                    : Insert(tuples, keyIndex, new(key, value));

                // update tuples
                Volatile.Write(ref this.tuples, tuples);
            }
        }

        // this is the optimal case - just insert at the end without clone
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static KeyValueCollection<TKey, TValue> Append(
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
        private static KeyValueCollection<TKey, TValue> Insert(
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
            return tuples.Count != 0 && BinarySearch(tuples, key) >= 0;
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

            var index = BinarySearch(tuples, key);
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
