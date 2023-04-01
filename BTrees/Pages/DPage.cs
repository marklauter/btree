using BTrees.Types;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

/*
 * i have this idea where every value just gets appended to end of the values array and the key is inserted in order into the key array as a tuple (key, valueIndex)
 */

namespace BTrees.Pages
{
    internal sealed class DPage<TKey, TValue>
        where TKey : ISizeable, IComparable<TKey>
        where TValue : ISizeable, IComparable<TValue>
    {
        public int Count => Volatile.Read(ref this.tuples).Count;

        private readonly record struct KeyValuePair(TKey Key, TValue Value)
            : IComparable<TKey>
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public int CompareTo(TKey? other)
            {
                return other is null ? -1 : this.Key.CompareTo(other);
            }
        }

        private sealed class KeyValueTuples
        {
            public static KeyValueTuples Empty { get; } = new KeyValueTuples(Array.Empty<KeyValuePair>(), 0);

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public KeyValueTuples(int size)
            {
                this.KeyValuePairs = new KeyValuePair[size];
                this.Length = size;
                this.Count = 0;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            internal KeyValueTuples(TKey key, TValue value)
            {
                this.KeyValuePairs = new KeyValuePair[] { new KeyValuePair(key, value) };
                this.Length = 1;
                this.Count = 1;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            internal KeyValueTuples(
                KeyValuePair[] keyValuePairs,
                int count)
            {
                this.KeyValuePairs = keyValuePairs;
                this.Length = keyValuePairs.Length;
                this.Count = count;
            }

            public KeyValueTuples Grow()
            {
                var newLength = this.KeyValuePairs.Length == 0
                    ? 16
                    : this.KeyValuePairs.Length << 1;

                var end = this.Count - 1;
                var keyValuePairs = new KeyValuePair[newLength];
                this.KeyValuePairs.AsSpan(..end)
                    .CopyTo(keyValuePairs.AsSpan(..end));

                return new KeyValueTuples(keyValuePairs, this.Count);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public KeyValueTuples Fork()
            {
                return new KeyValueTuples(
                    this.KeyValuePairs,
                    this.Count);
            }

            /// <summary>
            /// Performs deep copy
            /// </summary>
            /// <param name="count">new count</param>
            /// <returns>deep copy clone with new count</returns>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public KeyValueTuples Clone(int count)
            {
                return count <= this.KeyValuePairs.Length
                    ? new KeyValueTuples(this.KeyValuePairs.AsSpan().ToArray(), count)
                    : this.Grow();
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public KeyValueTuples Clone()
            {
                return new KeyValueTuples(
                    this.KeyValuePairs.AsSpan().ToArray(),
                    this.Count);
            }

            public readonly KeyValuePair[] KeyValuePairs;
            public readonly int Length;
            public readonly int Count;
        }

        private KeyValueTuples tuples;

        public static DPage<TKey, TValue> Empty { get; } = new DPage<TKey, TValue>();

        public DPage(TKey key, TValue value)
        {
            this.tuples = new KeyValueTuples(key, value);
        }

        public DPage(int size)
        {
            this.tuples = new KeyValueTuples(size);
        }

        private DPage(KeyValueTuples tuples, int count)
        {
            this.tuples = tuples;
            this.Count = count;
        }

        private DPage()
        {
            this.tuples = KeyValueTuples.Empty;
        }

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal int IndexOf(TKey key)
        {
            var low = 0;
            var high = this.Count - 1;
            var keys = this.tuples.KeyValuePairs.AsSpan(..high);

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
                var count = tuples.Count;
                var newCount = count + 1;

                // does tuples require array expansion?
                tuples = tuples.Clone(count);

                // find the key insertion point
                var keyIndex = this.IndexOf(key);
                keyIndex = keyIndex < 0 ? ~keyIndex : keyIndex;

                // shift right one space
                // todo: this might be better as a balanced tree
                var end = count - 1;
                var keyIndexPairs = tuples.KeyValuePairs.AsSpan(..end);
                keyIndexPairs[keyIndex..end]
                    .CopyTo(keyIndexPairs[(keyIndex + 1)..count]);

                // insert record
                keyIndexPairs[keyIndex] = new(key, value);

                Volatile.Write(ref this.tuples, tuples);
            }
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
