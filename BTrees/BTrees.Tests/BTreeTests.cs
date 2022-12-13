namespace BTrees.Tests
{
    public class BTreeTests
    {
        private readonly int pageSize = 5;

        [Fact]
        public void SplitTest()
        {
            var tree = new BTree<int, int>(this.pageSize);
            for (var i = 0; i < this.pageSize * 5; ++i)
            {
                tree.Insert(i + 1, i + 1);
            }
        }
    }
}
