using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using BTrees.Types;
using System.Buffers;
using System.Collections.Immutable;

namespace BTrees.Benchmarks
{
    [RankColumn]
    [SimpleJob(RuntimeMoniker.Net70)]
    [MemoryDiagnoser]
    public class ArrayPoolBenchmark
    {
        internal static class UniqueIdFactory
        {
            public static DbUniqueId[] Generate(
                int length,
                ArrayPool<DbUniqueId> bufferPool)
            {
                var ids = bufferPool.Rent(length); // new DbUniqueId2[length];
                for (var i = 0; i < length; ++i)
                {
                    ids[i] = DbUniqueId.NewId();
                }

                return ids;
            }
        }

        internal static class GuidFactory
        {
            public static Guid[] Generate(int length)
            {
                var ids = new Guid[length];
                for (var i = 0; i < length; ++i)
                {
                    ids[i] = Guid.NewGuid();
                }

                return ids;
            }
        }

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

        [Params(2048, 4096, 8192)]
        //[Params(1, 16, 32, 64, 128, 256, 512, 1024, 2048, 4096, 8192)]
        public int KeyCount { get; set; }
        private static readonly ArrayPool<int> IdBufferPool = ArrayPool<int>.Shared;
        private int[]? values;
        private int[]? keyIndexes;

        //private int[]? array1;
        //private int[]? array2;

        //[GlobalCleanup]
        //public void Cleanup()
        //{
        //    IdBufferPool.Return(this.values ?? throw new InvalidOperationException());
        //}

        [GlobalSetup]
        public void Setup()
        {
            //this.array1 = new int[this.KeyCount];
            //this.array2 = new int[this.KeyCount];
            this.values = RandomIntFactory.Generate(this.KeyCount);
            this.keyIndexes = RandomIntFactory.Generate(this.KeyCount);
        }

        ////[Benchmark]
        //public void SingleNewArrayCopyTo()
        //{
        //    var count = this.KeyCount;
        //    var values = this.values ?? throw new InvalidOperationException();
        //    var keyIndexes = this.keyIndexes ?? throw new InvalidOperationException();
        //    var array = new DbUniqueId[count];

        //    for (var i = 0; i < count; ++i)
        //    {
        //        var keyIndex = keyIndexes[i];
        //        var value = values[keyIndex];

        //        var insertIndex = Array.BinarySearch(array, 0, i, value);
        //        insertIndex = insertIndex > 0 ? insertIndex : ~insertIndex;

        //        Array.Copy(array, insertIndex, array, insertIndex + 1, i - insertIndex);
        //        array[insertIndex] = value;
        //    }
        //}

        //[Benchmark]
        //public void ImmutableArrayBench()
        //{
        //    var count = this.KeyCount;
        //    var values = (this.values ?? throw new InvalidOperationException()).AsSpan();
        //    var keyIndexes = (this.keyIndexes ?? throw new InvalidOperationException()).AsSpan();
        //    var array = ImmutableArray<int>.Empty;

        //    for (var i = 0; i < count; ++i)
        //    {
        //        var value = values[keyIndexes[i]];
        //        var insertIndex = array.BinarySearch(value);
        //        insertIndex = insertIndex > 0 ? insertIndex : ~insertIndex;
        //        array = array.Insert(insertIndex, value);
        //    }
        //}

        ////[Benchmark]
        //public void CopyToSpanBench()
        //{
        //    var count = this.KeyCount;
        //    var values = (this.values ?? throw new InvalidOperationException()).AsSpan();
        //    var keyIndexes = (this.keyIndexes ?? throw new InvalidOperationException()).AsSpan();
        //    var array = new DbUniqueId[count];
        //    var span = new Span<DbUniqueId>(array);

        //    for (var i = 0; i < count; ++i)
        //    {
        //        var keyIndex = keyIndexes[i];
        //        var value = values[keyIndex];

        //        var insertIndex = span[..i].BinarySearch(value);
        //        insertIndex = insertIndex > 0 ? insertIndex : ~insertIndex;

        //        span[insertIndex..i].CopyTo(span[(insertIndex + 1)..]);
        //        span[insertIndex] = value;
        //    }
        //}

        [Benchmark]
        public void SinglePooledSpanCopyToBench()
        {
            var count = this.KeyCount;
            var values = (this.values ?? throw new InvalidOperationException()).AsSpan();
            var keyIndexes = (this.keyIndexes ?? throw new InvalidOperationException()).AsSpan();
            var array = IdBufferPool.Rent(count);
            var span = array.AsSpan();

            for (var i = 0; i < count; ++i)
            {
                var value = values[keyIndexes[i]];
                var insertIndex = span[..i].BinarySearch(value);
                insertIndex = insertIndex > 0 ? insertIndex : ~insertIndex;

                span[insertIndex..i].CopyTo(span[(insertIndex + 1)..]);
                span[insertIndex] = value;
            }

            IdBufferPool.Return(array);
        }

        [Benchmark]
        public void SinglePooledArrayBlockCopyBench()
        {
            var count = this.KeyCount;
            var values = (this.values ?? throw new InvalidOperationException()).AsSpan();
            var keyIndexes = (this.keyIndexes ?? throw new InvalidOperationException()).AsSpan();
            var array = IdBufferPool.Rent(count);
            var span = array.AsSpan();

            for (var i = 0; i < count; ++i)
            {
                var value = values[keyIndexes[i]];
                var insertIndex = span[..i].BinarySearch(value);
                insertIndex = insertIndex > 0 ? insertIndex : ~insertIndex;

                Buffer.BlockCopy(array, insertIndex * sizeof(int), array, (insertIndex + 1) * sizeof(int), i - insertIndex);
                span[insertIndex] = value;
            }

            IdBufferPool.Return(array);
        }

        [Benchmark(Baseline = true)]
        public void SinglePooledArrayCopyToBench()
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


        //        [Benchmark]
        //        public void DualArrayPooledBench()
        //        {
        //            var count = this.KeyCount;
        //            var values = this.values ?? throw new InvalidOperationException();
        //            var keyIndexes = this.keyIndexes ?? throw new InvalidOperationException();
        //            var array1 = IdBufferPool.Rent(count);
        //            var array2 = IdBufferPool.Rent(count);
        //            var source = array1;
        //            var destination = array2;

        //            for (var i = 0; i < count; ++i)
        //            {
        //                var keyIndex = keyIndexes[i];
        //                var value = values[keyIndex];

        //                var insertIndex = Array.BinarySearch(source, 0, i, value);
        //                insertIndex = insertIndex > 0 ? insertIndex : ~insertIndex;

        //                Array.Copy(source, 0, destination, 0, insertIndex);
        //                Array.Copy(source, insertIndex, destination, insertIndex + 1, i - insertIndex);
        //                destination[insertIndex] = value;
        //                (source, destination) = (destination, source);
        //            }

        //            IdBufferPool.Return(array1);
        //            IdBufferPool.Return(array2);
        //        }

        //        [Benchmark]
        //        public void DualSpanPreallocatedArrayBench()
        //        {
        //            var count = this.KeyCount;
        //            var values = (this.values ?? throw new InvalidOperationException()).AsSpan();
        //            var keyIndexes = (this.keyIndexes ?? throw new InvalidOperationException()).AsSpan();

        //            var source = this.array1.AsSpan();
        //            var destination = this.array2.AsSpan();

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
        //                var exchange = source;
        //                source = destination;
        //                destination = exchange;
        //#pragma warning restore IDE0180 // Use tuple to swap values
        //            }
        //        }

        //        [Benchmark]
        //        public void BufferBlockCopyDualPooledArrayBench()
        //        {
        //            var count = this.KeyCount;
        //            var values = (this.values ?? throw new InvalidOperationException()).AsSpan();
        //            var keyIndexes = (this.keyIndexes ?? throw new InvalidOperationException()).AsSpan();

        //            var source = IdBufferPool.Rent(count);
        //            var destination = IdBufferPool.Rent(count);

        //            for (var i = 0; i < count; ++i)
        //            {
        //                var value = values[keyIndexes[i]];

        //                var insertIndex = Array.BinarySearch(source[..i], value);
        //                insertIndex = insertIndex > 0 ? insertIndex : ~insertIndex;

        //                Buffer.BlockCopy(source, 0, destination, 0, insertIndex);
        //                Buffer.BlockCopy(source, insertIndex, destination, insertIndex + 1, i - insertIndex);
        //                destination[insertIndex] = value;

        //                (destination, source) = (source, destination);
        //            }

        //            IdBufferPool.Return(source);
        //            IdBufferPool.Return(destination);
        //        }
    }
}
