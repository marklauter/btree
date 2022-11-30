namespace BTrees.Tests
{
    public class BTree<TKey, TValue>
        where TKey : IComparable<TKey>
    {
        private readonly int pageSize;
        private int itemCount;
        private readonly Page<TKey, TValue> root;

        public BTree(int pageSize)
        {
            this.pageSize = pageSize;
            this.root = new LeafPage<TKey, TValue>(pageSize);
        }

        private void Insert(TKey key, TValue value)
        {
            var page = this.root.SelectSubtree(key);
            _ = page.Insert(key, value);
            ++this.itemCount;
        }
    }
}
