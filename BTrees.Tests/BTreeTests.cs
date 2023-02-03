//namespace BTrees.Tests
//{
//    public class BTreeTests
//    {
//        private readonly int pageSize = 5;

//        [Theory]
//        [InlineData(1)]
//        [InlineData(2)]
//        [InlineData(5)]
//        [InlineData(7)]
//        [InlineData(13)]
//        public void SplitTest(int rndSeed)
//        {
//            var random = new Random(rndSeed);
//            using var tree = new BTree<int, int>(this.pageSize);
//            var keys = new int[this.pageSize * 100];
//            for (var i = 0; i < this.pageSize * 100; ++i)
//            {
//                var kv = random.Next(this.pageSize * 10);
//                var writeSuccess = tree.TryWrite(kv, kv);
//                Assert.True(writeSuccess);
//                keys[i] = kv;
//            }

//            Assert.Equal(keys.Distinct().Count(), tree.Count);
//            var bestCaseHeight = (int)(Math.Log(tree.Count, this.pageSize + 1) + 1);
//            var worstCaseHeight = (int)(Math.Log(tree.Count, (this.pageSize + 1) / 2) + 1);
//            Assert.True(tree.Height >= bestCaseHeight && tree.Height <= worstCaseHeight, $"count: {tree.Count}, best: {bestCaseHeight}, worst: {worstCaseHeight}, actual: {tree.Height}");
//        }

//        [Fact]
//        public void RightOnlyInsertTest()
//        {
//            using var tree = new BTree<int, int>(this.pageSize);
//            for (var i = 0; i < this.pageSize * 10; ++i)
//            {
//                var writeSuccess = tree.TryWrite(i + 1, i + 1);
//                Assert.True(writeSuccess);
//            }

//            Assert.Equal(this.pageSize * 10, tree.Count);
//            Assert.Equal(3, tree.Height);
//        }

//        public static int BinarySearch(int[] array, int target)
//        {
//            var left = 0;
//            var right = array.Length - 1;

//            while (left <= right)
//            {
//                var middle = (left + right) / 2;
//                if (array[middle] == target)
//                {
//                    return middle;
//                }
//                else if (array[middle] < target)
//                {
//                    left = middle + 1;
//                }
//                else
//                {
//                    right = middle - 1;
//                }
//            }

//            return ~left;
//        }

//        [Fact]
//        public void ArrayBinarySearch()
//        {
//            var numbers = new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, };
//            for (var i = 0; i < numbers.Length; ++i)
//            {
//                var index = Array.BinarySearch<int>(numbers, i);
//                Assert.Equal(i, index);
//                Assert.True(index >= 0);
//            }
//        }

//        [Fact]
//        public void ArrayBinarySearchFindsInsertionPointOnLeft()
//        {
//            var numbers = new int[] { 0, 1, 2, 4, 5, 6, 7, 8, 9, 10, };

//            var index = Array.BinarySearch<int>(numbers, 3);
//            Assert.True(index < 0);
//            Assert.Equal(3, ~index);
//        }

//        [Fact]
//        public void ArrayBinarySearchFindsInsertionPointOnRight()
//        {
//            var numbers = new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, };

//            var index = Array.BinarySearch<int>(numbers, 11);
//            Assert.True(index < 0);
//            Assert.Equal(11, ~index);
//        }

//        [Fact]
//        public void BinarySearchFindsIndex()
//        {
//            var numbers = new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, };

//            for (var i = 0; i < numbers.Length; ++i)
//            {
//                var index = BinarySearch(numbers, i);
//                Assert.Equal(i, index);
//                Assert.True(index >= 0);
//            }
//        }

//        [Fact]
//        public void BinarySearchFindsInsertionPointOnLeft()
//        {
//            var numbers = new int[] { 0, 1, 2, 4, 5, 6, 7, 8, 9, 10, };

//            var index = BinarySearch(numbers, 3);
//            Assert.True(index < 0);
//            Assert.Equal(3, ~index);
//        }

//        [Fact]
//        public void BinarySearchFindsInsertionPointOnRight()
//        {
//            var numbers = new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 10, };

//            var index = BinarySearch(numbers, 9);
//            Assert.True(index < 0);
//            Assert.Equal(9, ~index);
//        }

//        [Fact]
//        public void ReadReturnsValueForKey()
//        {
//            using var tree = new BTree<int, int>(this.pageSize);
//            for (var i = 0; i < this.pageSize * 5; ++i)
//            {
//                var writeSuccess = tree.TryWrite(i + 1, i + 1);
//                Assert.True(writeSuccess);
//            }

//            var found = tree.TryRead(1, out var value);
//            Assert.True(found);
//            Assert.Equal(1, value);
//        }

//        [Fact]
//        public void ReadReturnsFalseWhenKeyIsNotFound()
//        {
//            using var tree = new BTree<int, int>(this.pageSize);
//            for (var i = 0; i < this.pageSize * 5; ++i)
//            {
//                var writeSuccess = tree.TryWrite(i + 1, i + 1);
//                Assert.True(writeSuccess);
//            }

//            var found = tree.TryRead(this.pageSize * 10, out _);
//            Assert.False(found);
//        }

//        [Fact]
//        public void DeleteMergesUnderFlowLeaves()
//        {
//            using var tree = new BTree<int, int>(this.pageSize);
//            for (var i = 0; i < this.pageSize * 3; ++i)
//            {
//                var writeSuccess = tree.TryWrite(i + 1, i + 1);
//                Assert.True(writeSuccess);
//            }

//            Assert.Equal(2, tree.Height);

//            for (var i = 0; i < this.pageSize * 3 - 1; ++i)
//            {
//                Assert.True(tree.TryDelete(i + 1), $"delete i: {i + 1}");
//            }

//            Assert.Equal(1, tree.Height);
//        }

//        [Theory]
//        [InlineData(1)]
//        [InlineData(2)]
//        [InlineData(5)]
//        [InlineData(7)]
//        [InlineData(13)]
//        public void WriteAndRead(int rndSeed)
//        {
//            var localPageSize = 4096;
//            using var tree = new BTree<int, int>(localPageSize);
//            var random = new Random(rndSeed);
//            var maxRnd = localPageSize * 2;

//            for (var i = 0; i < localPageSize * 10; ++i)
//            {
//                var key = random.Next(maxRnd);
//                var expectedValue = random.Next(maxRnd);
//                var writeSuccess = tree.TryWrite(key, expectedValue);
//                Assert.True(writeSuccess);
//                Assert.True(tree.TryRead(key, out var actualValue), $"count: {tree.Count}, kvp: ({key}, {actualValue})");
//                Assert.Equal(expectedValue, actualValue);
//                var bestCaseHeight = (int)(Math.Log(tree.Count, localPageSize + 1) + 1);
//                var worstCaseHeight = (int)(Math.Log(tree.Count, (localPageSize + 1) / 2) + 1);
//                Assert.True(tree.Height >= bestCaseHeight && tree.Height <= worstCaseHeight, $"count: {tree.Count}, best: {bestCaseHeight}, worst: {worstCaseHeight}, actual: {tree.Height}");
//            }
//        }
//    }
//}
