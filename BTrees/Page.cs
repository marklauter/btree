using System.Diagnostics;

namespace BTrees
{
    [DebuggerDisplay("{Count}")]
    internal abstract class Page<TKey, TValue>
        where TKey : IComparable<TKey>
    {
        public Page(int size)
        {
            this.Size = size;
            this.Keys = new TKey[size];
        }

        public abstract (Page<TKey, TValue>? newPage, TKey? newPivotKey) Write(TKey key, TValue value);
        public abstract bool TryRead(TKey key, out TValue? value);

        internal abstract Page<TKey, TValue> SelectSubtree(TKey key);

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

        internal TKey[] Keys { get; }

        public int Count { get; protected set; }

        public int Size { get; }

        public bool IsEmpty => this.Count == 0;

        public bool IsUnderFlow => this.Count < this.Size / 2;

        public bool IsOverflow => this.Count == this.Size;
    }
}
