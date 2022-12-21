using System.Diagnostics;

namespace BTrees
{
    [DebuggerDisplay("{Count}")]
    internal class BTree<TKey, TValue>
        : IBTree<TKey, TValue>
        where TKey : IComparable<TKey>
    {
        public long Count { get; private set; }

        public int Degree { get; private set; }

        private readonly int pageSize;
        private Page<TKey, TValue> root;

        public BTree(int pageSize)
        {
            this.pageSize = pageSize;
            this.root = new LeafPage<TKey, TValue>(pageSize);
        }

        public void Delete(TKey key)
        {
            _ = this.root.Delete(key);
            // todo: merge the subtree up to root if the root is underflow and not a leaf
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
    }
}
