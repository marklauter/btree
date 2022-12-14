namespace BTrees
{
    // todo: merge leaves when underflow condition arrises. ie: count < k
    internal class PivotPage<TKey, TValue>
        : Page<TKey, TValue>
        where TKey : IComparable<TKey>
    {
        private readonly Page<TKey, TValue>[] children;

        public PivotPage(int size)
            : base(size)
        {
            this.children = new Page<TKey, TValue>[size + 1];
        }

        public PivotPage(int size, Page<TKey, TValue> leftPage, Page<TKey, TValue> rightPage)
            : this(size)
        {
            this.Keys[0] = rightPage.Keys[0];
            this.children[0] = leftPage;
            this.children[1] = rightPage;
            this.Count = 1;
        }

        public PivotPage(int size, Page<TKey, TValue> leftPage, Page<TKey, TValue> rightPage, TKey pivotKey)
            : this(size)
        {
            this.Keys[0] = pivotKey;
            this.children[0] = leftPage;
            this.children[1] = rightPage;
            this.Count = 1;
        }

        internal override Page<TKey, TValue> SelectSubtree(TKey key)
        {
            var index = this.IndexOfKey(key);
            if (index < 0)
            {
                index = ~index;
            }

            return this.children[index];

            // todo: faster with binary search
            //var keys = new Span<TKey>(this.Keys);
            //for (var i = 0; i < this.Count; ++i)
            //{
            //    if (key.CompareTo(keys[i]) < 0)
            //    {
            //        // any left page
            //        return this.children[i];
            //    }
            //}

            //// right most page
            //return this.children[this.Count];
        }

        public override (Page<TKey, TValue>? newPage, TKey? newPivotKey) Insert(TKey key, TValue value)
        {
            var page = this.SelectSubtree(key);
            var (newSubPage, newSubPagePivotKey) = page.Insert(key, value);
            if (newSubPage is null)
            {
                return (null, default);
            }

#pragma warning disable CS8604 // Possible null reference argument. - stfu, it's not null
            if (!this.IsOverflow)
            {
                this.InsertInternal(newSubPagePivotKey, newSubPage);
                return (null, default);
            }

            var (newPage, newPivotKey) = this.Split();
            if (key.CompareTo(newPivotKey) <= 0)
            {
                this.InsertInternal(newSubPagePivotKey, newSubPage);
            }
            else
            {
                newPage.InsertInternal(newSubPagePivotKey, newSubPage);
            }
#pragma warning restore CS8604 // Possible null reference argument.

            return (newPage, newPivotKey);
        }

        private void InsertInternal(TKey key, Page<TKey, TValue> value)
        {
            var index = this.IndexOfKey(key);
            index = index > 0
                ? index + 1
                : index < 0
                    ? ~index
                    : index;

            if (index != this.Count)
            {
                this.ShiftRight(index);
            }

            this.Keys[index] = key;
            this.children[index + 1] = value;
            ++this.Count;
        }

        internal void ShiftRight(int index)
        {
            for (var i = this.Count - 1; i >= index; --i)
            {
                this.Keys[i + 1] = this.Keys[i];
                this.children[i + 2] = this.children[i + 1];
            }
        }

        private (PivotPage<TKey, TValue> newPage, TKey newPivotKey) Split()
        {
            var count = this.Count;
            var keys = new Span<TKey>(this.Keys);
            var children = new Span<Page<TKey, TValue>>(this.children);
            var newPage = new PivotPage<TKey, TValue>(this.Size);
            var newKeys = new Span<TKey>(newPage.Keys);
            var newChildren = new Span<Page<TKey, TValue>>(newPage.children);

            var newPivotIndex = count / 2;
            var copyFromIndex = newPivotIndex + 1;
            var j = 0;
            for (var i = copyFromIndex; i < count; ++i)
            {
                newKeys[j] = keys[i];
                newChildren[j] = children[i];
                ++j;
            }

            newChildren[j] = children[count];

            newPage.Count = count - newPivotIndex;
            this.Count = newPivotIndex;

            return (newPage, newKeys[0]);
        }
    }
}
