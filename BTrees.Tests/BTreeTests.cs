namespace BTrees.Tests
{
    public class BTreeTests
    {
        private readonly int pageSize = 5;

        [Fact]
        public void SplitTest()
        {
            var tree = new BTree<int, int>(this.pageSize);
            for (var i = 0; i < this.pageSize * 10; ++i)
            {
                tree.Insert(i + 1, i + 1);
            }

            Assert.Equal(this.pageSize * 10, tree.Count);
            Assert.Equal(4, tree.Degree);
        }

        public static int BinarySearch(int[] array, int target)
        {
            var left = 0;
            var right = array.Length - 1;

            while (left <= right)
            {
                var middle = (left + right) / 2;
                if (array[middle] == target)
                {
                    return middle;
                }
                else if (array[middle] < target)
                {
                    left = middle + 1;
                }
                else
                {
                    right = middle - 1;
                }
            }

            return ~left;
        }

        [Fact]
        public void ArrayBinarySearch()
        {
            var numbers = new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, };
            for (var i = 0; i < numbers.Length; ++i)
            {
                var index = Array.BinarySearch<int>(numbers, i);
                Assert.Equal(i, index);
                Assert.True(index >= 0);
            }
        }

        [Fact]
        public void ArrayBinarySearchFindsInsertionPointOnLeft()
        {
            var numbers = new int[] { 0, 1, 2, 4, 5, 6, 7, 8, 9, 10, };

            var index = Array.BinarySearch<int>(numbers, 3);
            Assert.True(index < 0);
            Assert.Equal(3, ~index);
        }

        [Fact]
        public void ArrayBinarySearchFindsInsertionPointOnRight()
        {
            var numbers = new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, };

            var index = Array.BinarySearch<int>(numbers, 11);
            Assert.True(index < 0);
            Assert.Equal(11, ~index);
        }

        [Fact]
        public void BinarySearchFindsIndex()
        {
            var numbers = new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, };

            for (var i = 0; i < numbers.Length; ++i)
            {
                var index = BinarySearch(numbers, i);
                Assert.Equal(i, index);
                Assert.True(index >= 0);
            }
        }

        [Fact]
        public void BinarySearchFindsInsertionPointOnLeft()
        {
            var numbers = new int[] { 0, 1, 2, 4, 5, 6, 7, 8, 9, 10, };

            var index = BinarySearch(numbers, 3);
            Assert.True(index < 0);
            Assert.Equal(3, ~index);
        }

        [Fact]
        public void BinarySearchFindsInsertionPointOnRight()
        {
            var numbers = new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 10, };

            var index = BinarySearch(numbers, 9);
            Assert.True(index < 0);
            Assert.Equal(9, ~index);
        }

        [Fact]
        public void ReadReturnsValueForKey()
        {
            var tree = new BTree<int, int>(this.pageSize);
            for (var i = 0; i < this.pageSize * 5; ++i)
            {
                tree.Insert(i + 1, i + 1);
            }

            var found = tree.TryRead(1, out var value);
            Assert.True(found);
            Assert.Equal(1, value);
        }

        [Fact]
        public void ReadReturnsFalseWhenKeyIsNotFound()
        {
            var tree = new BTree<int, int>(this.pageSize);
            for (var i = 0; i < this.pageSize * 5; ++i)
            {
                tree.Insert(i + 1, i + 1);
            }

            var found = tree.TryRead(this.pageSize * 10, out _);
            Assert.False(found);
        }

        [Fact]
        public void DeleteMergesUnderFlowLeaves()
        {
            var tree = new BTree<int, int>(this.pageSize);
            for (var i = 0; i < this.pageSize * 3; ++i)
            {
                tree.Insert(i + 1, i + 1);
            }

            Assert.Equal(2, tree.Degree);

            for (var i = 0; i < this.pageSize * 3 - 1; ++i)
            {
                Assert.True(tree.TryDelete(i + 1), $"delete i: {i + 1}");
            }

            Assert.Equal(1, tree.Degree);
        }
    }
}
