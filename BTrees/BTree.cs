using BTrees.Pages;
using BTrees.Ranges;
using System.Diagnostics;

namespace BTrees
{
    [DebuggerDisplay("{Count}")]
    internal class BTree<TKey, TValue>
        : IBTree<TKey, TValue>
        where TKey : IComparable<TKey>
    {
        public long Count { get; private set; }

        public int Height { get; private set; } = 1;

        private readonly int pageSize;
        private Page<TKey, TValue> root;

        public BTree(int pageSize)
        {
            this.pageSize = pageSize;
            this.root = new LeafPage<TKey, TValue>(pageSize);
        }

        public bool TryDelete(TKey key)
        {
            var deleted = this.root.TryDelete(key, out var mergeInfo);
            if (deleted)
            {
                --this.Count;
                if (this.root is PivotPage<TKey, TValue> rootPage)
                {
                    if (mergeInfo.merged)
                    {
#pragma warning disable CS8604 // Possible null reference argument.
                        _ = rootPage.RemoveKey(mergeInfo.deprecatedPivotKey, out _);
#pragma warning restore CS8604 // Possible null reference argument.
                    }

                    if (rootPage.IsEmpty)
                    {
                        this.root = rootPage.subtrees[0];
                        --this.Height;
                    }
                }
            }

            return deleted;
        }

        public void Write(TKey key, TValue value)
        {
            var (newSubPage, newPivotKey, writeResult) = this.root.Write(key, value);
            if (newSubPage is not null)
            {
#pragma warning disable CS8604 // Possible null reference argument.
                this.root = new PivotPage<TKey, TValue>(
                    this.pageSize,
                    this.root,
                    newSubPage,
                    newPivotKey);
#pragma warning restore CS8604 // Possible null reference argument.

                ++this.Height;
            }

            if (writeResult == WriteResult.Inserted)
            {
                ++this.Count;
            }
        }

        public bool TryRead(TKey key, out TValue? value)
        {
            return this.root.TryRead(key, out value);
        }

        // todo: add unit tests
        public IEnumerable<TValue> Read(OpenRange<TKey> range)
        {
            // todo: this can use the leaf page's right sibling
            throw new NotImplementedException();
        }

        // todo: add unit tests
        public IEnumerable<TValue> Read(ClosedRange<TKey> range)
        {
            // todo: this can use the leaf page's right sibling
            // todo: if we identify the left and right most pages then we can pre-load first, last and all pages between that are involved in the range scan
            throw new NotImplementedException();
        }

        public bool TryUpdate(TKey key, TValue value)
        {
            throw new NotImplementedException();
        }
    }
}
