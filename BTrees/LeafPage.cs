using System.Diagnostics;

namespace BTrees
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

        internal override TKey Merge()
        {
            // todo: redistribute or merge?
            /*
In a B-tree, if a page has an underflow (i.e., it has fewer keys than the minimum required), it is possible to borrow a key from one of its siblings to restore the balance of keys. This operation is called "redistribution." If a page's siblings do not have enough keys to borrow, it may be necessary to merge the underflowing page with one of its siblings.

When merging two pages, the keys and values from both pages are combined into a single page, and this new page becomes the child of the parent of the two original pages. The parent node may need to be modified to reflect the changes to its child nodes. If the parent node also has an underflow after the merge, the process can be repeated until the tree is balanced.

It is important to maintain the balance of keys in a B-tree, as this helps to ensure that the tree remains efficient for searches, insertions, and deletions. When a page has an underflow, it is necessary to take action to restore the balance of the tree. Redistributing keys or merging pages are both possible options for addressing an underflow.
             */
            throw new NotImplementedException();
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

        public override (bool wasMerged, TKey? deprecatedPivotKey) Delete(TKey key)
        {
            var index = this.IndexOfKey(key);
            if (index < 0)
            {
                throw new KeyNotFoundException($"{key}");
            }

            this.ShiftLeft(index);
            --this.Count;

            var isUnderFlow = this.IsUnderFlow;

            var deprecatedPivotKey = this.IsUnderFlow
                ? this.Merge()
                : default;

            return (isUnderFlow, deprecatedPivotKey);
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
