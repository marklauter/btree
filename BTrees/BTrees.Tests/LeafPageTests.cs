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
        public void FindInsertionIndexTest()
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

            var mid = page.FindInsertionIndex(3);
            Assert.Equal(2, mid);

            mid = page.FindInsertionIndex(4);
            Assert.Equal(3, mid);

            mid = page.FindInsertionIndex(8);
            Assert.Equal(6, mid);

            mid = page.FindInsertionIndex(0);
            Assert.Equal(0, mid);

            mid = page.FindInsertionIndex(11);
            Assert.Equal(8, mid);
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
            var page = new LeafPage<int, int>(this.pageSize);
            for (var i = 0; i < page.Size; i++)
            {
                _ = page.Insert(i + 1, i + 1);
            }

            var (newPage, pivot) = page.Insert(4, 4);
            Assert.NotNull(newPage);
            Assert.Equal(6, pivot);
            Assert.Equal(6, page.Count);
            Assert.Equal(5, newPage.Count);
        }
    }
}
