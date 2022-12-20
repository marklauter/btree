using System.Diagnostics;

namespace BTrees
{
    [DebuggerDisplay("{Count}")]
    internal class BTree<TKey, TValue>
        : IBTree<TKey, TValue>
        where TKey : IComparable<TKey>
    {
        public long Count { get; private set; }
        public int Depth { get; private set; }

        private readonly int pageSize;
        private Page<TKey, TValue> root;

        public BTree(int pageSize)
        {
            this.pageSize = pageSize;
            this.root = new LeafPage<TKey, TValue>(pageSize);
        }

        public void Write(TKey key, TValue value)
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
                ++this.Depth;
            }

            ++this.Count;
        }

        public bool TryRead(TKey key, out TValue? value)
        {
            return this.root.TryRead(key, out value);
        }
    }
}
