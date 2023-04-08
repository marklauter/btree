using BenchmarkDotNet.Attributes;
using System.Buffers;
using System.Collections.Immutable;

namespace BTrees.Benchmarks
{
    [RankColumn]
    [MemoryDiagnoser]
    public class StackAllocBenchmark
    {
        internal static class RandomIntFactory
        {
            public static int[] Generate(int length)
            {
                var random = new Random(length);
                var hashset = new HashSet<int>();
                while (hashset.Count < length)
                {
                    var _ = hashset.Add(random.Next(0, length));
                }

                return hashset.ToArray();
            }
        }

        [Params(16, 32, 64, 128, 256, 512, 2048, 4096, 8192)]
        public int KeyCount { get; set; }
        private static readonly ArrayPool<int> IdBufferPool = ArrayPool<int>.Shared;
        private int[]? values;
        private int[]? keyIndexes;
        //private int[]? preallocatedArray;

        [GlobalSetup]
        public void Setup()
        {
            //this.preallocatedArray = new int[this.KeyCount];
            this.values = RandomIntFactory.Generate(this.KeyCount);
            this.keyIndexes = RandomIntFactory.Generate(this.KeyCount);
        }

        [Benchmark]
        public void Single_Pooled_Array_With_Span_CopyTo()
        {
            var count = this.KeyCount;
            var values = this.values ?? throw new InvalidOperationException();
            var keyIndexes = this.keyIndexes ?? throw new InvalidOperationException();
            var array = IdBufferPool.Rent(count);
            var span = array.AsSpan();

            for (var i = 0; i < count; ++i)
            {
                var value = values[keyIndexes[i]];
                var insertIndex = span[..i].BinarySearch(value);
                insertIndex = insertIndex > 0 ? insertIndex : ~insertIndex;

                Array.Copy(array, insertIndex, array, insertIndex + 1, i - insertIndex);
                array[insertIndex] = value;
            }

            IdBufferPool.Return(array);
        }

        //[Benchmark]
        //public void Dual_Pooled_Array_With_Buffer_BlockCopy()
        //{
        //    var count = this.KeyCount;
        //    var values = (this.values ?? throw new InvalidOperationException()).AsSpan();
        //    var keyIndexes = (this.keyIndexes ?? throw new InvalidOperationException()).AsSpan();

        //    var source = IdBufferPool.Rent(count);
        //    var destination = IdBufferPool.Rent(count);

        //    for (var i = 0; i < count; ++i)
        //    {
        //        var value = values[keyIndexes[i]];
        //        var insertIndex = Array.BinarySearch(source[..i], value);
        //        insertIndex = insertIndex > 0 ? insertIndex : ~insertIndex;

        //        Buffer.BlockCopy(source, 0, destination, 0, insertIndex);
        //        Buffer.BlockCopy(source, insertIndex, destination, insertIndex + 1, i - insertIndex);
        //        destination[insertIndex] = value;

        //        (destination, source) = (source, destination);
        //    }

        //    IdBufferPool.Return(source);
        //    IdBufferPool.Return(destination);
        //}

        //        [Benchmark]
        //        public void Dual_Pooled_Array_With_Array_CopyTo()
        //        {
        //            var count = this.KeyCount;
        //            var values = (this.values ?? throw new InvalidOperationException()).AsSpan();
        //            var keyIndexes = (this.keyIndexes ?? throw new InvalidOperationException()).AsSpan();
        //            var source = IdBufferPool.Rent(count);
        //            var destination = IdBufferPool.Rent(count);

        //            for (var i = 0; i < count; ++i)
        //            {
        //                var value = values[keyIndexes[i]];
        //                var insertIndex = Array.BinarySearch(source, 0, i, value);
        //                insertIndex = insertIndex > 0 ? insertIndex : ~insertIndex;

        //                Array.Copy(source, 0, destination, 0, insertIndex);
        //                Array.Copy(source, insertIndex, destination, insertIndex + 1, i - insertIndex);
        //                destination[insertIndex] = value;
        //                (source, destination) = (destination, source);
        //            }

        //            IdBufferPool.Return(source);
        //            IdBufferPool.Return(destination);
        //        }

        //        [Benchmark]
        //        public int[] Dual_StackAlloced_Span_CopyTo()
        //        {
        //            var count = this.KeyCount;
        //            var values = (this.values ?? throw new InvalidOperationException()).AsSpan();
        //            var keyIndexes = (this.keyIndexes ?? throw new InvalidOperationException()).AsSpan();
        //            Span<int> source = stackalloc int[count];
        //            Span<int> destination = stackalloc int[count];

        //            for (var i = 0; i < count; ++i)
        //            {
        //                var value = values[keyIndexes[i]];

        //                var insertIndex = source[..i].BinarySearch(value);
        //                insertIndex = insertIndex > 0 ? insertIndex : ~insertIndex;

        //                source[..insertIndex]
        //                    .CopyTo(destination);

        //                source[insertIndex..i]
        //                    .CopyTo(destination[(insertIndex + 1)..]);

        //                destination[insertIndex] = value;

        //#pragma warning disable IDE0180 // Use tuple to swap values
        //                var x = source;
        //                source = destination;
        //                destination = x;
        //#pragma warning restore IDE0180 // Use tuple to swap values
        //            }

        //            return source.ToArray();
        //        }

        [Benchmark(Baseline = true)]
        public void Dual_Pooled_Array_AsSpan_With_Span_CopyTo()
        {
            var count = this.KeyCount;
            var values = (this.values ?? throw new InvalidOperationException()).AsSpan();
            var keyIndexes = (this.keyIndexes ?? throw new InvalidOperationException()).AsSpan();
            var array1 = IdBufferPool.Rent(count);
            var array2 = IdBufferPool.Rent(count);
            var source = array1.AsSpan();
            var destination = array2.AsSpan();

            for (var i = 0; i < count; ++i)
            {
                var value = values[keyIndexes[i]];
                var insertIndex = source[..i].BinarySearch(value);
                insertIndex = insertIndex > 0 ? insertIndex : ~insertIndex;

                source[..insertIndex]
                    .CopyTo(destination);

                source[insertIndex..i]
                    .CopyTo(destination[(insertIndex + 1)..]);

                destination[insertIndex] = value;

#pragma warning disable IDE0180 // Use tuple to swap values
                var exchange = source;
                source = destination;
                destination = exchange;
#pragma warning restore IDE0180 // Use tuple to swap values
            }

            IdBufferPool.Return(array1);
            IdBufferPool.Return(array2);
        }

        [Benchmark]
        public void SortedList_Add()
        {
            var count = this.KeyCount;
            var values = (this.values ?? throw new InvalidOperationException()).AsSpan();
            var keyIndexes = (this.keyIndexes ?? throw new InvalidOperationException()).AsSpan();
            var list = new SortedList<int, int>(count);

            for (var i = 0; i < count; ++i)
            {
                var value = values[keyIndexes[i]];
                list.Add(value, 0);
            }
        }
    }
}
