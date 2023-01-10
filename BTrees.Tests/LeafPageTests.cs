using BTrees.Pages;

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

            var (rightPage, pivot) = leftPage.Insert(this.pageSize / 2, this.pageSize / 2);
            Assert.NotNull(rightPage);
            Assert.Equal(5, pivot);
            Assert.Equal(5, leftPage.Count);
            Assert.Equal(6, rightPage.Count);

            for (var i = 0; i < leftPage.Count; ++i)
            {
                Assert.Equal(i, leftPage.Keys[i]);
            }

            Assert.Equal(5, rightPage.Keys[0]);
            Assert.Equal(5, rightPage.Keys[1]);
            Assert.Equal(6, rightPage.Keys[2]);
            Assert.Equal(7, rightPage.Keys[3]);
            Assert.Equal(8, rightPage.Keys[4]);
            Assert.Equal(9, rightPage.Keys[5]);
        }

        [Fact]
        public void SplitSetsNewPagePivotKey()
        {
            var leftPage = new LeafPage<int, int>(this.pageSize);
            for (var i = 0; i < leftPage.Size; ++i)
            {
                var (newPage, _) = leftPage.Insert(i, i);
                Assert.Null(newPage);
            }

            var (rightPage, pivot) = leftPage.Insert(this.pageSize / 2, this.pageSize / 2);
            Assert.NotNull(rightPage);
            Assert.Equal(5, pivot);
            Assert.Equal(5, leftPage.Count);
            Assert.Equal(6, rightPage.Count);

            for (var i = 0; i < leftPage.Count; ++i)
            {
                Assert.Equal(i, leftPage.Keys[i]);
            }

            Assert.Equal(rightPage.MinKey, rightPage.PivotKey);
        }

        [Fact(Skip = "other problems to solve")]
        public void SplitSetsNewPagePivotKeyWithRightOnlyInsert()
        {
            var leftPage = new LeafPage<int, int>(this.pageSize);
            for (var i = 0; i < leftPage.Size; ++i)
            {
                var (newPage, _) = leftPage.Insert(i, i);
                Assert.Null(newPage);
            }

            var (rightPage, pivot) = leftPage.Insert(this.pageSize, this.pageSize);
            Assert.NotNull(rightPage);
            Assert.Equal(10, pivot);
            Assert.Equal(10, leftPage.Count);
            Assert.Equal(1, rightPage.Count);

            for (var i = 0; i < leftPage.Count; ++i)
            {
                Assert.Equal(i, leftPage.Keys[i]);
            }

            Assert.Equal(rightPage.MinKey, rightPage.PivotKey);
        }

        [Fact]
        public void TryDeleteReturnsFalseWhenKeyNotFound()
        {
            var page = new LeafPage<int, int>(this.pageSize);
            for (var i = 0; i < this.pageSize; ++i)
            {
                _ = page.Insert(i, i);
            }

            var deleted = page.TryDelete(this.pageSize, out _);
            Assert.False(deleted);
        }

        [Fact]
        public void TryDeleteReturnsTrueWhenKeyFound()
        {
            var page = new LeafPage<int, int>(this.pageSize);
            for (var i = 0; i < this.pageSize; ++i)
            {
                _ = page.Insert(i, i);
            }

            var deleted = page.TryDelete(this.pageSize / 2, out _);
            Assert.True(deleted);
        }

        [Fact]
        public void TryDeleteRemovesKeyAndChild()
        {
            var page = new LeafPage<int, int>(this.pageSize);
            for (var i = 0; i < this.pageSize; ++i)
            {
                _ = page.Insert(i, i);
            }

            _ = page.TryDelete(this.pageSize / 2, out _);
            Assert.DoesNotContain(this.pageSize / 2, page.Keys);
            Assert.DoesNotContain(this.pageSize / 2, page.values);
            Assert.Equal(page.Keys.Length, page.values.Length);
        }

        [Fact]
        public void TryDeleteUpdatesCount()
        {
            var page = new LeafPage<int, int>(this.pageSize);
            for (var i = 0; i < this.pageSize; ++i)
            {
                _ = page.Insert(i, i);
            }

            _ = page.TryDelete(this.pageSize / 2, out _);
            Assert.Equal(this.pageSize - 1, page.Count);
        }

        [Fact]
        public void TryDeleteDoesNotMergeWhenCountGTKDiv2()
        {
            var page = new LeafPage<int, int>(this.pageSize);
            for (var i = 0; i < this.pageSize; ++i)
            {
                _ = page.Insert(i, i);
            }

            _ = page.TryDelete(this.pageSize / 2, out var mergeInfo);
            Assert.False(mergeInfo.merged);
        }

        [Fact]
        public void TryDeleteMergesWhenCountLTKDiv2()
        {
            var leftSibling = new LeafPage<int, int>(this.pageSize);
            for (var i = 0; i < 3; ++i)
            {
                _ = leftSibling.Insert(i, i);
            }

            var page = new LeafPage<int, int>(this.pageSize, leftSibling);
            for (var i = 4; i < 4 + 3; ++i)
            {
                _ = page.Insert(i, i);
            }

            var rightSibling = new LeafPage<int, int>(this.pageSize, page);
            for (var i = 8; i < 8 + 3; ++i)
            {
                _ = rightSibling.Insert(i, i);
            }

            Assert.Equal(3, page.Count);

            var deleted = page.TryDelete(page.MaxKey, out var mergeInfo);
            Assert.True(deleted, "unexpected deleted value");
            Assert.True(mergeInfo.merged, "unexpected merged value");
        }

        [Fact]
        public void TryDeleteMergesLeftWhenCountLTKDiv2AndPreferredChoiceIsCurrentPage()
        {
            var leftSibling = new LeafPage<int, int>(this.pageSize);
            for (var i = 0; i < this.pageSize; ++i)
            {
                _ = leftSibling.Insert(i, i);
            }

            var (page, pivotKey) = leftSibling.Split();
            Assert.True(page.PivotKey == pivotKey, "page pivot doesn't match");

            var (rightSibling, rightSiblingPivotKey) = leftSibling.Split();
            Assert.True(rightSibling.PivotKey == rightSiblingPivotKey, "right sibling pivot doesn't match");

            _ = page.TryDelete(page.PivotKey, out var mergeInfo);
            Assert.Equal(page.PivotKey, mergeInfo.deprecatedPivotKey);
        }

        [Fact]
        public void TryDeleteMergesRightWhenCountLTKDiv2AndPreferredChoiceIsRightSibling()
        {
            var leftSibling = new LeafPage<int, int>(this.pageSize);
            for (var i = 0; i < this.pageSize; ++i)
            {
                _ = leftSibling.Insert(i, i);
            }

            var (page, pivotKey) = leftSibling.Split();
            var (rightSibling, _) = page.Split();

            var insertCount = page.Size - page.Count;
            for (var i = 0; i < insertCount; ++i)
            {
                _ = page.Insert(pivotKey + 1, pivotKey + 1);
            }

            _ = rightSibling.TryDelete(rightSibling.PivotKey, out _);

            var deleteCount = page.Count / 2;
            for (var i = 0; i < deleteCount; ++i)
            {
                _ = page.TryDelete(pivotKey + 1, out _);
            }

            _ = page.TryDelete(page.PivotKey, out var mergeInfo);
            Assert.True(mergeInfo.merged, "merged?");
            Assert.Equal(rightSibling.PivotKey, mergeInfo.deprecatedPivotKey);
        }

        [Fact]
        public void TryDeleteDoesntMergeWhenNoMergeCandidate()
        {
            var leftSibling = new LeafPage<int, int>(this.pageSize);
            for (var i = 0; i < this.pageSize; ++i)
            {
                _ = leftSibling.Insert(i, i);
            }

            var (page, pivotKey) = leftSibling.Split();
            var (rightSibling, _) = page.Split();

            var insertCount = page.Size - page.Count;
            for (var i = 0; i < insertCount; ++i)
            {
                _ = page.Insert(pivotKey + 1, pivotKey + 1);
            }

            _ = rightSibling.TryDelete(rightSibling.PivotKey, out var rightSiblingMergeInfo);
            Assert.False(rightSiblingMergeInfo.merged, "right sibling should not merge");
        }

        [Fact]
        public void TryDeleteMergeInfoReturnsRightPivotKeyWhenLeftMostPage()
        {
            var leftPage = new LeafPage<int, int>(this.pageSize);
            for (var i = 0; i < this.pageSize; ++i)
            {
                _ = leftPage.Insert(i, i);
            }

            var (rightPage, _) = leftPage.Split();

            _ = leftPage.TryDelete(leftPage.MinKey, out var mergeInfo);
            Assert.True(mergeInfo.merged, "merged shoud be true");
            Assert.True(rightPage.PivotKey == mergeInfo.deprecatedPivotKey, "deprecated pivot key comes from right page");
        }
    }
}
