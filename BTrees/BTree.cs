using BTrees.Expressions;
using BTrees.Pages;
using System.Diagnostics;

namespace BTrees
{
    [DebuggerDisplay("{Count}")]
    internal class BTree<TKey, TValue>
        : IBTree<TKey, TValue>
        where TKey : IComparable<TKey>
    {
        public long Count { get; private set; }

        public int Degree { get; private set; } = 1;

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
                        this.root = rootPage.children[0];
                        --this.Degree;
                    }
                }
            }

            return deleted;
        }

        public void Insert(TKey key, TValue value)
        {
            var (newSubPage, newPivotKey) = this.root.Insert(key, value);
            if (newSubPage is not null)
            {
#pragma warning disable CS8604 // Possible null reference argument. - it's not null
                this.root = new PivotPage<TKey, TValue>(
                    this.pageSize,
                    this.root,
                    newSubPage,
                    newPivotKey);
#pragma warning restore CS8604 // Possible null reference argument.

                ++this.Degree;
            }

            ++this.Count;
        }

        public bool TryRead(TKey key, out TValue? value)
        {
            return this.root.TryRead(key, out value);
        }

        // todo: add unit tests
        public IEnumerable<TValue> Read(Expression<TKey> expression)
        {
            // todo: this can use the leaf page's right sibling
            throw new NotImplementedException();
        }
    }
}
