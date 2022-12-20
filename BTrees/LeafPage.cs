﻿using System.Diagnostics;

namespace BTrees
{
    // todo: merge leaves when underflow condition arrises. ie: count < k
    [DebuggerDisplay("LeafPage {Count}")]
    internal class LeafPage<TKey, TValue>
        : Page<TKey, TValue>
        where TKey : IComparable<TKey>
    {
        private readonly TValue[] children;
        private readonly LeafPage<TKey, TValue>? leftSibling;
        private LeafPage<TKey, TValue>? rightSibling;

        public LeafPage(int size)
            : base(size)
        {
            this.children = new TValue[size];
        }

        private LeafPage(
            int size,
            LeafPage<TKey, TValue> leftSibling)
            : this(size)
        {
            this.leftSibling = leftSibling ?? throw new ArgumentNullException(nameof(leftSibling));
        }

        internal override Page<TKey, TValue> SelectSubtree(TKey key)
        {
            // it's a leaf, so there is no subtree - this is the end of the traversal
            return this;
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

        private void ShiftRight(int index)
        {
            for (var i = this.Count - 1; i >= index; --i)
            {
                this.Keys[i + 1] = this.Keys[i];
                this.children[i + 1] = this.children[i];
            }
        }

        public override (Page<TKey, TValue> newPage, TKey newPivotKey) Split()
        {
            var count = this.Count;
            var keys = new Span<TKey>(this.Keys);
            var children = new Span<TValue>(this.children);
            var newPage = new LeafPage<TKey, TValue>(this.Size, this);
            this.rightSibling = newPage;
            var newKeys = new Span<TKey>(newPage.Keys);
            var newChildren = new Span<TValue>(newPage.children);

            var newPivotIndex = count / 2;
            var copyFromIndex = newPivotIndex + 1;
            var j = 0;
            for (var i = copyFromIndex; i < count; ++i)
            {
                newKeys[j] = keys[i];
                newChildren[j] = children[i];
                ++j;
            }

            newPage.Count = count - newPivotIndex;
            this.Count = newPivotIndex;

            return (newPage, newKeys[0]);
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
