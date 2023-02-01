namespace Leaf.Tests
{
    public class LeafTest
    {
        [Fact]
        public void SplitCreatesCorrectRightAndLeftPages()
        {
            var keys = new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            var values = new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            var leaf = new LeafNode<int, int>(10, keys, values);

            var (leftPage, rightPage) = leaf.Split();

            Assert.Null(leftPage.LeftSibling);
            Assert.Equal(rightPage, leftPage.RightSibling);
            Assert.Null(rightPage.RightSibling);
            Assert.Equal(leftPage, rightPage.LeftSibling);

            Assert.Equal(0, leftPage.PivotKey);
            Assert.Equal(5, rightPage.PivotKey);

            Assert.Equal(5, leftPage.Count);
            Assert.Equal(5, rightPage.Count);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        [InlineData(5)]
        [InlineData(6)]
        [InlineData(7)]
        [InlineData(8)]
        [InlineData(9)]
        public void BinarySearchVsIndexOfWithMatches(int key)
        {
            var keys = new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            var expected = Array.BinarySearch(keys, key);
            Assert.Equal(key, expected);

            var actual = this.IndexOfKey(keys, key);
            Assert.Equal(expected, actual);
        }

        [Fact]
        //        [InlineData(4)]
        //[InlineData(10)]
        public void BinarySearchVsIndexOfWithoutMatchesMOne()
        {
            var key = -1;
            var keys = new int[] { 0, 1, 2, 3, 5, 6, 7, 8, 9 };
            var expected = Array.BinarySearch(keys, key);
            Assert.Equal(~0, expected);

            var actual = this.IndexOfKey(keys, key);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void BinarySearchVsIndexOfWithoutMatchesFour()
        {
            var key = 4;
            var keys = new int[] { 0, 1, 2, 3, 5, 6, 7, 8, 9 };
            var expected = Array.BinarySearch(keys, key);
            Assert.Equal(~4, expected);

            var actual = this.IndexOfKey(keys, key);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void BinarySearchVsIndexOfWithoutMatchesTen()
        {
            var key = 10;
            var keys = new int[] { 0, 1, 2, 3, 5, 6, 7, 8, 9 };
            var expected = Array.BinarySearch(keys, key);
            Assert.Equal(~keys.Length, expected);

            var actual = this.IndexOfKey(keys, key);
            Assert.Equal(expected, actual);
        }

        internal int IndexOfKey<TKey>(TKey[] keys, TKey key)
            where TKey : IComparable<TKey>
        {
            // _ = this.Keys.BinarySearch(key);

            var low = 0;
            var high = keys.Length - 1;

            while (low <= high)
            {
                var middle = (low + high) / 2;

                var comparison = keys[middle].CompareTo(key);

                if (comparison == 0)
                {
                    return middle;
                }

                if (comparison < 0)
                {
                    low = middle + 1;
                    continue;
                }

                high = middle - 1;
                continue;
            }

            return ~low;
        }
    }
}
