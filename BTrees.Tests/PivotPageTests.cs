namespace BTrees.Tests
{
    public class PivotPageTests
    {
        private readonly int pageSize = 10;

        [Fact]
        public void NewPageIsEmpty()
        {
            var page = new PivotPage<int, int>(this.pageSize);
            Assert.True(page.IsEmpty);
        }

        [Fact]
        public void NewPageHasCorrectPageSize()
        {
            var page = new PivotPage<int, int>(this.pageSize);
            Assert.Equal(this.pageSize, page.Size);
        }

        [Fact]
        public void InsertIncrememtsLeafNodeCount()
        {
            var leftpage = new LeafPage<int, int>(this.pageSize);
            var rightpage = new LeafPage<int, int>(this.pageSize);
            for (var i = 0; i < this.pageSize / 2; ++i)
            {
                _ = leftpage.Write(i, i + 1);
                _ = rightpage.Write(i + 5, i + 6);
            }

            var originalCount = leftpage.Count;
            var pivotPage = new PivotPage<int, int>(this.pageSize, leftpage, rightpage);
            Assert.Equal(1, pivotPage.Count);
            _ = pivotPage.Write(4, 5);

            Assert.Equal(originalCount, leftpage.Count - 1);
            Assert.Equal(originalCount, rightpage.Count);
        }
    }
}
