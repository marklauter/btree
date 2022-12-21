namespace BTrees.Tests
{
    public class LeafPageTests
    {
        private readonly int pageSize = 10;

        [Fact]
        public void NewPageIsEmpty()
        {
            var page = new LeafPage<int, int>(this.pageSize);
            Assert.True(page.IsEmpty);
        }

        [Fact]
        public void NewPageHasCorrectPageSize()
        {
            var page = new LeafPage<int, int>(this.pageSize);
            Assert.Equal(this.pageSize, page.Size);
        }

        [Fact]
        public void InsertIncrememtsCount()
        {
            var page = new LeafPage<int, int>(this.pageSize);
            _ = page.Insert(1, 1);
            Assert.Equal(1, page.Count);
        }

        [Fact]
        public void SelectSubtreeReturnsSelf()
        {
            var page = new LeafPage<int, int>(this.pageSize);
            var subtree = page.SelectSubtree(1);
            Assert.Equal(page, subtree);
        }

        [Fact]
        public void DivisionTest()
        {
            var i = this.pageSize / 2;
            Assert.Equal(5, i);
            i /= 2;
            Assert.Equal(2, i);

            i /= 2;
            Assert.Equal(1, i);

            i /= 2;
            Assert.Equal(0, i);

            var j = this.pageSize / 2;
            var x = 0;
            while (j > 0)
            {
                j /= 2;
                ++x;
            }

            Assert.Equal(3, x);
        }

        [Fact]
        public void IndexOfKeyTest()
        {
            var page = new LeafPage<int, int>(this.pageSize);
            _ = page.Insert(1, 1); // 0 goes to index 0
            _ = page.Insert(2, 2); // 3 goes to index 2
            _ = page.Insert(4, 4); // 4 goes to index 3
            _ = page.Insert(5, 5);
            _ = page.Insert(6, 6);
            _ = page.Insert(7, 7); // 8 goes to index 6
            _ = page.Insert(9, 9);
            _ = page.Insert(10, 10); // 11 goes to index 8

            var index = page.IndexOfKey(3);
            Assert.Equal(2, ~index);

            index = page.IndexOfKey(4);
            Assert.Equal(2, index);

            index = page.IndexOfKey(8);
            Assert.Equal(6, ~index);

            index = page.IndexOfKey(0);
            Assert.Equal(0, ~index);

            index = page.IndexOfKey(10);
            Assert.Equal(7, index);

            index = page.IndexOfKey(11);
            Assert.Equal(8, ~index);
        }

        [Fact]
        public void SortedInsertTest()
        {
            var page = new LeafPage<int, int>(this.pageSize);
            _ = page.Insert(1, 1); // 0 goes to index 0
            _ = page.Insert(2, 2); // 3 goes to index 2
            _ = page.Insert(4, 4);
            _ = page.Insert(5, 5);
            _ = page.Insert(6, 6);
            _ = page.Insert(7, 7); // 8 goes to index 6
            _ = page.Insert(9, 9);
            _ = page.Insert(10, 10); // 11 goes to index 8

            _ = page.Insert(3, 3);
            var sortedKeys = page.Keys
                .Take(page.Count)
                .Order();
            Assert.Equal(sortedKeys, page.Keys.Take(page.Count));

            _ = page.Insert(8, 8);
            sortedKeys = page.Keys
                .Take(page.Count)
                .Order();
            Assert.Equal(sortedKeys, page.Keys.Take(page.Count));
        }

        [Fact]
        public void SplitTest()
        {
            var leftPage = new LeafPage<int, int>(this.pageSize);
            for (var i = 0; i < leftPage.Size; ++i)
            {
                var (newPage, _) = leftPage.Insert(i, i);
                Assert.Null(newPage);
            }

            var (rightPage, pivot) = leftPage.Insert(this.pageSize, this.pageSize);
            Assert.NotNull(rightPage);
            Assert.Equal(5, pivot);
            Assert.Equal(5, leftPage.Count);
            Assert.Equal(6, rightPage.Count);

            for (var i = 0; i < leftPage.Count; ++i)
            {
                Assert.Equal(i, leftPage.Keys[i]);
            }

            for (var i = 0; i < rightPage.Count; ++i)
            {
                Assert.Equal(i + pivot, rightPage.Keys[i]);
            }
        }
    }
}
