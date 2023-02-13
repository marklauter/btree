//using BTrees.Pages;
//using System.Collections.Immutable;

//namespace BTrees.Tests
//{
//    public sealed class PartitionPageTests
//    {
//        [Fact]
//        public void New_Page_Has_Correct_Size()
//        {
//            var size = 10;
//            var leafPage = DataPage<int, int>.Empty(size);
//            for (var i = 0; i < size; ++i)
//            {
//                leafPage = leafPage.Insert(i, i);
//            }

//            var (leftPage, rightPage, _) = leafPage.Split();
//            var pivotPage = PartitionPage<int, int>.Create(size, leftPage, rightPage);
//            Assert.Equal(size, pivotPage.Size);
//        }

//        [Fact]
//        public void New_Page_Has_Correct_Count()
//        {
//            var size = 10;
//            var leafPage = DataPage<int, int>.Empty(size);
//            for (var i = 0; i < size; ++i)
//            {
//                leafPage = leafPage.Insert(i, i);
//            }

//            var (leftPage, rightPage, _) = leafPage.Split();
//            var pivotPage = PartitionPage<int, int>.Create(size, leftPage, rightPage);
//            Assert.Equal(1, pivotPage.Count);
//        }

//        [Fact]
//        public void Insert_Returns_New_Page()
//        {
//            var size = 10;
//            var leafPage = DataPage<int, int>.Empty(size);
//            for (var i = 0; i < size; ++i)
//            {
//                leafPage = leafPage.Insert(i, i);
//            }

//            var (leftPage, rightPage, _) = leafPage.Split();
//            var pivotPage = PartitionPage<int, int>.Create(size, leftPage, rightPage);

//            var newPage = pivotPage.Insert(size + 1, size + 1);
//            Assert.False(pivotPage == newPage);
//        }

//        [Fact]
//        public void Insert_Not_Resulting_In_Subtree_Overflow_Returns_New_Page_With_Count_Elements()
//        {
//            var size = 10;
//            var leafPage = DataPage<int, int>.Empty(size);
//            for (var i = 0; i < size; ++i)
//            {
//                leafPage = leafPage.Insert(i, i);
//            }

//            var (leftPage, rightPage, _) = leafPage.Split();
//            var pivotPage = PartitionPage<int, int>.Create(size, leftPage, rightPage);

//            var newPage = pivotPage.Insert(size + 1, size + 1);
//            Assert.Equal(pivotPage.Count, newPage.Count);
//        }

//        [Fact]
//        public void Insert_Resulting_In_Subtree_Overflow_Returns_New_Page_With_Count_Plus_One_Elements()
//        {
//            var size = 10;
//            var leafPage = DataPage<int, int>.Empty(size);
//            for (var i = 0; i < size * 2; ++i)
//            {
//                leafPage = leafPage.Insert(i, i);
//            }

//            var (leftPage, rightPage, _) = leafPage.Split();
//            Assert.Equal(10, leftPage.Count);
//            Assert.Equal(10, rightPage.Count);

//            var pivotPage = PartitionPage<int, int>.Create(size, leftPage, rightPage);

//            var newPage = pivotPage.Insert(size * 2, size * 2);
//            Assert.Equal(pivotPage.Count + 1, newPage.Count);
//        }

//        [Fact]
//        public void Insert_Returns_New_Page_That_Contains_All_Inserted_Keys()
//        {
//            var size = 10;
//            var leafPage = DataPage<int, int>.Empty(size);
//            for (var i = 0; i < size; ++i)
//            {
//                leafPage = leafPage.Insert(i, i);
//            }

//            var (leftPage, rightPage, _) = leafPage.Split();
//            var pivotPage = PartitionPage<int, int>.Create(size, leftPage, rightPage);

//            var newPage = pivotPage.Insert(size, size);

//            for (var i = 0; i < size + 1; ++i)
//            {
//                Assert.True(newPage.ContainsKey(i));
//            }
//        }

//        [Fact]
//        public void Insert_Returns_New_Page_That_Contains_All_Inserted_Values()
//        {
//            var size = 10;
//            var leafPage = DataPage<int, int>.Empty(size);
//            for (var i = 0; i < size; ++i)
//            {
//                leafPage = leafPage.Insert(i, i);
//            }

//            var (leftPage, rightPage, _) = leafPage.Split();
//            var pivotPage = PartitionPage<int, int>.Create(size, leftPage, rightPage);

//            var newPage = pivotPage.Insert(size, size);

//            for (var i = 0; i < size + 1; ++i)
//            {
//                Assert.True(newPage.TryRead(i, out var value));
//                Assert.Equal(i, value);
//            }
//        }

//        [Fact]
//        public void Fork_Creates_Shallow_Copy_Correct_Order()
//        {
//            var size = 10;
//            var leafPage = DataPage<int, int>.Empty(size);
//            for (var i = 0; i < size; ++i)
//            {
//                leafPage = leafPage.Insert(i, i);
//            }

//            var (leftPage, rightPage, _) = leafPage.Split();
//            var pivotPage = PartitionPage<int, int>.Create(size, leftPage, rightPage);

//            var fork = pivotPage.Fork();
//            Assert.False(pivotPage == fork);

//            for (var i = 0; i < size; ++i)
//            {
//                Assert.True(fork.TryRead(i, out var value));
//                Assert.Equal(i, value);
//            }
//        }

//        [Fact]
//        public void Delete_Returns_New_Page_With_Key_Removed()
//        {
//            var size = 10;
//            var leafPage = DataPage<int, int>.Empty(size);
//            for (var i = 0; i < size * 2; ++i)
//            {
//                leafPage = leafPage.Insert(i, i);
//            }

//            var (leftPage, rightPage, _) = leafPage.Split();
//            var pivotPage = PartitionPage<int, int>.Create(size, leftPage, rightPage);

//            var deletedPage = pivotPage.Delete(5);
//            Assert.False(deletedPage.ContainsKey(5));
//        }

//        [Fact]
//        public void Split_Works()
//        {
//            var size = 10;
//            var keys = new List<int>(size * size);
//            var pages = new List<IPage<int, int>>(size * size);
//            for (var i = 0; i < size + 1; ++i)
//            {
//                var key = i * size;
//                var items = Enumerable.Range(key, size);
//                keys.Add(key);
//                pages.Add(DataPage<int, int>.Create(size, items.ToImmutableArray(), items.ToImmutableArray()));
//            }

//            var pivotPage = PartitionPage<int, int>.Create(
//                size,
//                keys.Skip(1).Take(keys.Count - 1).ToImmutableArray(),
//                pages.ToImmutableArray());

//            Assert.Equal(size, pivotPage.Count);

//            var (leftPage, rightPage, pivotKey) = pivotPage.Split();

//            Assert.Equal(60, pivotKey);

//            Assert.Equal(size / 2, leftPage.Count);
//            Assert.Equal(0, leftPage.MinKey);
//            Assert.Equal(59, leftPage.MaxKey);

//            Assert.Equal(size / 2 - 1, rightPage.Count);
//            Assert.Equal(60, rightPage.MinKey);
//            Assert.Equal(109, rightPage.MaxKey);
//        }

//        [Fact]
//        public void Insert_Splits_Subtrees()
//        {
//            var size = 4;
//            var leafPage = DataPage<int, int>.Empty(size);
//            for (var i = 0; i < size; ++i)
//            {
//                leafPage = leafPage.Insert(i, i);
//            }

//            var (leftPage, rightPage, _) = leafPage.Split();
//            var pivotPage = PartitionPage<int, int>.Create(
//                size,
//                leftPage,
//                rightPage);

//            for (var i = size; i < size + 100; ++i)
//            {
//                pivotPage = pivotPage.Insert(i, i);
//            }

//            Assert.Equal(50, pivotPage.Count);
//        }
//    }
//}
