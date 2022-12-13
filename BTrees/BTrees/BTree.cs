namespace BTrees
{
    public class BTree<TKey, TValue>
        where TKey : IComparable<TKey>
    {
        private readonly int pageSize;
        private int itemCount;
        private Page<TKey, TValue> root;

        public BTree(int pageSize)
        {
            this.pageSize = pageSize;
            this.root = new LeafPage<TKey, TValue>(pageSize);
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
            }

            ++this.itemCount;
        }
    }
}
