using System.Diagnostics;

namespace BTrees.Pages
{
    [DebuggerDisplay("PivotPage {Count}")]
    internal class PivotPage<TKey, TValue>
        : Page<TKey, TValue>
        where TKey : IComparable<TKey>
    {
        internal readonly Page<TKey, TValue>[] children;

        #region CTOR
        public PivotPage(int size)
            : base(size)
        {
            this.children = new Page<TKey, TValue>[size + 1];
        }

        public PivotPage(
            int size,
            Page<TKey, TValue> leftPage,
            Page<TKey, TValue> rightPage,
            TKey pivotKey)
            : this(size)
        {
            this.Keys[0] = pivotKey;
            this.children[0] = leftPage;
            this.children[1] = rightPage;
            this.Count = 1;
        }

        internal PivotPage(int size, Page<TKey, TValue> leftSibling)
            : base(size, leftSibling)
        {
            this.children = new Page<TKey, TValue>[size + 1];
        }
        #endregion

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

        protected override void ShiftLeft(int index)
        {
            for (var i = index; i < this.Count - 1; ++i)
            {
                this.Keys[i] = this.Keys[i + 1];
                this.children[i + 1] = this.children[i + 2];
            }
        }

        protected override void ShiftRight(int index)
        {
            for (var i = this.Count - 1; i >= index; --i)
            {
                this.Keys[i + 1] = this.Keys[i];
                this.children[i + 2] = this.children[i + 1];
            }
        }

        internal override void Merge(Page<TKey, TValue> sourcePage)
        {
            var startIndex = this.Count;
            var endIndex = sourcePage.Count + startIndex;
            var keys = new Span<TKey>(this.Keys);
            var children = new Span<Page<TKey, TValue>>(this.children);
            var sourceKeys = new Span<TKey>(this.Keys);
            var sourceChildren = new Span<Page<TKey, TValue>>(this.children);

            var j = 0;
            for (var i = startIndex; i < endIndex; ++i)
            {
                keys[i] = sourceKeys[j];
                children[i] = sourceChildren[j];
                ++j;
            }

            children[endIndex] = sourceChildren[j];

            this.Count = endIndex;
        }

        internal override Page<TKey, TValue> SelectSubtree(TKey key)
        {
            var index = this.IndexOfKey(key);
            index = index < 0
                ? ~index
                : index + 1;

            return this.children[index];
        }

        internal override (Page<TKey, TValue> newPage, TKey newPivotKey) Split()
        {
            var count = this.Count;
            var keys = new Span<TKey>(this.Keys);
            var children = new Span<Page<TKey, TValue>>(this.children);
            var newPage = new PivotPage<TKey, TValue>(this.Size, this);
            var newKeys = new Span<TKey>(newPage.Keys);
            var newChildren = new Span<Page<TKey, TValue>>(newPage.children);

            var newPivotIndex = count / 2;
            var j = 0;
            for (var i = newPivotIndex; i < count; ++i)
            {
                newKeys[j] = keys[i];
                newChildren[j] = children[i];
                ++j;
            }

            newChildren[j] = children[count];

            newPage.PivotKey = newKeys[0];
            newPage.Count = count - newPivotIndex;
            this.Count = newPivotIndex;

            return (newPage, newPage.PivotKey);
        }

        public override bool TryDelete(TKey key, out (bool merged, TKey? deprecatedPivotKey) mergeInfo)
        {
            mergeInfo.merged = false;
            mergeInfo.deprecatedPivotKey = default;

            var page = this.SelectSubtree(key);
            var deleted = page.TryDelete(key, out var subTreeMergeInfo);

            if (deleted && subTreeMergeInfo.merged)
            {
#pragma warning disable CS8604 // Possible null reference argument.
                _ = this.RemoveKey(subTreeMergeInfo.deprecatedPivotKey, out mergeInfo);
#pragma warning restore CS8604 // Possible null reference argument.
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
                this.children[this.Count + 1] = null;
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
            }

            return deleted;
        }

        public override (Page<TKey, TValue>? newPage, TKey? newPivotKey) Insert(TKey key, TValue value)
        {
            var page = this.SelectSubtree(key);
            var (newSubTree, newSubTreePivotKey) = page.Insert(key, value);
            if (newSubTree is null)
            {
                return (null, default);
            }

#pragma warning disable CS8604 // Possible null reference argument. - stfu, it's not null
            if (!this.IsOverflow)
            {
                this.InsertInternal(newSubTreePivotKey, newSubTree);
                return (null, default);
            }

            var (newPage, newPivotKey) = this.Split();
            var pivotPage = key.CompareTo(newPivotKey) <= 0
                ? this
                : (PivotPage<TKey, TValue>)newPage;

            pivotPage.InsertInternal(newSubTreePivotKey, newSubTree);
#pragma warning restore CS8604 // Possible null reference argument.

            return (newPage, newPivotKey);
        }

        public override bool TryRead(TKey key, out TValue? value)
        {
            var page = this.SelectSubtree(key);
            return page.TryRead(key, out value);
        }
    }
}
