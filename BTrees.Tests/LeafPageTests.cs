using BTrees.Pages;

namespace BTrees.Tests
{
    public sealed class LeafPageTests
    {
        [Fact]
        public void EmptyPage_Has_CorrectSize()
        {
            var size = 10;
            var page = LeafPage<int, int>.Empty(size);
            Assert.Equal(size, page.Size);
        }

        [Fact]
        public void EmptyPage_Has_CorrectCount()
        {
            var size = 10;
            var page = LeafPage<int, int>.Empty(size);
            Assert.Equal(0, page.Count);
        }

        [Fact]
        public void EmptyPage_Has_IsEmpty()
        {
            var size = 10;
            var page = LeafPage<int, int>.Empty(size);
            Assert.True(page.IsEmpty);
        }

        [Fact]
        public void Insert_Returns_New_Page()
        {
            var size = 10;
            var page = LeafPage<int, int>.Empty(size);
            var newPage = page.Insert(1, 1);
            Assert.NotEqual(page, newPage);
        }

        [Fact]
        public void Insert_Returns_New_Page_With_Count_Plus_One_Elements()
        {
            var size = 10;
            var page = LeafPage<int, int>.Empty(size);
            var newPage = page.Insert(1, 1);
            Assert.Equal(page.Count + 1, newPage.Count);
        }

        [Fact]
        public void Insert_Returns_New_NonEmpty_Page()
        {
            var size = 10;
            var page = LeafPage<int, int>.Empty(size);
            var newPage = page.Insert(1, 1);
            Assert.False(newPage.IsEmpty);
        }

        [Fact]
        public void Insert_Returns_New_Page_That_Contains_Inserted_Key()
        {
            var size = 10;
            var page = LeafPage<int, string>.Empty(size);
            var newPage = page.Insert(1, "one");
            Assert.True(newPage.ContainsKey(1));
        }

        [Fact]
        public void Insert_Returns_New_Page_That_Contains_Inserted_Element()
        {
            var size = 10;
            var expectedValue = "one";
            var page = LeafPage<int, string>.Empty(size);
            var newPage = page.Insert(1, expectedValue);
            Assert.True(newPage.TryRead(1, out var actualValue));
            Assert.Equal(expectedValue, actualValue);
        }

        [Fact]
        public void Insert_Returns_New_Page_That_Contains_N_Inserted_Elements()
        {
            var size = 10;
            var page = LeafPage<int, int>.Empty(size);
            for (var i = 0; i < size; ++i)
            {
                page = page.Insert(i, i);
            }

            Assert.Equal(size, page.Count);
        }

        private static int[] UniqueRandoms(int seed, int count, int maxValue)
        {
            var rnd = new Random(seed);
            var rndSet = new HashSet<int>(count);

            while (rndSet.Count < count)
            {
                var value = rnd.Next(maxValue);
                while (!rndSet.Add(value))
                {
                    value = rnd.Next(maxValue);
                }
            }

            return rndSet.ToArray();
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(5)]
        [InlineData(7)]
        [InlineData(13)]
        public void Insert_Returns_New_Page_With_Elements_In_Correct_Order(int rndSeed)
        {
            var size = 10;
            var page = LeafPage<int, int>.Empty(size);
            var rndArray = UniqueRandoms(rndSeed, size, size);

            for (var i = 0; i < size; ++i)
            {
                page = page.Insert(rndArray[i], rndArray[i]);
            }

            for (var i = 0; i < size; ++i)
            {
                var index = page.BinarySearch(rndArray[i]);
                Assert.Equal(rndArray[i], index);
            }
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(5)]
        [InlineData(7)]
        [InlineData(13)]
        public void Insert_With_Multiple_Values_Returns_New_Page_Containing_Keys(int rndSeed)
        {
            var size = 10;
            var page = LeafPage<int, int>.Empty(size);
            var rndArray = UniqueRandoms(rndSeed, size, size * 10);

            for (var i = 0; i < size; ++i)
            {
                page = page.Insert(rndArray[i], rndArray[i]);
            }

            for (var i = 0; i < size; ++i)
            {
                Assert.True(page.ContainsKey(rndArray[i]));
            }
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(5)]
        [InlineData(7)]
        [InlineData(13)]
        public void Insert_Multiple_Values_Contains_Values(int rndSeed)
        {
            var size = 10;
            var page = LeafPage<int, int>.Empty(size);
            var rndArray = UniqueRandoms(rndSeed, size, size * 10);

            for (var i = 0; i < size; ++i)
            {
                page = page.Insert(rndArray[i], rndArray[i]);
            }

            for (var i = 0; i < size; ++i)
            {
                Assert.True(page.TryRead(rndArray[i], out var value));
                Assert.Equal(rndArray[i], value);
            }
        }

        [Fact]
        public void Insert_Duplicate_Key_Throws_InvalidOperationException()
        {
            var size = 10;
            var page = LeafPage<int, int>.Empty(size);
            page = page.Insert(1, 1);
            var ex = Assert.Throws<InvalidOperationException>(() => page.Insert(1, 1));
            Assert.Contains("1", ex.Message);
        }

        [Fact]
        public void Fork_Creates_Shallow_Copy_Correct_Order()
        {
            var size = 10;
            var expectedValue = new object();
            var page = LeafPage<int, object>.Empty(size);
            page = page.Insert(1, expectedValue);
            var fork = page.Fork();
            Assert.NotEqual(page, fork);
            Assert.True(fork.TryRead(1, out var actualValue));
            Assert.Equal(expectedValue, actualValue);
        }

        [Fact]
        public void Delete_Returns_New_Page_With_Key_Removed()
        {
            var size = 10;
            var page = LeafPage<int, int>.Empty(size);
            for (var i = 0; i < size; ++i)
            {
                page = page.Insert(i, i);
            }

            page = page.Delete(5);
            Assert.False(page.ContainsKey(5));
        }

        [Fact]
        public void Delete_Returns_New_Page_With_Keys_And_Values_Aligned()
        {
            var size = 10;
            var page = LeafPage<int, int>.Empty(size);
            for (var i = 0; i < size; ++i)
            {
                page = page.Insert(i, i);
            }

            page = page.Delete(5);
            Assert.True(page.TryRead(6, out var value));
            Assert.Equal(6, value);
        }

        [Fact]
        public void Delete_Returns_Same_Page_When_Key_Not_Found()
        {
            var size = 10;
            var page = LeafPage<int, int>.Empty(size);
            for (var i = 0; i < size; ++i)
            {
                page = page.Insert(i, i);
            }

            Assert.Equal(page, page.Delete(size + 1));
        }

        [Fact]
        public void Update_Returns_New_Page_With_New_Value()
        {
            var size = 10;
            var key = 5;
            var expectedValue = 50;
            var page = LeafPage<int, int>.Empty(size);
            for (var i = 0; i < size; ++i)
            {
                page = page.Insert(i, i);
            }

            var updatedPage = page.Update(key, expectedValue);
            Assert.True(updatedPage.TryRead(key, out var actualvalue));
            Assert.Equal(expectedValue, actualvalue);

            Assert.True(page.TryRead(key, out var originalValue));
            Assert.Equal(key, originalValue);
        }

        [Fact]
        public void Update_Throws_KeyNotFoundException_When_Key_Not_Found()
        {
            var size = 10;
            var key = size + 1;
            var page = LeafPage<int, int>.Empty(size);
            for (var i = 0; i < size; ++i)
            {
                page = page.Insert(i, i);
            }

            var ex = Assert.Throws<KeyNotFoundException>(() => page.Update(key, key));
#pragma warning disable CA1305 // Specify IFormatProvider
            Assert.Contains(key.ToString(), ex.Message);
#pragma warning restore CA1305 // Specify IFormatProvider
        }

        [Fact]
        public void Update_Throws_KeyNotFoundException_When_Page_Is_Empty()
        {
            var size = 10;
            var key = size + 1;
            var page = LeafPage<int, int>.Empty(size);

            var ex = Assert.Throws<KeyNotFoundException>(() => page.Update(key, key));
#pragma warning disable CA1305 // Specify IFormatProvider
            Assert.Contains(key.ToString(), ex.Message);
#pragma warning restore CA1305 // Specify IFormatProvider
        }

        [Fact]
        public void Split_Returns_New_Pages()
        {
            var size = 10;
            var page = LeafPage<int, int>.Empty(size);
            for (var i = 0; i < size; ++i)
            {
                page = page.Insert(i, i);
            }

            var (left, right, pivotKey) = page.Split();
            Assert.NotNull(left);
            Assert.NotNull(right);
            Assert.Equal(5, pivotKey);
        }

        [Fact]
        public void Split_Pages_Contain_Correct_Key_Subsets()
        {
            var size = 10;
            var page = LeafPage<int, int>.Empty(size);
            for (var i = 0; i < size; ++i)
            {
                page = page.Insert(i, i);
            }

            var (left, right, pivotKey) = page.Split();
            for (var i = 0; i < size / 2; ++i)
            {
                Assert.True(left.ContainsKey(i));
            }

            for (var i = pivotKey; i < pivotKey + size / 2; ++i)
            {
                Assert.True(right.ContainsKey(i));
            }
        }

        [Fact]
        public void Split_Pages_Does_Not_Contain_Incorrect_Key_Subsets()
        {
            var size = 10;
            var page = LeafPage<int, int>.Empty(size);
            for (var i = 0; i < size; ++i)
            {
                page = page.Insert(i, i);
            }

            var (left, right, pivotKey) = page.Split();
            for (var i = 0; i < size / 2; ++i)
            {
                Assert.False(right.ContainsKey(i));
            }

            for (var i = pivotKey; i < pivotKey + size / 2; ++i)
            {
                Assert.False(left.ContainsKey(i));
            }
        }
    }

    //public class LeafPageTests
    //{
    //    private readonly int pageSize = 10;

    //    [Fact]
    //    public void NewPageIsEmpty()
    //    {
    //        using var page = new LeafPage<int, int>(this.pageSize);
    //        Assert.True(page.IsEmpty);
    //    }

    //    [Fact]
    //    public void NewPageHasCorrectPageSize()
    //    {
    //        using var page = new LeafPage<int, int>(this.pageSize);
    //        Assert.Equal(this.pageSize, page.Size);
    //    }

    //    [Fact]
    //    public void InsertIncrememtsCount()
    //    {
    //        using var page = new LeafPage<int, int>(this.pageSize);
    //        var writeSuccess = page.TryWrite(1, 1, out _);
    //        Assert.True(writeSuccess);
    //        Assert.Equal(1, page.Count);
    //    }

    //    [Fact]
    //    public void SelectSubtreeReturnsSelf()
    //    {
    //        using var page = new LeafPage<int, int>(this.pageSize);
    //        var subtree = page.SelectSubtree(1);
    //        Assert.Equal(page, subtree);
    //    }

    //    [Fact]
    //    public void DivisionTest()
    //    {
    //        var i = this.pageSize / 2;
    //        Assert.Equal(5, i);
    //        i /= 2;
    //        Assert.Equal(2, i);

    //        i /= 2;
    //        Assert.Equal(1, i);

    //        i /= 2;
    //        Assert.Equal(0, i);

    //        var j = this.pageSize / 2;
    //        var x = 0;
    //        while (j > 0)
    //        {
    //            j /= 2;
    //            ++x;
    //        }

    //        Assert.Equal(3, x);
    //    }

    //    [Fact]
    //    public void IndexOfKeyTest()
    //    {
    //        using var page = new LeafPage<int, int>(this.pageSize);
    //        Assert.True(page.TryWrite(1, 1, out _)); // 0 goes to index 0
    //        Assert.True(page.TryWrite(2, 2, out _)); // 3 goes to index 2
    //        Assert.True(page.TryWrite(4, 4, out _)); // 4 goes to index 3
    //        Assert.True(page.TryWrite(5, 5, out _));
    //        Assert.True(page.TryWrite(6, 6, out _));
    //        Assert.True(page.TryWrite(7, 7, out _)); // 8 goes to index 6
    //        Assert.True(page.TryWrite(9, 9, out _));
    //        Assert.True(page.TryWrite(10, 10, out _)); // 11 goes to index 8

    //        var index = page.BinarySearch(3);
    //        Assert.Equal(2, ~index);

    //        index = page.BinarySearch(4);
    //        Assert.Equal(2, index);

    //        index = page.BinarySearch(8);
    //        Assert.Equal(6, ~index);

    //        index = page.BinarySearch(0);
    //        Assert.Equal(0, ~index);

    //        index = page.BinarySearch(10);
    //        Assert.Equal(7, index);

    //        index = page.BinarySearch(11);
    //        Assert.Equal(8, ~index);
    //    }

    //    [Fact]
    //    public void SortedInsertTest()
    //    {
    //        using var page = new LeafPage<int, int>(this.pageSize);
    //        Assert.True(page.TryWrite(1, 1, out _)); // 0 goes to index 0
    //        Assert.True(page.TryWrite(2, 2, out _)); // 3 goes to index 2
    //        Assert.True(page.TryWrite(4, 4, out _));
    //        Assert.True(page.TryWrite(5, 5, out _));
    //        Assert.True(page.TryWrite(6, 6, out _));
    //        Assert.True(page.TryWrite(7, 7, out _)); // 8 goes to index 6
    //        Assert.True(page.TryWrite(9, 9, out _));
    //        Assert.True(page.TryWrite(10, 10, out _)); // 11 goes to index 8

    //        Assert.True(page.TryWrite(3, 3, out _));
    //        var sortedKeys = page.Keys
    //            .Take(page.Count)
    //            .Order();
    //        Assert.Equal(sortedKeys, page.Keys.Take(page.Count));

    //        Assert.True(page.TryWrite(8, 8, out _));
    //        sortedKeys = page.Keys
    //            .Take(page.Count)
    //            .Order();
    //        Assert.Equal(sortedKeys, page.Keys.Take(page.Count));
    //    }

    //    [Fact]
    //    public void SplitTest()
    //    {
    //        var maxKey = this.pageSize * 2;
    //        using var leftPage = new LeafPage<int, int>(this.pageSize);
    //        for (var i = 0; i < maxKey; i += 2)
    //        {
    //            Assert.True(leftPage.TryWrite(i, i, out var responseI));
    //            Assert.Equal(WriteResult.Inserted, responseI.result);
    //            Assert.Null(responseI.newPage);
    //        }

    //        Assert.True(leftPage.TryWrite(maxKey / 2 - 1, maxKey / 2 - 1, out var responseO));
    //        Assert.Equal(WriteResult.Inserted, responseO.result);
    //        Assert.NotNull(responseO.newPage);
    //        Assert.Equal(10, responseO.newPivotKey);
    //        Assert.Equal(6, leftPage.Count);
    //        Assert.Equal(5, responseO.newPage.Count);

    //        Assert.Equal(8, leftPage.Keys[4]);
    //        Assert.Equal(9, leftPage.Keys[5]);

    //        Assert.Equal(responseO.newPivotKey, responseO.newPage.MinKey);
    //    }

    //    [Fact]
    //    public void SplitSetsNewPagePivotKey()
    //    {
    //        using var leftPage = new LeafPage<int, int>(this.pageSize);
    //        for (var i = 0; i < this.pageSize * 2; i += 2)
    //        {
    //            Assert.True(leftPage.TryWrite(i, i, out var response));
    //            Assert.Null(response.newPage);
    //        }

    //        var key = 9;
    //        Assert.True(leftPage.TryWrite(key, key, out var rightPageResponse));
    //        Assert.NotNull(rightPageResponse.newPage);
    //        Assert.Equal(10, rightPageResponse.newPivotKey);
    //        Assert.Equal(6, leftPage.Count);
    //        Assert.Equal(5, rightPageResponse.newPage.Count);

    //        Assert.Equal(rightPageResponse.newPage.MinKey, rightPageResponse.newPage.PivotKey);
    //    }

    //    [Fact]
    //    public void SplitSetsNewPagePivotKeyWithRightOnlyInsert()
    //    {
    //        using var leftPage = new LeafPage<int, int>(this.pageSize);
    //        for (var i = 0; i < leftPage.Size; ++i)
    //        {
    //            Assert.True(leftPage.TryWrite(i, i, out var response));
    //            Assert.Null(response.newPage);
    //        }

    //        Assert.True(leftPage.TryWrite(this.pageSize, this.pageSize, out var rightPageResponse));
    //        Assert.NotNull(rightPageResponse.newPage);
    //        Assert.Equal(10, rightPageResponse.newPivotKey);
    //        Assert.Equal(10, leftPage.Count);
    //        Assert.Equal(1, rightPageResponse.newPage.Count);

    //        for (var i = 0; i < leftPage.Count; ++i)
    //        {
    //            Assert.Equal(i, leftPage.Keys[i]);
    //        }

    //        Assert.Equal(rightPageResponse.newPage.MinKey, rightPageResponse.newPage.PivotKey);
    //    }

    //    [Fact]
    //    public void TryDeleteReturnsFalseWhenKeyNotFound()
    //    {
    //        using var page = new LeafPage<int, int>(this.pageSize);
    //        for (var i = 0; i < this.pageSize; ++i)
    //        {
    //            Assert.True(page.TryWrite(i, i, out _));
    //        }

    //        var deleted = page.TryDelete(this.pageSize, out _);
    //        Assert.False(deleted);
    //    }

    //    [Fact]
    //    public void TryDeleteReturnsTrueWhenKeyFound()
    //    {
    //        using var page = new LeafPage<int, int>(this.pageSize);
    //        for (var i = 0; i < this.pageSize; ++i)
    //        {
    //            Assert.True(page.TryWrite(i, i, out _));
    //        }

    //        var deleted = page.TryDelete(this.pageSize / 2, out _);
    //        Assert.True(deleted);
    //    }

    //    [Fact]
    //    public void TryDeleteRemovesKeyAndChild()
    //    {
    //        using var page = new LeafPage<int, int>(this.pageSize);
    //        for (var i = 0; i < this.pageSize; ++i)
    //        {
    //            Assert.True(page.TryWrite(i, i, out _));
    //        }

    //        _ = page.TryDelete(this.pageSize / 2, out _);
    //        Assert.DoesNotContain(this.pageSize / 2, page.Keys);
    //        Assert.DoesNotContain(this.pageSize / 2, page.values);
    //        Assert.Equal(page.Keys.Length, page.values.Length);
    //    }

    //    [Fact]
    //    public void TryDeleteUpdatesCount()
    //    {
    //        using var page = new LeafPage<int, int>(this.pageSize);
    //        for (var i = 0; i < this.pageSize; ++i)
    //        {
    //            Assert.True(page.TryWrite(i, i, out _));
    //        }

    //        _ = page.TryDelete(this.pageSize / 2, out _);
    //        Assert.Equal(this.pageSize - 1, page.Count);
    //    }

    //    [Fact]
    //    public void TryDeleteDoesNotMergeWhenCountGTKDiv2()
    //    {
    //        using var page = new LeafPage<int, int>(this.pageSize);
    //        for (var i = 0; i < this.pageSize; ++i)
    //        {
    //            Assert.True(page.TryWrite(i, i, out _));
    //        }

    //        _ = page.TryDelete(this.pageSize / 2, out var mergeInfo);
    //        Assert.False(mergeInfo.merged);
    //    }

    //    [Fact]
    //    public void TryDeleteMergesWhenCountLTKDiv2()
    //    {
    //        using var leftSibling = new LeafPage<int, int>(this.pageSize);
    //        for (var i = 0; i < 3; ++i)
    //        {
    //            Assert.True(leftSibling.TryWrite(i, i, out _));
    //        }

    //        using var page = new LeafPage<int, int>(this.pageSize, leftSibling);
    //        for (var i = 4; i < 4 + 3; ++i)
    //        {
    //            Assert.True(page.TryWrite(i, i, out _));
    //        }

    //        using var rightSibling = new LeafPage<int, int>(this.pageSize, page);
    //        for (var i = 8; i < 8 + 3; ++i)
    //        {
    //            Assert.True(rightSibling.TryWrite(i, i, out _));
    //        }

    //        Assert.Equal(3, page.Count);

    //        var deleted = page.TryDelete(page.MaxKey, out var mergeInfo);
    //        Assert.True(deleted, "unexpected deleted value");
    //        Assert.True(mergeInfo.merged, "unexpected merged value");
    //    }

    //    [Fact]
    //    public void TryDeleteMergesLeftWhenCountLTKDiv2AndPreferredChoiceIsCurrentPage()
    //    {
    //        using var leftSibling = new LeafPage<int, int>(this.pageSize);
    //        for (var i = 0; i < this.pageSize; ++i)
    //        {
    //            Assert.True(leftSibling.TryWrite(i, i, out _));
    //        }

    //        var (page, pivotKey) = leftSibling.Split();
    //        Assert.True(page.PivotKey == pivotKey, "page pivot doesn't match");

    //        var (rightSibling, rightSiblingPivotKey) = leftSibling.Split();
    //        Assert.True(rightSibling.PivotKey == rightSiblingPivotKey, "right sibling pivot doesn't match");

    //        _ = page.TryDelete(page.PivotKey, out var mergeInfo);
    //        Assert.Equal(page.PivotKey, mergeInfo.deprecatedPivotKey);
    //    }

    //    [Fact]
    //    public void TryDeleteMergesRightWhenCountLTKDiv2AndPreferredChoiceIsRightSibling()
    //    {
    //        using var leftPage = new LeafPage<int, int>(this.pageSize);
    //        var count = this.pageSize * 10;
    //        for (var i = 0; i < count; i += 10)
    //        {
    //            Assert.True(leftPage.TryWrite(i, i, out _));
    //        }

    //        var (middlePage, middlePivotKey) = leftPage.Split();
    //        Assert.Equal(50, middlePivotKey);
    //        var (rightPage, rightPivotKey) = middlePage.Split();
    //        Assert.Equal(70, rightPivotKey);

    //        for (var i = 61; i < 69; ++i)
    //        {
    //            Assert.True(middlePage.TryWrite(i, i, out var response));
    //            Assert.Null(response.newPage);
    //            Assert.Equal(WriteResult.Inserted, response.result);
    //        }

    //        var deleted = rightPage.TryDelete(rightPage.PivotKey, out var mergeInfo1);
    //        Assert.True(deleted);
    //        Assert.False(mergeInfo1.merged);

    //        var deleteUntilIndex = middlePage.Count / 2;
    //        for (var i = middlePage.Count - 1; i >= deleteUntilIndex; i--)
    //        {
    //            Assert.True(middlePage.TryDelete(middlePage.Keys[i], out _));
    //        }

    //        _ = middlePage.TryDelete(middlePage.PivotKey, out var mergeInfo);
    //        Assert.True(mergeInfo.merged, "merged?");
    //        Assert.Equal(rightPage.PivotKey, mergeInfo.deprecatedPivotKey);
    //    }

    //    [Fact]
    //    public void TryDeleteDoesntMergeWhenNoMergeCandidate()
    //    {
    //        // 1. fill a page
    //        using var leftSibling = new LeafPage<int, int>(this.pageSize);
    //        for (var i = 0; i < this.pageSize; ++i)
    //        {
    //            Assert.True(leftSibling.TryWrite(i, i, out _));
    //        }

    //        // 2. split it and then split the result so middle and right pages are both in underflow state
    //        var (middlePage, _) = leftSibling.Split();
    //        var (rightSibling, _) = middlePage.Split();

    //        // 3. fill the middle page so it is no longer in underflow and is unmergable
    //        var insertCount = middlePage.Size - middlePage.Count;
    //        var max = middlePage.MaxKey;
    //        for (var i = max + 1; i < max + 1 + insertCount; ++i)
    //        {
    //            Assert.True(middlePage.TryWrite(i, i, out var response));
    //            Assert.Null(response.newPage);
    //            Assert.Equal(WriteResult.Inserted, response.result);
    //        }

    //        // 4. delete an item from the right sibling in an unmergable condition
    //        _ = rightSibling.TryDelete(rightSibling.PivotKey, out var rightSiblingMergeInfo);
    //        Assert.False(rightSiblingMergeInfo.merged, "right sibling should not merge");
    //    }

    //    [Fact]
    //    public void TryDeleteMergeInfoReturnsRightPivotKeyWhenLeftMostPage()
    //    {
    //        using var leftPage = new LeafPage<int, int>(this.pageSize);
    //        for (var i = 0; i < this.pageSize; ++i)
    //        {
    //            Assert.True(leftPage.TryWrite(i, i, out _));
    //        }

    //        var (rightPage, _) = leftPage.Split();

    //        _ = leftPage.TryDelete(leftPage.MinKey, out var mergeInfo);
    //        Assert.True(mergeInfo.merged, "merged shoud be true");
    //        Assert.True(rightPage.PivotKey == mergeInfo.deprecatedPivotKey, "deprecated pivot key comes from right page");
    //    }
    //}
}
