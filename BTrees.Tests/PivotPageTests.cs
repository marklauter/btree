using BTrees.Pages;

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
                _ = leftpage.Insert(i, i + 1);
                _ = rightpage.Insert(i + 5, i + 6);
            }

            var originalCount = leftpage.Count;
            var pivotPage = new PivotPage<int, int>(this.pageSize, leftpage, rightpage, rightpage.MinKey);
            Assert.Equal(1, pivotPage.Count);
            _ = pivotPage.Insert(4, 5);

            Assert.Equal(originalCount, leftpage.Count - 1);
            Assert.Equal(originalCount, rightpage.Count);
        }

        [Fact(Skip = "not using right-only at the moment")]
        public void PagesSplitOnInsertsCorrectly()
        {
            var pageSize = 4;
            var leftLeafPage = new LeafPage<int, int>(pageSize);
            var index = 0;
            for (var i = 0; i < pageSize; ++i)
            {
                var (newPage, _) = leftLeafPage.Insert(index, index);
                ++index;
                Assert.Null(newPage);
            }

            var (rightLeafPage, newLeafPivotKey) = leftLeafPage.Insert(index, index);
            ++index;
            Assert.NotNull(rightLeafPage);

            var leftPivotPage = new PivotPage<int, int>(pageSize, leftLeafPage, rightLeafPage, newLeafPivotKey);
            for (var i = 0; i < 15; ++i)
            {
                var (newPage, _) = leftPivotPage.Insert(index, index);
                ++index;
                Assert.Null(newPage);
            }

            var (rightPivotPage, newPivotKey) = leftPivotPage.Insert(index, index);
            Assert.NotNull(rightPivotPage);
            Assert.Equal(2, leftPivotPage.Count);
            Assert.Equal(2, rightPivotPage.Count);
            Assert.Equal(newPivotKey, rightPivotPage.MinKey);
        }
    }
}
