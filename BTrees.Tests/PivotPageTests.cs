using BTrees.Pages;

namespace BTrees.Tests
{
    public class PivotPageTests
    {
        private readonly int pageSize = 10;

        [Fact]
        public void NewPageIsEmpty()
        {
            using var page = new PivotPage<int, int>(this.pageSize);
            Assert.True(page.IsEmpty);
        }

        [Fact]
        public void NewPageHasCorrectPageSize()
        {
            using var page = new PivotPage<int, int>(this.pageSize);
            Assert.Equal(this.pageSize, page.Size);
        }

        [Fact]
        public void InsertIncrememtsLeafNodeCount()
        {
            using var leftpage = new LeafPage<int, int>(this.pageSize);
            using var rightpage = new LeafPage<int, int>(this.pageSize);

            for (var i = 0; i < this.pageSize / 2; ++i)
            {
                Assert.True(leftpage.TryWrite(i, i, out _));
            }

            for (var i = this.pageSize + 1; i < this.pageSize + this.pageSize / 2; ++i)
            {
                Assert.True(rightpage.TryWrite(i, i, out _));
            }

            using var pivotPage = new PivotPage<int, int>(this.pageSize, leftpage, rightpage, rightpage.MinKey);
            Assert.Equal(1, pivotPage.Count);

            var expectedLeftCount = leftpage.Count + 1;
            var expectedRightCount = rightpage.Count;
            Assert.True(pivotPage.TryWrite(this.pageSize, this.pageSize, out _));

            Assert.Equal(expectedLeftCount, leftpage.Count);
            Assert.Equal(expectedRightCount, rightpage.Count);
        }

        [Fact]
        public void PagesSplitOnInsertsCorrectly()
        {
            var pageSize = 4;
            using var leftLeafPage = new LeafPage<int, int>(pageSize);
            var index = 0;
            for (var i = 0; i < pageSize; ++i)
            {
                Assert.True(leftLeafPage.TryWrite(index, index, out var response));
                ++index;
                Assert.Null(response.newPage);
            }

            Assert.True(leftLeafPage.TryWrite(index, index, out var rightLeafResponse));
            ++index;
            Assert.NotNull(rightLeafResponse.newPage);

            using var leftPivotPage = new PivotPage<int, int>(pageSize, leftLeafPage, rightLeafResponse.newPage, rightLeafResponse.newPivotKey);
            for (var i = 0; i < 15; ++i)
            {
                Assert.True(leftPivotPage.TryWrite(index, index, out var response));
                ++index;
                Assert.Null(response.newPage);
            }

            Assert.True(leftPivotPage.TryWrite(index, index, out var rightPivotResponse));
            Assert.NotNull(rightPivotResponse.newPage);
            Assert.Equal(2, leftPivotPage.Count);
            Assert.Equal(2, rightPivotResponse.newPage.Count);
            Assert.Equal(12, rightPivotResponse.newPivotKey);
            Assert.Equal(16, rightPivotResponse.newPage.MinKey);
        }
    }
}
