using System.Diagnostics;

namespace BTrees.Pages
{
    [DebuggerDisplay("LeafPage {Count}")]
    internal class LeafPage<TKey, TValue>
        : Page<TKey, TValue>
        where TKey : IComparable<TKey>
    {
        internal readonly TValue[] values;

        public override int Order => this.Size;

        #region CTOR
        public LeafPage(int size)
            : base(size)
        {
            this.values = new TValue[size];
        }

        internal LeafPage(
            int size,
            Page<TKey, TValue> leftSibling)
            : base(
                  size,
                  leftSibling)
        {
            this.values = new TValue[size];
        }
        #endregion

        private WriteResult WriteInternal(TKey key, TValue value)
        {
            if (this.IsEmpty)
            {
                this.Keys[0] = key;
                this.values[0] = value;
                ++this.Count;

                return WriteResult.Inserted;
            }

            var index = this.IndexOfKey(key);
            var keyNotFound = index < 0;

            index = keyNotFound
                    ? ~index
                    : index;

            var writeResult = keyNotFound
                ? WriteResult.Inserted
                : WriteResult.Updated;

            if (keyNotFound && index != this.Count)
            {
                this.ShiftRight(index);
            }

            this.Keys[index] = key;
            this.values[index] = value;
            if (writeResult == WriteResult.Inserted)
            {
                ++this.Count;
            }

            return writeResult;
        }

        protected override void ShiftLeft(int index)
        {
            for (var i = index; i < this.Count - 1; ++i)
            {
                this.Keys[i] = this.Keys[i + 1];
                this.values[i] = this.values[i + 1];
            }
        }

        protected override void ShiftRight(int index)
        {
            for (var i = this.Count - 1; i >= index; --i)
            {
                this.Keys[i + 1] = this.Keys[i];
                this.values[i + 1] = this.values[i];
            }
        }

        internal override void Merge(Page<TKey, TValue> sourcePage)
        {
            var startIndex = this.Count;
            var endIndex = sourcePage.Count + startIndex;

            var keys = new Span<TKey>(this.Keys);
            var children = new Span<TValue>(this.values);

            var sourceKeys = new Span<TKey>(sourcePage.Keys);
            var sourceChildren = new Span<TValue>(((LeafPage<TKey, TValue>)sourcePage).values);

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
            var newPage = new LeafPage<TKey, TValue>(this.Size, this);
            var newKeys = new Span<TKey>(newPage.Keys);
            var newChildren = new Span<TValue?>(newPage.values);

            var keys = new Span<TKey>(this.Keys);
            var children = new Span<TValue?>(this.values);

            var count = this.Count;
            var newPivotIndex = count / 2;
            var j = 0;
            for (var i = newPivotIndex; i < count; ++i)
            {
                newKeys[j] = keys[i];
                newChildren[j] = children[i];
                ++j;
            }

            newPage.PivotKey = newKeys[0];
            newPage.Count = count - newPivotIndex;
            this.Count = newPivotIndex;

            return (newPage, newPage.PivotKey);
        }

        public override bool TryDelete(TKey key, out (bool merged, TKey? deprecatedPivotKey) mergeInfo)
        {
            return this.RemoveKey(key, out mergeInfo);
        }

        public override (Page<TKey, TValue>? newPage, TKey? newPivotKey, WriteResult result) Write(TKey key, TValue value)
        {
            if (!this.IsOverflow)
            {
                return (null, default, this.WriteInternal(key, value));
            }

            var rightOnly = key.CompareTo(this.MaxKey) > 0;
            var (newPage, newPivotKey) = rightOnly
                ? (new LeafPage<TKey, TValue>(this.Size, this) { PivotKey = key }, key)
                : this.Split();

            var destinationPage = rightOnly || key.CompareTo(newPivotKey) >= 0
                ? ((LeafPage<TKey, TValue>)newPage)
                : this;

            return (newPage, newPivotKey, destinationPage.WriteInternal(key, value));
        }

        public override bool TryRead(TKey key, out TValue? value)
        {
            value = default;
            var index = this.IndexOfKey(key);
            if (index < 0)
            {
                return false;
            }

            value = this.values[index];
            return true;
        }
    }
}
