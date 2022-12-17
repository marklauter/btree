﻿namespace BTrees.Tests
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
            _ = page.Write(1, 1);
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
            _ = page.Write(1, 1); // 0 goes to index 0
            _ = page.Write(2, 2); // 3 goes to index 2
            _ = page.Write(4, 4); // 4 goes to index 3
            _ = page.Write(5, 5);
            _ = page.Write(6, 6);
            _ = page.Write(7, 7); // 8 goes to index 6
            _ = page.Write(9, 9);
            _ = page.Write(10, 10); // 11 goes to index 8

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
            _ = page.Write(1, 1); // 0 goes to index 0
            _ = page.Write(2, 2); // 3 goes to index 2
            _ = page.Write(4, 4);
            _ = page.Write(5, 5);
            _ = page.Write(6, 6);
            _ = page.Write(7, 7); // 8 goes to index 6
            _ = page.Write(9, 9);
            _ = page.Write(10, 10); // 11 goes to index 8

            _ = page.Write(3, 3);
            var sortedKeys = page.Keys
                .Take(page.Count)
                .Order();
            Assert.Equal(sortedKeys, page.Keys.Take(page.Count));

            _ = page.Write(8, 8);
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
                _ = page.Write(i + 1, i + 1);
            }

            var (newPage, pivot) = page.Write(4, 4);
            Assert.NotNull(newPage);
            Assert.Equal(7, pivot);
            Assert.Equal(6, page.Count);
            Assert.Equal(5, newPage.Count);
        }
    }
}