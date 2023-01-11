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
                _ = leftpage.Write(i, i);
            }

            for (var i = this.pageSize + 1; i < this.pageSize + this.pageSize / 2; ++i)
            {
                _ = rightpage.Write(i, i);
            }

            var pivotPage = new PivotPage<int, int>(this.pageSize, leftpage, rightpage, rightpage.MinKey);
            Assert.Equal(1, pivotPage.Count);

            var expectedLeftCount = leftpage.Count + 1;
            var expectedRightCount = rightpage.Count;
            _ = pivotPage.Write(this.pageSize, this.pageSize);

            Assert.Equal(expectedLeftCount, leftpage.Count);
            Assert.Equal(expectedRightCount, rightpage.Count);
        }

        [Fact]
        public void PagesSplitOnInsertsCorrectly()
        {
            var pageSize = 4;
            var leftLeafPage = new LeafPage<int, int>(pageSize);
            var index = 0;
            for (var i = 0; i < pageSize; ++i)
            {
                var (newPage, _, _) = leftLeafPage.Write(index, index);
                ++index;
                Assert.Null(newPage);
            }

            var (rightLeafPage, newLeafPivotKey, _) = leftLeafPage.Write(index, index);
            ++index;
            Assert.NotNull(rightLeafPage);

            var leftPivotPage = new PivotPage<int, int>(pageSize, leftLeafPage, rightLeafPage, newLeafPivotKey);
            for (var i = 0; i < 15; ++i)
            {
                var (newPage, _, _) = leftPivotPage.Write(index, index);
                ++index;
                Assert.Null(newPage);
            }

            var (rightPivotPage, newPivotKey, _) = leftPivotPage.Write(index, index);
            Assert.NotNull(rightPivotPage);
            Assert.Equal(2, leftPivotPage.Count);
            Assert.Equal(2, rightPivotPage.Count);
            Assert.Equal(12, newPivotKey);
            Assert.Equal(16, rightPivotPage.MinKey);
        }
    }
}
