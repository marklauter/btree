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
        public void Insert_Returns_Page()
        {
            var page = DataPage<int, int>.Empty;
            var newPage = page.Insert(1, 1);
            Assert.True(page.IsEmpty);
            Assert.False(newPage.IsEmpty);
            Assert.True(page.CompareTo(newPage) != 0);
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
            for (var key = 0; key < length; ++key)
            {
                page = page.Insert(key, guids[key]);
            }

            for (var key = 0; key < length; ++key)
            {
                Assert.True(page.ContainsKey(key));
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
            for (var key = 0; key < length; ++key)
            {
                page = page.Insert(key, guids[key]);
            }

            for (var key = length; key < length * 2; ++key)
            {
                Assert.False(page.ContainsKey(key));
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
            for (var key = 0; key < length; ++key)
            {
                page = page.Insert(key, guids[key]);
            }

            for (var key = 0; key < length; ++key)
            {
                var index = page.IndexOfKey(key);
                Assert.Equal(key, index);
            }

            var expected = ~page.Length;
            for (var key = length; key < length * 2; ++key)
            {
                var index = page.IndexOfKey(key);
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
            for (var key = 0; key < length; ++key)
            {
                page = page.Insert(key, guids[key]);
            }

            for (var key = 0; key < length; ++key)
            {
                var values = page.Read(key);
                var actual = Assert.Single(values);
                Assert.Equal(guids[key], actual);
            }

            for (var key = length; key < length * 2; ++key)
            {
                var values = page.Read(key);
                Assert.Empty(values);
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
        public void Delete_Key_Returns_Page_With_KeyValuesTuple_Removed(int length)
        {
            var guids = UniqueRandoms(length);
            var page = DataPage<int, Guid>.Empty;
            for (var key = 0; key < length; ++key)
            {
                page = page.Insert(key, guids[key]);
            }

            for (var key = 0; key < length; ++key)
            {
                Assert.True(page.ContainsKey(key));
                page = page.Delete(key);
                Assert.False(page.ContainsKey(key));
            }
        }

        [Theory]
        [InlineData(2)]
        [InlineData(4)]
        [InlineData(8)]
        [InlineData(16)]
        [InlineData(32)]
        [InlineData(64)]
        public void Insert_Multiple_Values_Per_Key(int length)
        {
            var guids = UniqueRandoms(length);
            var page = DataPage<int, Guid>.Empty;
            for (var i = 0; i < length; ++i)
            {
                var key = i >> 1;
                page = page.Insert(key, guids[i]);
            }

            Assert.Equal(length, page.Count());
            Assert.Equal(length >> 1, page.Length);

            for (var i = 0; i < length; ++i)
            {
                var key = i >> 1;
                Assert.True(page.ContainsKey(key));
                var values = page.Read(key);
                Assert.Equal(2, values.Length);
            }
        }

        [Theory]
        [InlineData(2)]
        [InlineData(4)]
        [InlineData(8)]
        [InlineData(16)]
        [InlineData(32)]
        [InlineData(64)]
        public void Delete_KeyValue_Returns_Page_With_Only_The_Specific_Value_Removed(int length)
        {
            var guids = UniqueRandoms(length);
            var page = DataPage<int, Guid>.Empty;
            for (var i = 0; i < length; ++i)
            {
                var key = i >> 1;
                page = page.Insert(key, guids[i]);
            }

            for (var key = 0; key < length >> 1; key += 2)
            {
                Assert.True(page.ContainsKey(key));
                var values = page.Read(key);
                Assert.Equal(2, values.Length);

                var first = values[0];
                var second = values[1];

                page = page.Delete(key, first);
                values = page.Read(key);

                var actual = Assert.Single(values);
                Assert.Equal(second, actual);
                Assert.DoesNotContain(first, values);
            }
        }

        [Theory]
        [InlineData(2)]
        [InlineData(4)]
        [InlineData(8)]
        [InlineData(16)]
        [InlineData(32)]
        [InlineData(64)]
        public void Delete_Returns_Same_Page_When_Key_Not_Found(int length)
        {
            var guids = UniqueRandoms(length);
            var page = DataPage<int, Guid>.Empty;
            for (var key = 0; key < length; key += 2)
            {
                page = page.Insert(key, guids[key]);
            }

            for (var key = 1; key < length; key += 2)
            {
                Assert.False(page.ContainsKey(key));
                Assert.Equal(page, page.Delete(key));
            }
        }

        [Theory]
        [InlineData(2)]
        [InlineData(5)]
        [InlineData(8)]
        [InlineData(13)]
        [InlineData(21)]
        [InlineData(24)]
        public void Split_Returns_New_Pages(int length)
        {
            var guids = UniqueRandoms(length);
            var page = DataPage<int, Guid>.Empty;
            for (var key = 0; key < length; ++key)
            {
                page = page.Insert(key, guids[key]);
            }

            var split = page.Split();
            Assert.Equal(length >> 1, split.LeftPage.Length);
            if (page.Length % 2 == 0)
            {
                Assert.Equal(length >> 1, split.RightPage.Length);
            }
            else
            {
                Assert.Equal((length >> 1) + 1, split.RightPage.Length);
            }

            Assert.Equal(page.Length >> 1, split.PivotKey);
        }

        [Theory]
        [InlineData(2)]
        [InlineData(5)]
        [InlineData(8)]
        [InlineData(13)]
        [InlineData(21)]
        [InlineData(24)]
        public void Split_Pages_Contain_Correct_Key_Subsets(int length)
        {
            var guids = UniqueRandoms(length);
            var page = DataPage<int, Guid>.Empty;
            for (var key = 0; key < length; ++key)
            {
                page = page.Insert(key, guids[key]);
            }

            var split = page.Split();

            for (var key = 0; key < split.PivotKey; ++key)
            {
                Assert.True(split.LeftPage.ContainsKey(key));
            }

            for (var i = 0; i < split.RightPage.Length; ++i)
            {
                var key = split.PivotKey + i;
                Assert.True(split.RightPage.ContainsKey(key));
            }
        }

        [Theory]
        [InlineData(2)]
        [InlineData(5)]
        [InlineData(8)]
        [InlineData(13)]
        [InlineData(21)]
        [InlineData(24)]
        public void Split_Pages_Does_Not_Contain_Incorrect_Key_Subsets(int length)
        {
            var guids = UniqueRandoms(length);
            var page = DataPage<int, Guid>.Empty;
            for (var key = 0; key < length; ++key)
            {
                page = page.Insert(key, guids[key]);
            }

            var split = page.Split();

            for (var key = split.PivotKey; key < length; ++key)
            {
                Assert.False(split.LeftPage.ContainsKey(key));
            }

            for (var key = 0; key < split.PivotKey; ++key)
            {
                Assert.False(split.RightPage.ContainsKey(key));
            }
        }

        [Theory]
        [InlineData(2)]
        [InlineData(5)]
        [InlineData(8)]
        [InlineData(13)]
        [InlineData(21)]
        [InlineData(24)]
        public void Merged_Left_Pages_Contain_All_Keys(int length)
        {
            var guids = UniqueRandoms(length);
            var page = DataPage<int, Guid>.Empty;
            for (var key = 0; key < length; ++key)
            {
                page = page.Insert(key, guids[key]);
            }

            var split = page.Split();
            var merged = split.LeftPage.Merge(split.RightPage);

            for (var key = 0; key < length; ++key)
            {
                Assert.True(merged.ContainsKey(key));
            }
        }

        [Theory]
        [InlineData(2)]
        [InlineData(5)]
        [InlineData(8)]
        [InlineData(13)]
        [InlineData(21)]
        [InlineData(24)]
        public void Merged_Right_Pages_Contain_All_Keys(int length)
        {
            var guids = UniqueRandoms(length);
            var page = DataPage<int, Guid>.Empty;
            for (var key = 0; key < length; ++key)
            {
                page = page.Insert(key, guids[key]);
            }

            var split = page.Split();
            var merged = split.RightPage.Merge(split.LeftPage);

            for (var key = 0; key < length; ++key)
            {
                Assert.True(merged.ContainsKey(key));
            }
        }
    }
}

