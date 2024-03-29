﻿using System.Diagnostics;

namespace BTrees.Pages
{
    [DebuggerDisplay("PivotPage {Count}")]
    internal class PivotPage<TKey, TValue>
        : Page<TKey, TValue>
        where TKey : IComparable<TKey>
    {
        internal readonly Page<TKey, TValue>[] subtrees;

        public override int Order => this.Size + 1;

        #region CTOR
        public PivotPage(int size)
            : base(size)
        {
            this.subtrees = new Page<TKey, TValue>[size + 1];
        }

        public PivotPage(
            int size,
            Page<TKey, TValue> leftPage,
            Page<TKey, TValue> rightPage,
            TKey pivotKey)
            : this(size)
        {
            this.Keys[0] = pivotKey;
            this.subtrees[0] = leftPage;
            this.subtrees[1] = rightPage;
            this.Count = 1;
        }

        internal PivotPage(int size, Page<TKey, TValue> leftSibling)
            : base(size, leftSibling)
        {
            this.subtrees = new Page<TKey, TValue>[size + 1];
        }
        #endregion

        private void WriteInternal(TKey key, Page<TKey, TValue> value)
        {
            var index = this.IndexOfKey(key);
            var shiftRequired = index < 0;
            index = index > 0
                    ? index
                    : ~index;

            shiftRequired = index != this.Count && shiftRequired;
            if (shiftRequired)
            {
                this.ShiftRight(index);
            }

            this.Keys[index] = key;
            this.subtrees[index + 1] = value;
            ++this.Count;
        }

        protected override void ShiftLeft(int index)
        {
            for (var i = index; i < this.Count - 1; ++i)
            {
                this.Keys[i] = this.Keys[i + 1];
                this.subtrees[i + 1] = this.subtrees[i + 2];
            }
        }

        protected override void ShiftRight(int index)
        {
            for (var i = this.Count - 1; i >= index; --i)
            {
                this.Keys[i + 1] = this.Keys[i];
                this.subtrees[i + 2] = this.subtrees[i + 1];
            }
        }

        internal override void Merge(Page<TKey, TValue> sourcePage)
        {
            var startIndex = this.Count;
            var endIndex = sourcePage.Count + startIndex;
            var keys = new Span<TKey>(this.Keys);
            var children = new Span<Page<TKey, TValue>>(this.subtrees);
            var sourceKeys = new Span<TKey>(this.Keys);
            var sourceChildren = new Span<Page<TKey, TValue>>(this.subtrees);

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

            return this.subtrees[index];
        }

        internal override (Page<TKey, TValue> newPage, TKey newPivotKey) Split()
        {
            var count = this.Count;
            var newPivotIndex = count / 2;
            var copyFromIndex = newPivotIndex + 1;

            var newPageCount = count - copyFromIndex;
            var subtreesLength = newPageCount + 1;
            var keys = new Span<TKey>(this.Keys, copyFromIndex, newPageCount);
            var subtrees = new Span<Page<TKey, TValue>?>(this.subtrees, copyFromIndex, subtreesLength);

            var newPage = new PivotPage<TKey, TValue>(this.Size, this);
            var newKeys = new Span<TKey>(newPage.Keys, 0, newPageCount);
            var newSubtrees = new Span<Page<TKey, TValue>?>(newPage.subtrees, 0, newPageCount + 1);
            newPage.Count = newPageCount;
            newPage.PivotKey = this.Keys[newPivotIndex];

            for (var i = 0; i < newPageCount; ++i)
            {
                newKeys[i] = keys[i];
                newSubtrees[i] = subtrees[i];
            }

            newSubtrees[newPageCount] = subtrees[newPageCount];

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
                this.subtrees[this.Count + 1] = null;
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
            }

            return deleted;
        }

        public override (Page<TKey, TValue>? newPage, TKey? newPivotKey, WriteResult result) Write(TKey key, TValue value)
        {
            var page = this.SelectSubtree(key);
            var (newSubTree, newSubTreePivotKey, result) = page.Write(key, value);
            if (newSubTree is null)
            {
                return (null, default, result);
            }

            if (!this.IsOverflow)
            {
#pragma warning disable CS8604 // Possible null reference argument.
                this.WriteInternal(newSubTreePivotKey, newSubTree);
#pragma warning restore CS8604 // Possible null reference argument.
                return (null, default, result);
            }

            var (newPage, newPivotKey) = this.Split();

            var destinationPage = key.CompareTo(newPivotKey) >= 0
                ? (PivotPage<TKey, TValue>)newPage
                : this;

#pragma warning disable CS8604 // Possible null reference argument.
            destinationPage.WriteInternal(newSubTreePivotKey, newSubTree);
#pragma warning restore CS8604 // Possible null reference argument.

            return (newPage, newPivotKey, result);
        }

        public override bool TryRead(TKey key, out TValue? value)
        {
            var page = this.SelectSubtree(key);
            return page.TryRead(key, out value);
        }
    }
}
