using System.Diagnostics;

namespace BTrees.Pages
{
    [DebuggerDisplay("{Count}")]
    internal abstract class Page<TKey, TValue>
        where TKey : IComparable<TKey>
    {
        internal TKey[] Keys { get; }

        public int Count { get; protected set; }
        public bool IsEmpty => this.Count == 0;
        public bool IsUnderFlow => this.Count < this.Size / 2;
        public bool IsOverflow => this.Count == this.Size;
        public Page<TKey, TValue>? LeftSibling { get; }
        public Page<TKey, TValue>? RightSibling { get; protected set; }
        public int Size { get; }

        #region CTOR
        public Page(int size)
        {
            this.Size = size;
            this.Keys = new TKey[size];
        }

        protected Page(
            int size,
            Page<TKey, TValue> leftSibling)
            : this(size)
        {
            this.LeftSibling = leftSibling ?? throw new ArgumentNullException(nameof(leftSibling));
        }
        #endregion

        /// <summary>
        /// Merge source page into current page
        /// </summary>
        internal abstract void Merge(Page<TKey, TValue> sourcePage);
        internal abstract Page<TKey, TValue> SelectSubtree(TKey key);
        internal abstract (Page<TKey, TValue> newPage, TKey newPivotKey) Split();

        internal int IndexOfKey(TKey key)
        {
            if (this.IsEmpty)
            {
                return 0;
            }

            var low = 0;
            var high = this.Count - 1;
            var keys = new Span<TKey>(this.Keys);

            while (low <= high)
            {
                var middle = (low + high) / 2;

                var comparison = keys[middle].CompareTo(key);

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

        internal bool CanMerge(Page<TKey, TValue>? sourcePage)
        {
            return sourcePage is not null
                && this.Count + sourcePage.Count <= this.Size;
        }

        /// <summary>
        /// Merge left on underflow.
        /// </summary>
        /// <returns>True if merge was required.</returns>
        public abstract bool TryDelete(TKey key, out (bool merged, TKey? deprecatedPivotKey) mergeInfo);

        public abstract (Page<TKey, TValue>? newPage, TKey? newPivotKey) Insert(TKey key, TValue value);

        public abstract bool TryRead(TKey key, out TValue? value);
    }
}
