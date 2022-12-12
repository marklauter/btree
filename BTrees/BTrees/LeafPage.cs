namespace BTrees
{
    // todo: merge leaves when underflow condition arrises. ie: count < k
    internal class LeafPage<TKey, TValue>
        : Page<TKey, TValue>
        where TKey : IComparable<TKey>
    {
        private readonly TValue[] children;

        public LeafPage(int size)
            : base(size)
        {
            this.children = new TValue[size];
        }

        internal override Page<TKey, TValue> SelectSubtree(TKey key)
        {
            // it's a leaf, so there is no subtree - this is the end of the traversal
            return this;
        }

        public override (IPage<TKey, TValue>? newPage, TKey? newPivotKey) Insert(TKey key, TValue value)
        {
            if (!this.IsFull)
            {
                this.InsertInternal(key, value);
                return (null, default);
            }

            var (newPage, newPivotKey) = this.Split();
            if (key.CompareTo(newPivotKey) <= 0)
            {
                this.InsertInternal(key, value);
            }
            else
            {
                newPage.InsertInternal(key, value);
            }

            return (newPage, newPivotKey);
        }

        internal void InsertInternal(TKey key, TValue value)
        {
            var index = this.FindInsertionIndex(key);
            if (index != this.Count)
            {
                this.ShiftRight(index);
            }

            this.Keys[index] = key;
            this.children[index] = value;
            ++this.Count;
        }

        internal void ShiftRight(int index)
        {
            for (var i = this.Count - 1; i >= index; --i)
            {
                this.Keys[i + 1] = this.Keys[i];
                this.children[i + 1] = this.children[i];
            }
        }

        private (LeafPage<TKey, TValue> newPage, TKey newPivotKey) Split()
        {
            var count = this.Count;
            var keys = new Span<TKey>(this.Keys);
            var children = new Span<TValue>(this.children);
            var newPage = new LeafPage<TKey, TValue>(this.Size);
            var newKeys = new Span<TKey>(newPage.Keys);
            var newChildren = new Span<TValue>(newPage.children);

            var copyFromIndex = count / 2;
            var j = 0;
            for (var i = copyFromIndex; i < count; ++i)
            {
                newKeys[j] = keys[i];
                newChildren[j] = children[i];
                ++j;
            }

            newPage.Count = count - copyFromIndex;
            this.Count = copyFromIndex;

            return (newPage, newPage.Keys[0]);
        }
    }
}
