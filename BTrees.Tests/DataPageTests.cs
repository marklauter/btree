using BTrees.Pages;

namespace BTrees.Tests
{
    public sealed class DataPageTests
    {
        [Fact]
        public void Empty_Page_Has_Correct_Length()
        {
            var page = DataPage<int, int>.Empty;
            Assert.Equal(0, page.Length);
        }

        [Fact]
        public void Empty_Page_Has_Correct_Count()
        {
            var page = DataPage<int, int>.Empty;
            Assert.Equal(0, page.Count());
        }

        [Fact]
        public void Empty_Page_IsEmpty()
        {
            var page = DataPage<int, int>.Empty;
            Assert.True(page.IsEmpty);
        }

        [Fact]
        public void Insert_Returns_New_Page()
        {
            var page = DataPage<int, int>.Empty;
            var newPage = page.Insert(1, 1);
            Assert.True(page.IsEmpty);
            Assert.False(newPage.IsEmpty);
            Assert.True(page != newPage);
            Assert.Equal(1, newPage.Length);
            Assert.Equal(1, newPage.Count());
        }

        [Fact]
        public void NewPage_Returned_By_Insert_Contains_Inserted_Key()
        {
            var page = DataPage<int, int>.Empty;
            page = page.Insert(1, 1);
            Assert.True(page.ContainsKey(1));
        }

        [Fact]
        public void Read_Returns_Inserted_Element()
        {
            var expectedValue = "one";
            var page = DataPage<int, string>.Empty
                .Insert(1, expectedValue);
            var values = page.Read(1);
            var actualValue = Assert.Single(values);
            Assert.Equal(expectedValue, actualValue, true);
        }

        [Fact]
        public void Inserted_Multiple_Values_With_Duplicate_Key_Read_Returns_All_Inserted_Elements()
        {
            var expectedValue1 = "one";
            var expectedValue2 = "two";
            var page = DataPage<int, string>.Empty
                .Insert(1, expectedValue1)
                .Insert(1, expectedValue2);

            Assert.Equal(1, page.Length);
            Assert.Equal(2, page.Count());

            var values = page.Read(1);
            Assert.Equal(2, values.Length);
            Assert.Contains(expectedValue1, values);
            Assert.Contains(expectedValue2, values);
        }

        [Fact]
        public void Insert_Returns_New_Page_That_Contains_N_Inserted_Elements()
        {
            var length = 5;
            var page = DataPage<int, int>.Empty;
            for (var i = 0; i < length; ++i)
            {
                page = page.Insert(i, i);
            }

            Assert.Equal(length, page.Length);
            Assert.Equal(length, page.Count());
        }

        private static Guid[] UniqueRandoms(int length)
        {
            var guids = new Guid[length];
            for (var i = 0; i < length; ++i)
            {
                guids[i] = Guid.NewGuid();
            }

            return guids;
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(5)]
        [InlineData(8)]
        [InlineData(13)]
        [InlineData(21)]
        [InlineData(24)]
        public void ContainsKey_Returns_True(int length)
        {
            var guids = UniqueRandoms(length);
            var page = DataPage<int, Guid>.Empty;
            for (var i = 0; i < length; ++i)
            {
                page = page.Insert(i, guids[i]);
            }

            for (var i = 0; i < length; ++i)
            {
                Assert.True(page.ContainsKey(i));
            }
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(5)]
        [InlineData(8)]
        [InlineData(13)]
        [InlineData(21)]
        [InlineData(24)]
        public void ContainsKey_Returns_False(int length)
        {
            var guids = UniqueRandoms(length);
            var page = DataPage<int, Guid>.Empty;
            for (var i = 0; i < length; ++i)
            {
                page = page.Insert(i, guids[i]);
            }

            for (var i = length; i < length * 2; ++i)
            {
                Assert.False(page.ContainsKey(i));
            }
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(5)]
        [InlineData(8)]
        [InlineData(13)]
        [InlineData(21)]
        [InlineData(24)]
        public void IndexOfKey_Returns_Correct_Key(int length)
        {
            var guids = UniqueRandoms(length);
            var page = DataPage<int, Guid>.Empty;
            for (var i = 0; i < length; ++i)
            {
                page = page.Insert(i, guids[i]);
            }

            for (var i = 0; i < length; ++i)
            {
                var index = page.IndexOfKey(i);
                Assert.Equal(i, index);
            }

            var expected = ~page.Length;
            for (var i = length; i < length * 2; ++i)
            {
                var index = page.IndexOfKey(i);
                Assert.Equal(expected, index);
            }
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(5)]
        [InlineData(8)]
        [InlineData(13)]
        [InlineData(21)]
        [InlineData(24)]
        public void Read_Returns_Correct_Values(int length)
        {
            var guids = UniqueRandoms(length);
            var page = DataPage<int, Guid>.Empty;
            for (var i = 0; i < length; ++i)
            {
                page = page.Insert(i, guids[i]);
            }

            for (var i = 0; i < length; ++i)
            {
                var values = page.Read(i);
                var actual = Assert.Single(values);
                Assert.Equal(guids[i], actual);
            }

            for (var i = length; i < length * 2; ++i)
            {
                var values = page.Read(i);
                Assert.Empty(values);
            }
        }

        //        [Fact]
        //        public void Delete_Returns_New_Page_With_Key_Removed()
        //        {
        //            var size = 10;
        //            var page = DataPage<int, int>.Empty(size);
        //            for (var i = 0; i < size; ++i)
        //            {
        //                page = page.Insert(i, i);
        //            }

        //            page = page.Delete(5);
        //            Assert.False(page.ContainsKey(5));
        //            Assert.Equal(size - 1, page.Count);
        //        }

        //        [Fact]
        //        public void Delete_Returns_New_Page_With_Keys_And_Values_Aligned()
        //        {
        //            var size = 10;
        //            var page = DataPage<int, int>.Empty(size);
        //            for (var i = 0; i < size; ++i)
        //            {
        //                page = page.Insert(i, i);
        //            }

        //            page = page.Delete(5);
        //            Assert.True(page.TryRead(6, out var value));
        //            Assert.Equal(6, value);
        //        }

        //        [Fact]
        //        public void Delete_Returns_Same_Page_When_Key_Not_Found()
        //        {
        //            var size = 10;
        //            var page = DataPage<int, int>.Empty(size);
        //            for (var i = 0; i < size; ++i)
        //            {
        //                page = page.Insert(i, i);
        //            }

        //            Assert.Equal(page, page.Delete(size + 1));
        //        }

        //        [Fact]
        //        public void Split_Returns_New_Pages()
        //        {
        //            var size = 10;
        //            var page = DataPage<int, int>.Empty(size);
        //            for (var i = 0; i < size; ++i)
        //            {
        //                page = page.Insert(i, i);
        //            }

        //            var (left, right, pivotKey) = page.Split();

        //            Assert.NotNull(left);
        //            Assert.Equal(size / 2, left.Count);

        //            Assert.NotNull(right);
        //            Assert.Equal(size / 2, right.Count);

        //            Assert.Equal(5, pivotKey);
        //        }

        //        [Fact]
        //        public void Split_Pages_Contain_Correct_Key_Subsets()
        //        {
        //            var size = 10;
        //            var page = DataPage<int, int>.Empty(size);
        //            for (var i = 0; i < size; ++i)
        //            {
        //                page = page.Insert(i, i);
        //            }

        //            var (left, right, pivotKey) = page.Split();
        //            for (var i = 0; i < size / 2; ++i)
        //            {
        //                Assert.True(left.ContainsKey(i));
        //            }

        //            for (var i = pivotKey; i < pivotKey + size / 2; ++i)
        //            {
        //                Assert.True(right.ContainsKey(i));
        //            }
        //        }

        //        [Fact]
        //        public void Split_Pages_Does_Not_Contain_Incorrect_Key_Subsets()
        //        {
        //            var size = 10;
        //            var page = DataPage<int, int>.Empty(size);
        //            for (var i = 0; i < size; ++i)
        //            {
        //                page = page.Insert(i, i);
        //            }

        //            var (left, right, pivotKey) = page.Split();
        //            for (var i = 0; i < size / 2; ++i)
        //            {
        //                Assert.False(right.ContainsKey(i));
        //            }

        //            for (var i = pivotKey; i < pivotKey + size / 2; ++i)
        //            {
        //                Assert.False(left.ContainsKey(i));
        //            }
        //        }

        //        [Fact]
        //        public void Merge_Left_Contains_All_Keys()
        //        {
        //            var size = 10;
        //            var page = DataPage<int, int>.Empty(size);
        //            for (var i = 0; i < size; ++i)
        //            {
        //                page = page.Insert(i, i);
        //            }

        //            var (left, right, _) = page.Split();
        //            var mergedPage = left.Merge(right);

        //            for (var i = 0; i < size; ++i)
        //            {
        //                Assert.True(mergedPage.ContainsKey(i));
        //            }
        //        }

        //        [Fact]
        //        public void Merge_Right_Contains_All_Keys()
        //        {
        //            var size = 10;
        //            var page = DataPage<int, int>.Empty(size);
        //            for (var i = 0; i < size; ++i)
        //            {
        //                page = page.Insert(i, i);
        //            }

        //            var (left, right, _) = page.Split();
        //            var mergedPage = right.Merge(left);

        //            for (var i = 0; i < size; ++i)
        //            {
        //                Assert.True(mergedPage.ContainsKey(i));
        //            }
        //        }
    }
}

