using BTrees.Pages;
using System.Collections.Immutable;

namespace BTrees.Tests
{
    public sealed class PivotPageTests
    {
        [Fact]
        public void New_Page_Has_Correct_Size()
        {
            var size = 10;
            var leafPage = LeafPage<int, int>.Empty(size);
            for (var i = 0; i < size; ++i)
            {
                leafPage = leafPage.Insert(i, i);
            }

            var splitResult = leafPage.Split();
            var pivotPage = new PivotPage<int, int>(size, splitResult.leftPage, splitResult.rightPage);
            Assert.Equal(size, pivotPage.Size);
        }

        [Fact]
        public void New_Page_Has_Correct_Count()
        {
            var size = 10;
            var leafPage = LeafPage<int, int>.Empty(size);
            for (var i = 0; i < size; ++i)
            {
                leafPage = leafPage.Insert(i, i);
            }

            var splitResult = leafPage.Split();
            var pivotPage = new PivotPage<int, int>(size, splitResult.leftPage, splitResult.rightPage);
            Assert.Equal(1, pivotPage.Count);
        }

        [Fact]
        public void Insert_Returns_New_Page()
        {
            var size = 10;
            var leafPage = LeafPage<int, int>.Empty(size);
            for (var i = 0; i < size; ++i)
            {
                leafPage = leafPage.Insert(i, i);
            }

            var splitResult = leafPage.Split();
            var pivotPage = new PivotPage<int, int>(size, splitResult.leftPage, splitResult.rightPage);

            var newPage = pivotPage.Insert(size + 1, size + 1);
            Assert.False(pivotPage == newPage);
        }

        [Fact]
        public void Insert_Not_Resulting_In_Subtree_Overflow_Returns_New_Page_With_Count_Elements()
        {
            var size = 10;
            var leafPage = LeafPage<int, int>.Empty(size);
            for (var i = 0; i < size; ++i)
            {
                leafPage = leafPage.Insert(i, i);
            }

            var splitResult = leafPage.Split();
            var pivotPage = new PivotPage<int, int>(size, splitResult.leftPage, splitResult.rightPage);

            var newPage = pivotPage.Insert(size + 1, size + 1);
            Assert.Equal(pivotPage.Count, newPage.Count);
        }

        [Fact]
        public void Insert_Resulting_In_Subtree_Overflow_Returns_New_Page_With_Count_Plus_One_Elements()
        {
            var size = 10;
            var leafPage = LeafPage<int, int>.Empty(size);
            for (var i = 0; i < size * 2; ++i)
            {
                leafPage = leafPage.Insert(i, i);
            }

            var splitResult = leafPage.Split();
            Assert.Equal(10, splitResult.leftPage.Count);
            Assert.Equal(10, splitResult.rightPage.Count);

            var pivotPage = new PivotPage<int, int>(size, splitResult.leftPage, splitResult.rightPage);

            var newPage = pivotPage.Insert(size * 2, size * 2);
            Assert.Equal(pivotPage.Count + 1, newPage.Count);
        }

        [Fact]
        public void Insert_Returns_New_Page_That_Contains_All_Inserted_Keys()
        {
            var size = 10;
            var leafPage = LeafPage<int, int>.Empty(size);
            for (var i = 0; i < size; ++i)
            {
                leafPage = leafPage.Insert(i, i);
            }

            var splitResult = leafPage.Split();
            var pivotPage = new PivotPage<int, int>(size, splitResult.leftPage, splitResult.rightPage);

            var newPage = pivotPage.Insert(size, size);

            for (var i = 0; i < size + 1; ++i)
            {
                Assert.True(newPage.ContainsKey(i));
            }
        }

        [Fact]
        public void Insert_Returns_New_Page_That_Contains_All_Inserted_Values()
        {
            var size = 10;
            var leafPage = LeafPage<int, int>.Empty(size);
            for (var i = 0; i < size; ++i)
            {
                leafPage = leafPage.Insert(i, i);
            }

            var splitResult = leafPage.Split();
            var pivotPage = new PivotPage<int, int>(size, splitResult.leftPage, splitResult.rightPage);

            var newPage = pivotPage.Insert(size, size);

            for (var i = 0; i < size + 1; ++i)
            {
                Assert.True(newPage.TryRead(i, out var value));
                Assert.Equal(i, value);
            }
        }

        [Fact]
        public void Fork_Creates_Shallow_Copy_Correct_Order()
        {
            var size = 10;
            var leafPage = LeafPage<int, int>.Empty(size);
            for (var i = 0; i < size; ++i)
            {
                leafPage = leafPage.Insert(i, i);
            }

            var splitResult = leafPage.Split();
            var pivotPage = new PivotPage<int, int>(size, splitResult.leftPage, splitResult.rightPage);

            var fork = pivotPage.Fork();
            Assert.False(pivotPage == fork);

            for (var i = 0; i < size; ++i)
            {
                Assert.True(fork.TryRead(i, out var value));
                Assert.Equal(i, value);
            }
        }

        [Fact]
        public void Delete_Returns_New_Page_With_Key_Removed()
        {
            var size = 10;
            var leafPage = LeafPage<int, int>.Empty(size);
            for (var i = 0; i < size * 2; ++i)
            {
                leafPage = leafPage.Insert(i, i);
            }

            var splitResult = leafPage.Split();
            var pivotPage = new PivotPage<int, int>(size, splitResult.leftPage, splitResult.rightPage);

            var deletedPage = pivotPage.Delete(5);
            Assert.False(deletedPage.ContainsKey(5));
        }

        [Fact]
        public void Split_Works()
        {
            var size = 10;
            var keys = new List<int>(size * size);
            var pages = new List<IPage<int, int>>(size * size);
            for (var i = 0; i < size + 1; ++i)
            {
                var key = i * size;
                var items = Enumerable.Range(key, size);
                keys.Add(key);
                pages.Add(new LeafPage<int, int>(size, items.ToImmutableArray(), items.ToImmutableArray()));
            }

            var pivotPage = new PivotPage<int, int>(
                size,
                keys.Skip(1).Take(keys.Count - 1).ToImmutableArray(),
                pages.ToImmutableArray());

            Assert.Equal(size, pivotPage.Count);

            var splitResult = pivotPage.Split();

            Assert.Equal(60, splitResult.pivotKey);

            Assert.Equal(size / 2, splitResult.leftPage.Count);
            Assert.Equal(10, splitResult.leftPage.MinKey);
            Assert.Equal(50, splitResult.leftPage.MaxKey);

            Assert.Equal(size / 2 - 1, splitResult.rightPage.Count);
            Assert.Equal(70, splitResult.rightPage.MinKey);
            Assert.Equal(100, splitResult.rightPage.MaxKey);
        }
    }


    //public class PivotPageTests
    //{
    //    private readonly int pageSize = 10;

    //    [Fact]
    //    public void NewPageIsEmpty()
    //    {
    //        using var page = new PivotPage<int, int>(this.pageSize);
    //        Assert.True(page.IsEmpty);
    //    }

    //    [Fact]
    //    public void NewPageHasCorrectPageSize()
    //    {
    //        using var page = new PivotPage<int, int>(this.pageSize);
    //        Assert.Equal(this.pageSize, page.Size);
    //    }

    //    [Fact]
    //    public void InsertIncrememtsLeafNodeCount()
    //    {
    //        using var leftpage = new LeafPage<int, int>(this.pageSize);
    //        using var rightpage = new LeafPage<int, int>(this.pageSize);

    //        for (var i = 0; i < this.pageSize / 2; ++i)
    //        {
    //            Assert.True(leftpage.TryWrite(i, i, out _));
    //        }

    //        for (var i = this.pageSize + 1; i < this.pageSize + this.pageSize / 2; ++i)
    //        {
    //            Assert.True(rightpage.TryWrite(i, i, out _));
    //        }

    //        using var pivotPage = new PivotPage<int, int>(this.pageSize, leftpage, rightpage, rightpage.MinKey);
    //        Assert.Equal(1, pivotPage.Count);

    //        var expectedLeftCount = leftpage.Count + 1;
    //        var expectedRightCount = rightpage.Count;
    //        Assert.True(pivotPage.TryWrite(this.pageSize, this.pageSize, out _));

    //        Assert.Equal(expectedLeftCount, leftpage.Count);
    //        Assert.Equal(expectedRightCount, rightpage.Count);
    //    }

    //    [Fact]
    //    public void PagesSplitOnInsertsCorrectly()
    //    {
    //        var pageSize = 4;
    //        using var leftLeafPage = new LeafPage<int, int>(pageSize);
    //        var index = 0;
    //        for (var i = 0; i < pageSize; ++i)
    //        {
    //            Assert.True(leftLeafPage.TryWrite(index, index, out var response));
    //            ++index;
    //            Assert.Null(response.newPage);
    //        }

    //        Assert.True(leftLeafPage.TryWrite(index, index, out var rightLeafResponse));
    //        ++index;
    //        Assert.NotNull(rightLeafResponse.newPage);

    //        using var leftPivotPage = new PivotPage<int, int>(pageSize, leftLeafPage, rightLeafResponse.newPage, rightLeafResponse.newPivotKey);
    //        for (var i = 0; i < 15; ++i)
    //        {
    //            Assert.True(leftPivotPage.TryWrite(index, index, out var response));
    //            ++index;
    //            Assert.Null(response.newPage);
    //        }

    //        Assert.True(leftPivotPage.TryWrite(index, index, out var rightPivotResponse));
    //        Assert.NotNull(rightPivotResponse.newPage);
    //        Assert.Equal(2, leftPivotPage.Count);
    //        Assert.Equal(2, rightPivotResponse.newPage.Count);
    //        Assert.Equal(12, rightPivotResponse.newPivotKey);
    //        Assert.Equal(16, rightPivotResponse.newPage.MinKey);
    //    }
    //}
}
