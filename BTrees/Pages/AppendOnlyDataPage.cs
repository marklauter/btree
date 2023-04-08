using BTrees.Types;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace BTrees.Pages
{
    /// <summary>
    /// data page optimized for right side appends and does not support insertions and makes no effort to maintain sort order
    /// ideal for clustered index with monotonic keys
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public sealed class AppendOnlyDataPage<TKey, TValue>
        where TKey : ISizeable, IComparable<TKey>
        where TValue : ISizeable, IComparable<TValue>
    {
        public int Count => Volatile.Read(ref this.tuples).Count;

        private KeyValueCollection<TKey, TValue> tuples;

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static AppendOnlyDataPage<TKey, TValue> Empty()
        {
            return new AppendOnlyDataPage<TKey, TValue>();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private AppendOnlyDataPage()
        {
            this.tuples = KeyValueCollection<TKey, TValue>.Empty();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public AppendOnlyDataPage(int size)
        {
            this.tuples = new KeyValueCollection<TKey, TValue>(size);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private AppendOnlyDataPage(KeyValueCollection<TKey, TValue> tuples)
        {
            this.tuples = tuples;
        }

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal int IndexOf(TKey key)
        {
            var tuples = Volatile.Read(ref this.tuples);
            return IndexOf(tuples, key);
        }

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int IndexOf(KeyValueCollection<TKey, TValue> tuples, TKey key)
        {
            var keys = tuples.Items.AsSpan(..tuples.Count);

            for (var i = 0; i < keys.Length; i++)
            {
                if (keys[i].CompareTo(key) == 0)
                {
                    return i;
                }
            }

            return -1;
        }

        public void Add(TKey key, TValue value)
        {
            lock (this)
            {
                var tuples = Volatile.Read(ref this.tuples);

                // pick an insertion strategy
                tuples = Append(tuples, tuples.Count, new(key, value));

                // update tuples
                Volatile.Write(ref this.tuples, tuples);
            }
        }

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

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool ContainsKey(TKey key)
        {
            var tuples = Volatile.Read(ref this.tuples);
            return tuples.Count != 0 && IndexOf(tuples, key) >= 0;
        }

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Span<KeyValueTuple<TKey, TValue>> Read(TKey key)
        {
            throw new NotImplementedException();

            //var tuples = Volatile.Read(ref this.tuples);
            //if (tuples.Count == 0)
            //{
            //    return Span<KeyValueTuple<TKey, TValue>>.Empty;
            //}

            //var index = IndexOf(tuples, key);
            //if (index < 0)
            //{
            //    return Span<KeyValueTuple<TKey, TValue>>.Empty;
            //}

            //// find left edge
            //var start = index;
            //for (var i = index - 1; i >= 0; i--)
            //{
            //    if (tuples.Items[i].Key.CompareTo(key) != 0)
            //    {
            //        start = i + 1;
            //        break;
            //    }
            //}

            //// find right edge
            //var end = index;
            //for (var i = index + 1; i < tuples.Count; i++)
            //{
            //    if (tuples.Items[i].Key.CompareTo(key) != 0)
            //    {
            //        end = i - 1;
            //        break;
            //    }
            //}

            //// return slice
            //return tuples.Items.AsSpan(start..(end + 1));
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
