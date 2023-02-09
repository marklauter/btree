namespace BTrees.Pages
{
    internal abstract class Page<TKey, TValue>
        : IPage<TKey, TValue>
        where TKey : IComparable<TKey>
    {
        private readonly int halfSize;

        protected Page()
        {
            this.Size = 0;
        }

        protected Page(int size)
        {
            if (size < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(size), $"{nameof(size)} must be > 0");
            }

            this.Size = size;
            this.halfSize = size / 2;
        }

        public abstract int Count { get; }
        public bool IsEmpty => this.Count == 0;
        public bool IsOverflow => this.Count > this.Size && this.Count != 0;
        public bool IsUnderflow => this.Count < this.halfSize || this.Count == 0;
        public int Size { get; }
        public abstract TKey MinKey { get; }
        public abstract TKey MaxKey { get; }

        public abstract bool ContainsKey(TKey key);
        public abstract IPage<TKey, TValue> Delete(TKey key);
        public abstract IPage<TKey, TValue> Fork();
        public abstract int BinarySearch(TKey key);
        public abstract IPage<TKey, TValue> Insert(TKey key, TValue value);
        public abstract IPage<TKey, TValue> Merge(IPage<TKey, TValue> page);
        public abstract (IPage<TKey, TValue> leftPage, IPage<TKey, TValue> rightPage, TKey pivotKey) Split();
        public abstract bool TryRead(TKey key, out TValue? value);
        public abstract IPage<TKey, TValue> Update(TKey key, TValue value);

        public int CompareTo(IPage<TKey, TValue>? other)
        {
            return other is null
                ? -1
                : this == other
                    ? 0
                    : this.MinKey.CompareTo(other.MinKey);
        }
    }
}
