using BTrees.Types;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace BTrees.Pages
{
    internal sealed class KeyValueCollection<TKey, TValue>
        where TKey : ISizeable, IComparable<TKey>
        where TValue : ISizeable, IComparable<TValue>
    {
        private const int MIN_SIZE = 16;

        public static KeyValueCollection<TKey, TValue> Empty()
        {
            return new KeyValueCollection<TKey, TValue>(MIN_SIZE);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public KeyValueCollection(int size)
        {
            this.Items = new KeyValueTuple<TKey, TValue>[size];
            this.Length = size;
            this.Count = 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private KeyValueCollection(
            KeyValueTuple<TKey, TValue>[] items,
            int count)
        {
            this.Items = items;
            this.Length = items.Length;
            this.Count = count;
        }

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private KeyValueCollection<TKey, TValue> Grow(int newCount)
        {
            // double the array
            var newLength = this.Items.Length == 0
                ? MIN_SIZE
                : this.Items.Length << 1;

            // copy old array to new array
            var items = new KeyValueTuple<TKey, TValue>[newLength];
            this.Items
                .AsSpan(..this.Count)
                .CopyTo(items.AsSpan(..this.Count));

            // return new collection
            return new KeyValueCollection<TKey, TValue>(items, newCount);
        }

        /// <summary>
        /// performs shallow copy
        /// </summary>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public KeyValueCollection<TKey, TValue> Fork()
        {
            return new KeyValueCollection<TKey, TValue>(
                this.Items,
                this.Count);
        }

        /// <summary>
        /// performs shallow copy, unless grow is required, and sets new count 
        /// </summary> 
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public KeyValueCollection<TKey, TValue> Fork(int newCount)
        {
            return newCount > this.Length
                ? this.Grow(newCount)
                : new KeyValueCollection<TKey, TValue>(
                    this.Items,
                    newCount);
        }

        /// <summary>
        /// performs deep copy, unless grow is required, and sets new count
        /// </summary>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public KeyValueCollection<TKey, TValue> Clone(int newCount)
        {
            return newCount > this.Length
                ? this.Grow(newCount)
                : new KeyValueCollection<TKey, TValue>(
                    this.Items.AsSpan().ToArray(),
                    newCount);
        }

        public readonly KeyValueTuple<TKey, TValue>[] Items;
        public readonly int Length;
        public readonly int Count;
    }
}
