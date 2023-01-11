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
            var maxKey = this.pageSize * 2;
            var leftPage = new LeafPage<int, int>(this.pageSize);
            for (var i = 0; i < maxKey; i += 2)
            {
                var (newPage, _, writeResultI) = leftPage.Write(i, i);
                Assert.Equal(WriteResult.Inserted, writeResultI);
                Assert.Null(newPage);
            }

            var (rightPage, pivot, writeResultO) = leftPage.Write(maxKey / 2 - 1, maxKey / 2 - 1);
            Assert.Equal(WriteResult.Inserted, writeResultO);
            Assert.NotNull(rightPage);
            Assert.Equal(10, pivot);
            Assert.Equal(6, leftPage.Count);
            Assert.Equal(5, rightPage.Count);

            Assert.Equal(8, leftPage.Keys[4]);
            Assert.Equal(9, leftPage.Keys[5]);

            Assert.Equal(pivot, rightPage.Keys[0]);
        }

        [Fact]
        public void SplitSetsNewPagePivotKey()
        {
            var leftPage = new LeafPage<int, int>(this.pageSize);
            for (var i = 0; i < this.pageSize * 2; i += 2)
            {
                var (newPage, _, _) = leftPage.Write(i, i);
                Assert.Null(newPage);
            }

            var key = 9;
            var (rightPage, pivot, _) = leftPage.Write(key, key);
            Assert.NotNull(rightPage);
            Assert.Equal(10, pivot);
            Assert.Equal(6, leftPage.Count);
            Assert.Equal(5, rightPage.Count);

            Assert.Equal(rightPage.MinKey, rightPage.PivotKey);
        }

        [Fact]
        public void SplitSetsNewPagePivotKeyWithRightOnlyInsert()
        {
            var leftPage = new LeafPage<int, int>(this.pageSize);
            for (var i = 0; i < leftPage.Size; ++i)
            {
                var (newPage, _, _) = leftPage.Write(i, i);
                Assert.Null(newPage);
            }

            var (rightPage, pivot, _) = leftPage.Write(this.pageSize, this.pageSize);
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
                _ = page.Write(i, i);
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
                _ = page.Write(i, i);
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
                _ = page.Write(i, i);
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
                _ = page.Write(i, i);
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
                _ = page.Write(i, i);
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
                _ = leftSibling.Write(i, i);
            }

            var page = new LeafPage<int, int>(this.pageSize, leftSibling);
            for (var i = 4; i < 4 + 3; ++i)
            {
                _ = page.Write(i, i);
            }

            var rightSibling = new LeafPage<int, int>(this.pageSize, page);
            for (var i = 8; i < 8 + 3; ++i)
            {
                _ = rightSibling.Write(i, i);
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
                _ = leftSibling.Write(i, i);
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
            var leftPage = new LeafPage<int, int>(this.pageSize);
            var count = this.pageSize * 10;
            for (var i = 0; i < count; i += 10)
            {
                _ = leftPage.Write(i, i);
            }

            var (middlePage, middlePivotKey) = leftPage.Split();
            Assert.Equal(50, middlePivotKey);
            var (rightPage, rightPivotKey) = middlePage.Split();
            Assert.Equal(70, rightPivotKey);

            for (var i = 61; i < 69; ++i)
            {
                var (newPage, _, result) = middlePage.Write(i, i);
                Assert.Null(newPage);
                Assert.Equal(WriteResult.Inserted, result);
            }

            var deleted = rightPage.TryDelete(rightPage.PivotKey, out var mergeInfo1);
            Assert.True(deleted);
            Assert.False(mergeInfo1.merged);

            var deleteUntilIndex = middlePage.Count / 2;
            for (var i = middlePage.Count - 1; i >= deleteUntilIndex; i--)
            {
                Assert.True(middlePage.TryDelete(middlePage.Keys[i], out _));
            }

            _ = middlePage.TryDelete(middlePage.PivotKey, out var mergeInfo);
            Assert.True(mergeInfo.merged, "merged?");
            Assert.Equal(rightPage.PivotKey, mergeInfo.deprecatedPivotKey);
        }

        [Fact]
        public void TryDeleteDoesntMergeWhenNoMergeCandidate()
        {
            // 1. fill a page
            var leftSibling = new LeafPage<int, int>(this.pageSize);
            for (var i = 0; i < this.pageSize; ++i)
            {
                _ = leftSibling.Write(i, i);
            }

            // 2. split it and then split the result so middle and right pages are both in underflow state
            var (middlePage, _) = leftSibling.Split();
            var (rightSibling, _) = middlePage.Split();

            // 3. fill the middle page so it is no longer in underflow and is unmergable
            var insertCount = middlePage.Size - middlePage.Count;
            var max = middlePage.MaxKey;
            for (var i = max + 1; i < max + 1 + insertCount; ++i)
            {
                var (newPage, _, result) = middlePage.Write(i, i);
                Assert.Null(newPage);
                Assert.Equal(WriteResult.Inserted, result);
            }

            // 4. delete an item from the right sibling in an unmergable condition
            _ = rightSibling.TryDelete(rightSibling.PivotKey, out var rightSiblingMergeInfo);
            Assert.False(rightSiblingMergeInfo.merged, "right sibling should not merge");
        }

        [Fact]
        public void TryDeleteMergeInfoReturnsRightPivotKeyWhenLeftMostPage()
        {
            var leftPage = new LeafPage<int, int>(this.pageSize);
            for (var i = 0; i < this.pageSize; ++i)
            {
                _ = leftPage.Write(i, i);
            }

            var (rightPage, _) = leftPage.Split();

            _ = leftPage.TryDelete(leftPage.MinKey, out var mergeInfo);
            Assert.True(mergeInfo.merged, "merged shoud be true");
            Assert.True(rightPage.PivotKey == mergeInfo.deprecatedPivotKey, "deprecated pivot key comes from right page");
        }
    }
}
