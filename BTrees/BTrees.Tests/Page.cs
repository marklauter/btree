namespace BTrees.Tests
{
    internal abstract class Page<TKey, TValue>
        : IPage<TKey, TValue>
        where TKey : IComparable<TKey>
    {
        public Page(int size)
        {
            this.Size = size;
            this.Keys = new TKey[size];
        }

        public abstract (IPage<TKey, TValue>? newPage, TKey? newPivotKey) Insert(TKey key, TValue value);

        internal abstract Page<TKey, TValue> SelectSubtree(TKey key);
        internal int FindInsertionIndex(TKey key)
        {
            if (this.IsEmpty)
            {
                return 0;
            }

            var high = this.Count - 1;
            var low = 0;

            // check edge cases first
            if (key.CompareTo(this.Keys[high]) >= 0) // insert at tail
            {
                return this.Count;
            }

            if (key.CompareTo(this.Keys[low]) <= 0) // insert at head
            {
                return 0;
            }

            var index = 0;
            while (low < high)
            {
                index = (high + low) / 2;

                var comparison = key.CompareTo(this.Keys[index]);
                if (comparison > 0)
                {
                    low = index + 1;
                    continue;
                }

                if (comparison < 0)
                {
                    high = index;
                    continue;
                }

                break;
            }

            return index;
        }
        internal TKey[] Keys { get; }

        public int Count { get; protected set; }
        public int Size { get; }
        public bool IsEmpty => this.Count == 0;
        public bool IsFull => this.Count == this.Size;
    }
}
