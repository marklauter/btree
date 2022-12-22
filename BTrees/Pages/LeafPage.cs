using System.Diagnostics;

namespace BTrees.Pages
{
    // todo: merge leaves when underflow condition arrises. ie: count < k
    [DebuggerDisplay("LeafPage {Count}")]
    internal class LeafPage<TKey, TValue>
        : Page<TKey, TValue>
        where TKey : IComparable<TKey>
    {
        private readonly TValue[] children;

        #region CTOR
        public LeafPage(int size)
            : base(size)
        {
            this.children = new TValue[size];
        }

        private LeafPage(
            int size,
            Page<TKey, TValue> leftSibling)
            : base(
                  size,
                  leftSibling)
        {
            this.children = new TValue[size];
        }
        #endregion

        private void InsertInternal(TKey key, TValue value)
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
            this.children[index] = value;
            ++this.Count;
        }

        private void ShiftLeft(int index)
        {
            for (var i = index; i < this.Count - 1; ++i)
            {
                this.Keys[i] = this.Keys[i + 1];
                this.children[i] = this.children[i + 1];
            }
        }

        private void ShiftRight(int index)
        {
            for (var i = this.Count - 1; i >= index; --i)
            {
                this.Keys[i + 1] = this.Keys[i];
                this.children[i + 1] = this.children[i];
            }
        }

        internal override void Merge(Page<TKey, TValue> sourcePage)
        {
            var startIndex = this.Count;
            var endIndex = sourcePage.Count + startIndex;
            var keys = new Span<TKey>(this.Keys);
            var children = new Span<TValue>(this.children);
            var sourceKeys = new Span<TKey>(this.Keys);
            var sourceChildren = new Span<TValue>(this.children);

            var j = 0;
            for (var i = startIndex; i < endIndex; ++i)
            {
                keys[i] = sourceKeys[j];
                children[i] = sourceChildren[j];
                ++j;
            }

            this.Count = endIndex;
        }

        internal override Page<TKey, TValue> SelectSubtree(TKey key)
        {
            // it's a leaf, so there is no subtree - this is the end of the traversal
            return this;
        }

        internal override (Page<TKey, TValue> newPage, TKey newPivotKey) Split()
        {
            var count = this.Count;
            var keys = new Span<TKey>(this.Keys);
            var children = new Span<TValue>(this.children);
            var newPage = new LeafPage<TKey, TValue>(this.Size, this);
            this.RightSibling = newPage;
            var newKeys = new Span<TKey>(newPage.Keys);
            var newChildren = new Span<TValue>(newPage.children);

            var newPivotIndex = count / 2;
            var j = 0;
            for (var i = newPivotIndex; i < count; ++i)
            {
                newKeys[j] = keys[i];
                newChildren[j] = children[i];
                ++j;
            }

            newPage.Count = count - newPivotIndex;
            this.Count = newPivotIndex;

            return (newPage, newKeys[0]);
        }

        public override bool TryDelete(TKey key, out (bool merged, TKey? deprecatedPivotKey) mergeInfo)
        {
            mergeInfo.merged = false;
            mergeInfo.deprecatedPivotKey = default;

            var index = this.IndexOfKey(key);
            if (index < 0)
            {
                return false;
            }

            this.ShiftLeft(index);
            --this.Count;

            if (this.IsUnderFlow)
            {
                var leftSiblingCount = this.LeftSibling is null
                    ? this.Size
                    : this.LeftSibling.Count;

                var rightSiblingCount = this.RightSibling is null
                    ? this.Size
                    : this.RightSibling.Count;

                var mergeCandidate = leftSiblingCount < rightSiblingCount
                    ? this.LeftSibling
                    : this.RightSibling;

                if (this.CanMerge(mergeCandidate))
                {
                    mergeInfo.deprecatedPivotKey = this.Keys[0];
                    mergeInfo.merged = true;

#pragma warning disable CS8602 // Dereference of a possibly null reference. - CanMerge would return false if mergeCandiate is was null
                    mergeCandidate.Merge(this);
#pragma warning restore CS8602 // Dereference of a possibly null reference.
                }
            }

            return true;
        }

        public override (Page<TKey, TValue>? newPage, TKey? newPivotKey) Insert(TKey key, TValue value)
        {
            if (!this.IsOverflow)
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
                ((LeafPage<TKey, TValue>)newPage).InsertInternal(key, value);
            }

            return (newPage, newPivotKey);
        }

        public override bool TryRead(TKey key, out TValue? value)
        {
            value = default;
            var index = this.IndexOfKey(key);
            if (index < 0)
            {
                return false;
            }

            value = this.children[index];
            return true;
        }
    }
}
