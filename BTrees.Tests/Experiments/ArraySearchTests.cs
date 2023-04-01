using BTrees.Types;
using System.Buffers;
using System.Collections.Immutable;

namespace BTrees.Tests.Experiments
{
    public class ArraySearchTests
    {
        [Fact]
        public void BinarySearchNotFoundOnEmptyArray()
        {
            var i = Array.Empty<int>();
            var key = 5;
            var index = Array.BinarySearch(i, key);
            Assert.True(index < 0);
            Assert.Equal(-1, index);
            Assert.Equal(0, ~index);
        }

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

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(25)]
        [InlineData(50)]
        [InlineData(100)]
        [InlineData(250)]
        public void RandomIntFactoryTest(int count)
        {
            var ids = RandomIntFactory.Generate(count);
            Assert.Equal(count, ids.Length);

            var values = UniqueIdFactory.Generate(count);
            Assert.Equal(count, values.Length);

            for (var i = 0; i < count; ++i)
            {
                var valueIndex = ids[i];

                _ = values[valueIndex];
            }
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(25)]
        [InlineData(50)]
        [InlineData(100)]
        [InlineData(250)]
        public void ArrayShiftRightBench(int count)
        {
            var values = UniqueIdFactory.Generate(count);
            var keyIndexes = RandomIntFactory.Generate(count);
            var array = new DbUniqueId[count];

            for (var i = 0; i < count; ++i)
            {
                var keyIndex = keyIndexes[i];
                var value = values[keyIndex];
                var insertIndex = Array.BinarySearch(array, 0, i, value);
                insertIndex = insertIndex > 0 ? insertIndex : ~insertIndex;
                for (var shift = i - 1; shift >= insertIndex; --shift)
                {
                    array[shift + 1] = array[shift];
                }

                array[insertIndex] = value;
            }
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(25)]
        [InlineData(50)]
        [InlineData(100)]
        [InlineData(250)]
        public void DualArrayBench(int count)
        {
            var values = UniqueIdFactory.Generate(count);
            var keyIndexes = RandomIntFactory.Generate(count);
            var array1 = new DbUniqueId[count];
            var array2 = new DbUniqueId[count];
            var current = 1;
            var source = array1;
            DbUniqueId[] destination;
            var iarray = ImmutableArray<DbUniqueId>.Empty;

            for (var i = 0; i < count; ++i)
            {
                var keyIndex = keyIndexes[i];
                var value = values[keyIndex];

                var insertIndex = Array.BinarySearch(source, 0, i, value);
                insertIndex = insertIndex > 0 ? insertIndex : ~insertIndex;

                iarray = iarray.Insert(insertIndex, value);

                if (current == 1)
                {
                    source = array1;
                    destination = array2;
                    current = 2;
                }
                else
                {
                    source = array2;
                    destination = array1;
                    current = 1;
                }

                Array.Copy(source, 0, destination, 0, insertIndex);
                Array.Copy(source, insertIndex, destination, insertIndex + 1, i - insertIndex);
                destination[insertIndex] = value;
                source = destination;

                Assert.Equal(iarray, source.AsSpan(0, i + 1).ToArray());
            }
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(25)]
        [InlineData(50)]
        [InlineData(100)]
        [InlineData(250)]
        public void DualSpanBench(int count)
        {
            var values = UniqueIdFactory.Generate(count);
            var keyIndexes = RandomIntFactory.Generate(count);
            var array1 = new Span<DbUniqueId>(new DbUniqueId[count]);
            var array2 = new Span<DbUniqueId>(new DbUniqueId[count]);
            var current = 1;
            var source = array1;
            Span<DbUniqueId> destination;
            var iarray = ImmutableArray<DbUniqueId>.Empty;

            for (var i = 0; i < count; ++i)
            {
                var keyIndex = keyIndexes[i];
                var value = values[keyIndex];

                var insertIndex = source[..i].BinarySearch(value);
                insertIndex = insertIndex > 0 ? insertIndex : ~insertIndex;

                iarray = iarray.Insert(insertIndex, value);

                if (current == 1)
                {
                    source = array1;
                    destination = array2;
                    current = 2;
                }
                else
                {
                    source = array2;
                    destination = array1;
                    current = 1;
                }

                source[..insertIndex].CopyTo(destination);
                source[insertIndex..i].CopyTo(destination[(insertIndex + 1)..]);
                destination[insertIndex] = value;
                source = destination;

                Assert.Equal(iarray, source[..(i + 1)].ToArray());
            }
        }

        [Theory]
        [InlineData(50)]
        [InlineData(100)]
        [InlineData(250)]
        public void SingleSpan(int count)
        {
            var values = RandomIntFactory.Generate(count).AsSpan();
            var sortedArray = ArrayPool<int>.Shared.Rent(count);
            var sorted = sortedArray.AsSpan();
            var ima = ImmutableArray<int>.Empty;

            for (var i = 0; i < count; ++i)
            {
                var value = values[i];

                var key = sorted[..i].BinarySearch(value);
                key = key > 0 ? key : ~key;

                ima = ima.Insert(key, value);

                sorted[key..i]
                    .CopyTo(sorted[(key + 1)..(i + 1)]);
                sorted[key] = value;

                Assert.Equal(ima, sorted[..(i + 1)].ToArray());
            }

            ArrayPool<int>.Shared.Return(sortedArray);
        }
    }
}
