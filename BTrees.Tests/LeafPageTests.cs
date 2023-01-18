using BTrees.Pages;

namespace BTrees.Tests
{
    public class LeafPageTests
    {
        private readonly int pageSize = 10;

        [Fact]
        public void NewPageIsEmpty()
        {
            using var page = new LeafPage<int, int>(this.pageSize);
            Assert.True(page.IsEmpty);
        }

        [Fact]
        public void NewPageHasCorrectPageSize()
        {
            using var page = new LeafPage<int, int>(this.pageSize);
            Assert.Equal(this.pageSize, page.Size);
        }

        [Fact]
        public void InsertIncrememtsCount()
        {
            using var page = new LeafPage<int, int>(this.pageSize);
            var writeSuccess = page.TryWrite(1, 1, out _);
            Assert.True(writeSuccess);
            Assert.Equal(1, page.Count);
        }

        [Fact]
        public void SelectSubtreeReturnsSelf()
        {
            using var page = new LeafPage<int, int>(this.pageSize);
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
            using var page = new LeafPage<int, int>(this.pageSize);
            Assert.True(page.TryWrite(1, 1, out _)); // 0 goes to index 0
            Assert.True(page.TryWrite(2, 2, out _)); // 3 goes to index 2
            Assert.True(page.TryWrite(4, 4, out _)); // 4 goes to index 3
            Assert.True(page.TryWrite(5, 5, out _));
            Assert.True(page.TryWrite(6, 6, out _));
            Assert.True(page.TryWrite(7, 7, out _)); // 8 goes to index 6
            Assert.True(page.TryWrite(9, 9, out _));
            Assert.True(page.TryWrite(10, 10, out _)); // 11 goes to index 8

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
            using var page = new LeafPage<int, int>(this.pageSize);
            Assert.True(page.TryWrite(1, 1, out _)); // 0 goes to index 0
            Assert.True(page.TryWrite(2, 2, out _)); // 3 goes to index 2
            Assert.True(page.TryWrite(4, 4, out _));
            Assert.True(page.TryWrite(5, 5, out _));
            Assert.True(page.TryWrite(6, 6, out _));
            Assert.True(page.TryWrite(7, 7, out _)); // 8 goes to index 6
            Assert.True(page.TryWrite(9, 9, out _));
            Assert.True(page.TryWrite(10, 10, out _)); // 11 goes to index 8

            Assert.True(page.TryWrite(3, 3, out _));
            var sortedKeys = page.Keys
                .Take(page.Count)
                .Order();
            Assert.Equal(sortedKeys, page.Keys.Take(page.Count));

            Assert.True(page.TryWrite(8, 8, out _));
            sortedKeys = page.Keys
                .Take(page.Count)
                .Order();
            Assert.Equal(sortedKeys, page.Keys.Take(page.Count));
        }

        [Fact]
        public void SplitTest()
        {
            var maxKey = this.pageSize * 2;
            using var leftPage = new LeafPage<int, int>(this.pageSize);
            for (var i = 0; i < maxKey; i += 2)
            {
                Assert.True(leftPage.TryWrite(i, i, out var responseI));
                Assert.Equal(WriteResult.Inserted, responseI.result);
                Assert.Null(responseI.newPage);
            }

            Assert.True(leftPage.TryWrite(maxKey / 2 - 1, maxKey / 2 - 1, out var responseO));
            Assert.Equal(WriteResult.Inserted, responseO.result);
            Assert.NotNull(responseO.newPage);
            Assert.Equal(10, responseO.newPivotKey);
            Assert.Equal(6, leftPage.Count);
            Assert.Equal(5, responseO.newPage.Count);

            Assert.Equal(8, leftPage.Keys[4]);
            Assert.Equal(9, leftPage.Keys[5]);

            Assert.Equal(responseO.newPivotKey, responseO.newPage.MinKey);
        }

        [Fact]
        public void SplitSetsNewPagePivotKey()
        {
            using var leftPage = new LeafPage<int, int>(this.pageSize);
            for (var i = 0; i < this.pageSize * 2; i += 2)
            {
                Assert.True(leftPage.TryWrite(i, i, out var response));
                Assert.Null(response.newPage);
            }

            var key = 9;
            Assert.True(leftPage.TryWrite(key, key, out var rightPageResponse));
            Assert.NotNull(rightPageResponse.newPage);
            Assert.Equal(10, rightPageResponse.newPivotKey);
            Assert.Equal(6, leftPage.Count);
            Assert.Equal(5, rightPageResponse.newPage.Count);

            Assert.Equal(rightPageResponse.newPage.MinKey, rightPageResponse.newPage.PivotKey);
        }

        [Fact]
        public void SplitSetsNewPagePivotKeyWithRightOnlyInsert()
        {
            using var leftPage = new LeafPage<int, int>(this.pageSize);
            for (var i = 0; i < leftPage.Size; ++i)
            {
                Assert.True(leftPage.TryWrite(i, i, out var response));
                Assert.Null(response.newPage);
            }

            Assert.True(leftPage.TryWrite(this.pageSize, this.pageSize, out var rightPageResponse));
            Assert.NotNull(rightPageResponse.newPage);
            Assert.Equal(10, rightPageResponse.newPivotKey);
            Assert.Equal(10, leftPage.Count);
            Assert.Equal(1, rightPageResponse.newPage.Count);

            for (var i = 0; i < leftPage.Count; ++i)
            {
                Assert.Equal(i, leftPage.Keys[i]);
            }

            Assert.Equal(rightPageResponse.newPage.MinKey, rightPageResponse.newPage.PivotKey);
        }

        [Fact]
        public void TryDeleteReturnsFalseWhenKeyNotFound()
        {
            using var page = new LeafPage<int, int>(this.pageSize);
            for (var i = 0; i < this.pageSize; ++i)
            {
                Assert.True(page.TryWrite(i, i, out _));
            }

            var deleted = page.TryDelete(this.pageSize, out _);
            Assert.False(deleted);
        }

        [Fact]
        public void TryDeleteReturnsTrueWhenKeyFound()
        {
            using var page = new LeafPage<int, int>(this.pageSize);
            for (var i = 0; i < this.pageSize; ++i)
            {
                Assert.True(page.TryWrite(i, i, out _));
            }

            var deleted = page.TryDelete(this.pageSize / 2, out _);
            Assert.True(deleted);
        }

        [Fact]
        public void TryDeleteRemovesKeyAndChild()
        {
            using var page = new LeafPage<int, int>(this.pageSize);
            for (var i = 0; i < this.pageSize; ++i)
            {
                Assert.True(page.TryWrite(i, i, out _));
            }

            _ = page.TryDelete(this.pageSize / 2, out _);
            Assert.DoesNotContain(this.pageSize / 2, page.Keys);
            Assert.DoesNotContain(this.pageSize / 2, page.values);
            Assert.Equal(page.Keys.Length, page.values.Length);
        }

        [Fact]
        public void TryDeleteUpdatesCount()
        {
            using var page = new LeafPage<int, int>(this.pageSize);
            for (var i = 0; i < this.pageSize; ++i)
            {
                Assert.True(page.TryWrite(i, i, out _));
            }

            _ = page.TryDelete(this.pageSize / 2, out _);
            Assert.Equal(this.pageSize - 1, page.Count);
        }

        [Fact]
        public void TryDeleteDoesNotMergeWhenCountGTKDiv2()
        {
            using var page = new LeafPage<int, int>(this.pageSize);
            for (var i = 0; i < this.pageSize; ++i)
            {
                Assert.True(page.TryWrite(i, i, out _));
            }

            _ = page.TryDelete(this.pageSize / 2, out var mergeInfo);
            Assert.False(mergeInfo.merged);
        }

        [Fact]
        public void TryDeleteMergesWhenCountLTKDiv2()
        {
            using var leftSibling = new LeafPage<int, int>(this.pageSize);
            for (var i = 0; i < 3; ++i)
            {
                Assert.True(leftSibling.TryWrite(i, i, out _));
            }

            using var page = new LeafPage<int, int>(this.pageSize, leftSibling);
            for (var i = 4; i < 4 + 3; ++i)
            {
                Assert.True(page.TryWrite(i, i, out _));
            }

            using var rightSibling = new LeafPage<int, int>(this.pageSize, page);
            for (var i = 8; i < 8 + 3; ++i)
            {
                Assert.True(rightSibling.TryWrite(i, i, out _));
            }

            Assert.Equal(3, page.Count);

            var deleted = page.TryDelete(page.MaxKey, out var mergeInfo);
            Assert.True(deleted, "unexpected deleted value");
            Assert.True(mergeInfo.merged, "unexpected merged value");
        }

        [Fact]
        public void TryDeleteMergesLeftWhenCountLTKDiv2AndPreferredChoiceIsCurrentPage()
        {
            using var leftSibling = new LeafPage<int, int>(this.pageSize);
            for (var i = 0; i < this.pageSize; ++i)
            {
                Assert.True(leftSibling.TryWrite(i, i, out _));
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
            using var leftPage = new LeafPage<int, int>(this.pageSize);
            var count = this.pageSize * 10;
            for (var i = 0; i < count; i += 10)
            {
                Assert.True(leftPage.TryWrite(i, i, out _));
            }

            var (middlePage, middlePivotKey) = leftPage.Split();
            Assert.Equal(50, middlePivotKey);
            var (rightPage, rightPivotKey) = middlePage.Split();
            Assert.Equal(70, rightPivotKey);

            for (var i = 61; i < 69; ++i)
            {
                Assert.True(middlePage.TryWrite(i, i, out var response));
                Assert.Null(response.newPage);
                Assert.Equal(WriteResult.Inserted, response.result);
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
            using var leftSibling = new LeafPage<int, int>(this.pageSize);
            for (var i = 0; i < this.pageSize; ++i)
            {
                Assert.True(leftSibling.TryWrite(i, i, out _));
            }

            // 2. split it and then split the result so middle and right pages are both in underflow state
            var (middlePage, _) = leftSibling.Split();
            var (rightSibling, _) = middlePage.Split();

            // 3. fill the middle page so it is no longer in underflow and is unmergable
            var insertCount = middlePage.Size - middlePage.Count;
            var max = middlePage.MaxKey;
            for (var i = max + 1; i < max + 1 + insertCount; ++i)
            {
                Assert.True(middlePage.TryWrite(i, i, out var response));
                Assert.Null(response.newPage);
                Assert.Equal(WriteResult.Inserted, response.result);
            }

            // 4. delete an item from the right sibling in an unmergable condition
            _ = rightSibling.TryDelete(rightSibling.PivotKey, out var rightSiblingMergeInfo);
            Assert.False(rightSiblingMergeInfo.merged, "right sibling should not merge");
        }

        [Fact]
        public void TryDeleteMergeInfoReturnsRightPivotKeyWhenLeftMostPage()
        {
            using var leftPage = new LeafPage<int, int>(this.pageSize);
            for (var i = 0; i < this.pageSize; ++i)
            {
                Assert.True(leftPage.TryWrite(i, i, out _));
            }

            var (rightPage, _) = leftPage.Split();

            _ = leftPage.TryDelete(leftPage.MinKey, out var mergeInfo);
            Assert.True(mergeInfo.merged, "merged shoud be true");
            Assert.True(rightPage.PivotKey == mergeInfo.deprecatedPivotKey, "deprecated pivot key comes from right page");
        }
    }
}
