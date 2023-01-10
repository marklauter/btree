namespace BTrees.Tests
{
    public class ArraySearchTests
    {
        [Fact]
        public void BinarySearchNotFoundBoudingRight()
        {
            var i = new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, };
            var key = 5;
            var index = Array.BinarySearch(i, key);
            Assert.True(index < 0);
            Assert.Equal(~10, index);
        }

        [Fact]
        public void BinarySearchNotFoundBoudingLeft()
        {
            var i = new int[] { 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, };
            var key = 1;
            var index = Array.BinarySearch(i, key);
            Assert.True(index < 0);
            Assert.Equal(~0, index);
        }

        [Fact]
        public void BinarySearchNotFoundMid()
        {
            var i = new int[] { 1, 2, 3, 4, 6, 7, 8, 9, 10, 11, };
            var key = 5;
            var index = Array.BinarySearch(i, key);
            Assert.Equal(~4, index);
        }
    }
}
