using BTrees.Pages;
using BTrees.Types;
using System.Text;

namespace BTrees.Tests.Pages
{
    public sealed class DataPageTests
    {
        [Fact]
        public void Empty_Page_Has_Correct_Length()
        {
            var page = DataPage<DbInt32, DbInt32>.Empty;
            Assert.Equal(0, page.Length);
        }

        [Fact]
        public void Empty_Page_Has_Correct_Count()
        {
            var page = DataPage<DbInt32, DbInt32>.Empty;
            Assert.Equal(0, page.Count());
        }

        [Fact]
        public void Empty_Page_IsEmpty()
        {
            var page = DataPage<DbInt32, DbInt32>.Empty;
            Assert.True(page.IsEmpty);
        }

        [Fact]
        public void Insert_Returns_Page()
        {
            var page = DataPage<DbInt32, DbInt32>.Empty;
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
            var page = DataPage<DbInt32, DbInt32>.Empty;
            page = page.Insert(1, 1);
            Assert.True(page.ContainsKey(1));
        }

        [Fact]
        public void Read_Returns_Inserted_Element()
        {
            var expectedValue = "one";
            var page = DataPage<DbInt32, DbText>.Empty
                .Insert(1, expectedValue);
            var values = page.Read(1);
            var actualValue = Assert.Single(values);
            Assert.Equal(expectedValue, (string)actualValue, true);
        }

        [Fact]
        public void Inserted_Multiple_Values_With_Duplicate_Key_Read_Returns_All_Inserted_Elements()
        {
            var expectedValue1 = "one";
            var expectedValue2 = "two";
            var page = DataPage<DbInt32, DbText>.Empty
                .Insert(1, expectedValue1)
                .Insert(1, expectedValue2);

            Assert.Equal(1, page.Length);
            Assert.Equal(2, page.Count());

            var values = page.Read(1);
            Assert.Equal(2, values.Length);
            Assert.Contains((DbText)expectedValue1, values);
            Assert.Contains((DbText)expectedValue2, values);
        }

        [Fact]
        public void Insert_Returns_New_Page_That_Contains_N_Inserted_Elements()
        {
            var length = 5;
            var page = DataPage<DbInt32, DbInt32>.Empty;
            for (var i = 0; i < length; ++i)
            {
                page = page.Insert(i, i);
            }

            Assert.Equal(length, page.Length);
            Assert.Equal(length, page.Count());
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
            var guids = UniqueIdFactory.Generate(length);
            var page = DataPage<DbInt32, DbUniqueId>.Empty;
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
            var guids = UniqueIdFactory.Generate(length);
            var page = DataPage<DbInt32, DbUniqueId>.Empty;
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
            var guids = UniqueIdFactory.Generate(length);
            var page = DataPage<DbInt32, DbUniqueId>.Empty;
            for (var key = 0; key < length; ++key)
            {
                page = page.Insert(key, guids[key]);
            }

            for (var key = 0; key < length; ++key)
            {
                var index = page.IndexOf(key);
                Assert.Equal(key, index);
            }

            var expected = ~page.Length;
            for (var key = length; key < length * 2; ++key)
            {
                var index = page.IndexOf(key);
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
            var guids = UniqueIdFactory.Generate(length);
            var page = DataPage<DbInt32, DbUniqueId>.Empty;
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

        [Fact]
        public void Read_Range_Returns_All_Values()
        {
            var page = DataPage<DbInt32, DbInt32>.Empty;
            for (var key = 0; key < 10; ++key)
            {
                page = page.Insert(key, key);
            }
            var values = page
                .Read(0..10)
                .Select(kvp => kvp.Value);
            Assert.Equal(10, values.Count());
            for (var key = 0; key < 10; ++key)
            {
                Assert.Contains((DbInt32)key, values);
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
        public void Remove_Key_Returns_Page_With_KeyValuesTuple_Removed(int length)
        {
            var guids = UniqueIdFactory.Generate(length);
            var page = DataPage<DbInt32, DbUniqueId>.Empty;
            for (var key = 0; key < length; ++key)
            {
                page = page.Insert(key, guids[key]);
            }

            for (var key = 0; key < length; ++key)
            {
                Assert.True(page.ContainsKey(key));
                page = page.Remove(key);
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
            var guids = UniqueIdFactory.Generate(length);
            var page = DataPage<DbInt32, DbUniqueId>.Empty;
            for (var i = 0; i < length; ++i)
            {
                var key = i >> 1;
                page = page.Insert(key, (Guid)guids[i]);
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
        public void Remove_KeyValue_Returns_Page_With_Only_The_Specific_Value_Removed(int length)
        {
            var guids = UniqueIdFactory.Generate(length);
            var page = DataPage<DbInt32, DbUniqueId>.Empty;
            for (var i = 0; i < length; ++i)
            {
                var key = i >> 1;
                page = page.Insert(key, (Guid)guids[i]);
            }

            for (var key = 0; key < length >> 1; key += 2)
            {
                Assert.True(page.ContainsKey(key));
                var values = page.Read(key);
                Assert.Equal(2, values.Length);

                var first = values[0];
                var second = values[1];

                page = page.Remove(key, first);
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
        public void Remove_Returns_Same_Page_When_Key_Not_Found(int length)
        {
            var guids = UniqueIdFactory.Generate(length);
            var page = DataPage<DbInt32, DbUniqueId>.Empty;
            for (var key = 0; key < length; key += 2)
            {
                page = page.Insert(key, guids[key]);
            }

            for (var key = 1; key < length; key += 2)
            {
                Assert.False(page.ContainsKey(key));
                Assert.Equal(page, page.Remove(key));
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
            var guids = UniqueIdFactory.Generate(length);
            var page = DataPage<DbInt32, DbUniqueId>.Empty;
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
            var guids = UniqueIdFactory.Generate(length);
            var page = DataPage<DbInt32, DbUniqueId>.Empty;
            for (var key = 0; key < length; ++key)
            {
                page = page.Insert(key, guids[key]);
            }

            var split = page.Split();

            for (var key = 0; key < split.LeftPage.Length; ++key)
            {
                Assert.True(split.LeftPage.ContainsKey(key));
            }

            var pivotKey = split.RightPage.MinKey;
            for (var i = 0; i < split.RightPage.Length; ++i)
            {
                var key = pivotKey + i;
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
            var guids = UniqueIdFactory.Generate(length);
            var page = DataPage<DbInt32, DbUniqueId>.Empty;
            for (var key = 0; key < length; ++key)
            {
                page = page.Insert(key, guids[key]);
            }

            var split = page.Split();
            var pivotKey = split.RightPage.MinKey;
            for (var key = pivotKey; key < length; ++key)
            {
                Assert.False(split.LeftPage.ContainsKey(key));
            }

            for (var key = 0; key < pivotKey; ++key)
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
            var guids = UniqueIdFactory.Generate(length);
            var page = DataPage<DbInt32, DbUniqueId>.Empty;
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
            var guids = UniqueIdFactory.Generate(length);
            var page = DataPage<DbInt32, DbUniqueId>.Empty;
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

        [Theory]
        [InlineData(2)]
        [InlineData(5)]
        [InlineData(8)]
        [InlineData(13)]
        [InlineData(21)]
        [InlineData(24)]
        public void Size_Matches_Prediction(int length)
        {
            var size = 0;
            var guids = UniqueIdFactory.Generate(length);
            var page = DataPage<DbInt32, DbUniqueId>.Empty;
            for (var key = 0; key < length; ++key)
            {
                size += DbInt32.Size + DbUniqueId.Size;
                page = page.Insert(key, guids[key]);
                Assert.Equal(size, page.Size);
            }
        }

        [Theory]
        [InlineData(2)]
        [InlineData(5)]
        [InlineData(8)]
        [InlineData(13)]
        [InlineData(21)]
        [InlineData(24)]
        public void Size_Matches_Prediction_With_Variable_Size_Type(int length)
        {
            var builder = new StringBuilder();
            for (var i = 0; i < length; ++i)
            {
                builder = builder.Append('x');
            }

            var s = builder.ToString();
            builder = builder.Clear();
            var size = 0;
            var page = DataPage<DbInt32, DbText>.Empty;
            for (var key = 0; key < length; ++key)
            {
                builder = builder.Append(s);
                var value = builder.ToString();
                size += DbInt32.Size + DbInt32.Size + value.Length;
                page = page.Insert(key, value);
                Assert.Equal(size, page.Size);
            }
        }
    }
}

