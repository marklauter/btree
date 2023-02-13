using BTrees.Nodes;
using BTrees.Pages;

namespace BTrees.Tests
{
    public sealed class DataNodeTests
    {
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

        [Fact]
        public void Empty_Node_Has_Correct_Size()
        {
            var size = 10;
            var node = DataNode<int, int>.Empty(size);
            Assert.Equal(size, node.Size);
        }

        [Fact]
        public void Empty_Node_Has_Correct_Count()
        {
            var size = 10;
            var node = DataNode<int, int>.Empty(size);
            Assert.Equal(0, node.Count);
        }

        [Fact]
        public void Empty_Node_IsEmpty()
        {
            var size = 10;
            var node = DataNode<int, int>.Empty(size);
            Assert.True(node.IsEmpty);
        }

        [Fact]
        public void Node_Contains_Key_After_Insert()
        {
            var size = 10;
            var key = 1;
            var value = 1;
            var node = DataNode<int, int>.Empty(size);
            node.Insert(key, value);

            Assert.True(node.ContainsKey(key));
        }

        [Fact]
        public void Node_Contains_Value_After_Insert()
        {
            var size = 10;
            var key = 1;
            var value = 1;
            var node = DataNode<int, int>.Empty(size);
            node.Insert(key, value);

            Assert.True(node.TryRead(key, out var actualValue));
            Assert.Equal(value, actualValue);
        }

        [Fact]
        public void Node_Contains_N_Inserted_Elements()
        {
            var size = 10;
            var node = DataNode<int, int>.Empty(size);
            for (var i = 0; i < size; ++i)
            {
                node.Insert(i, i);
            }

            Assert.Equal(size, node.Count);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(5)]
        [InlineData(7)]
        [InlineData(13)]
        public void Multi_Threaded_Insert_Adds_All_Elements_In_Correct_Order(int rndSeed)
        {
            var size = 5000;
            var node = DataNode<int, int>.Empty(size);
            var rndArray = UniqueRandoms(rndSeed, size, size);

            var tasks = new Task[size];
            for (var i = 0; i < size; ++i)
            {
                var key = rndArray[i];
                var value = key;
                tasks[i] = Task.Run(() => node.Insert(key, value));
            }

            Task.WaitAll(tasks);
            Assert.Equal(size, node.Count);

            for (var i = 0; i < size; ++i)
            {
                var index = node.BinarySearch(rndArray[i]);
                Assert.Equal(rndArray[i], index);
            }

            for (var i = 0; i < size; ++i)
            {
                Assert.True(node.TryRead(rndArray[i], out var actualValue));
                Assert.Equal(rndArray[i], actualValue);
            }
        }
    }

    public sealed class DataPageTests
    {
        [Fact]
        public void Empty_Page_Has_Correct_Size()
        {
            var size = 10;
            var page = DataPage<int, int>.Empty(size);
            Assert.Equal(size, page.Size);
        }

        [Fact]
        public void Empty_Page_Has_Correct_Count()
        {
            var size = 10;
            var page = DataPage<int, int>.Empty(size);
            Assert.Equal(0, page.Count);
        }

        [Fact]
        public void Empty_Page_IsEmpty()
        {
            var size = 10;
            var page = DataPage<int, int>.Empty(size);
            Assert.True(page.IsEmpty);
        }

        [Fact]
        public void Insert_Returns_New_Page()
        {
            var size = 10;
            var page = DataPage<int, int>.Empty(size);
            var newPage = page.Insert(1, 1);
            Assert.False(page == newPage);
        }

        [Fact]
        public void Insert_Returns_New_Page_With_Count_Plus_One_Elements()
        {
            var size = 10;
            var page = DataPage<int, int>.Empty(size);
            var newPage = page.Insert(1, 1);
            Assert.Equal(page.Count + 1, newPage.Count);
        }

        [Fact]
        public void Insert_Returns_New_NonEmpty_Page()
        {
            var size = 10;
            var page = DataPage<int, int>.Empty(size);
            var newPage = page.Insert(1, 1);
            Assert.False(newPage.IsEmpty);
        }

        [Fact]
        public void Insert_Returns_New_Page_That_Contains_Inserted_Key()
        {
            var size = 10;
            var page = DataPage<int, string>.Empty(size);
            var newPage = page.Insert(1, "one");
            Assert.True(newPage.ContainsKey(1));
        }

        [Fact]
        public void Insert_Returns_New_Page_That_Contains_Inserted_Element()
        {
            var size = 10;
            var expectedValue = "one";
            var page = DataPage<int, string>.Empty(size);
            var newPage = page.Insert(1, expectedValue);
            Assert.True(newPage.TryRead(1, out var actualValue));
            Assert.Equal(expectedValue, actualValue);
        }

        [Fact]
        public void Insert_Returns_New_Page_That_Contains_N_Inserted_Elements()
        {
            var size = 10;
            var page = DataPage<int, int>.Empty(size);
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
            var page = DataPage<int, int>.Empty(size);
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
            var page = DataPage<int, int>.Empty(size);
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
            var page = DataPage<int, int>.Empty(size);
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
            var page = DataPage<int, int>.Empty(size);
            page = page.Insert(1, 1);
            var ex = Assert.Throws<InvalidOperationException>(() => page.Insert(1, 1));
            Assert.Contains("1", ex.Message);
        }

        [Fact]
        public void Fork_Creates_Shallow_Copy_Correct_Order()
        {
            var size = 10;
            var expectedValue = new object();
            var page = DataPage<int, object>.Empty(size);
            page = page.Insert(1, expectedValue);
            var fork = page.Fork();
            Assert.False(page == fork); // have to use == instead of Assert.Equal because of implementing IComparable<IPage>
            Assert.True(fork.TryRead(1, out var actualValue));
            Assert.Equal(expectedValue, actualValue);
        }

        [Fact]
        public void Delete_Returns_New_Page_With_Key_Removed()
        {
            var size = 10;
            var page = DataPage<int, int>.Empty(size);
            for (var i = 0; i < size; ++i)
            {
                page = page.Insert(i, i);
            }

            page = page.Delete(5);
            Assert.False(page.ContainsKey(5));
            Assert.Equal(size - 1, page.Count);
        }

        [Fact]
        public void Delete_Returns_New_Page_With_Keys_And_Values_Aligned()
        {
            var size = 10;
            var page = DataPage<int, int>.Empty(size);
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
            var page = DataPage<int, int>.Empty(size);
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
            var page = DataPage<int, int>.Empty(size);
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
            var page = DataPage<int, int>.Empty(size);
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
            var page = DataPage<int, int>.Empty(size);

            var ex = Assert.Throws<KeyNotFoundException>(() => page.Update(key, key));
#pragma warning disable CA1305 // Specify IFormatProvider
            Assert.Contains(key.ToString(), ex.Message);
#pragma warning restore CA1305 // Specify IFormatProvider
        }

        [Fact]
        public void Split_Returns_New_Pages()
        {
            var size = 10;
            var page = DataPage<int, int>.Empty(size);
            for (var i = 0; i < size; ++i)
            {
                page = page.Insert(i, i);
            }

            var (left, right, pivotKey) = page.Split();

            Assert.NotNull(left);
            Assert.Equal(size / 2, left.Count);

            Assert.NotNull(right);
            Assert.Equal(size / 2, right.Count);

            Assert.Equal(5, pivotKey);
        }

        [Fact]
        public void Split_Pages_Contain_Correct_Key_Subsets()
        {
            var size = 10;
            var page = DataPage<int, int>.Empty(size);
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
            var page = DataPage<int, int>.Empty(size);
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

        [Fact]
        public void Merge_Left_Contains_All_Keys()
        {
            var size = 10;
            var page = DataPage<int, int>.Empty(size);
            for (var i = 0; i < size; ++i)
            {
                page = page.Insert(i, i);
            }

            var (left, right, _) = page.Split();
            var mergedPage = left.Merge(right);

            for (var i = 0; i < size; ++i)
            {
                Assert.True(mergedPage.ContainsKey(i));
            }
        }

        [Fact]
        public void Merge_Right_Contains_All_Keys()
        {
            var size = 10;
            var page = DataPage<int, int>.Empty(size);
            for (var i = 0; i < size; ++i)
            {
                page = page.Insert(i, i);
            }

            var (left, right, _) = page.Split();
            var mergedPage = right.Merge(left);

            for (var i = 0; i < size; ++i)
            {
                Assert.True(mergedPage.ContainsKey(i));
            }
        }
    }
}
