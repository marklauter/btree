//using BenchmarkDotNet.Attributes;
//using BenchmarkDotNet.Jobs;
//using BTrees.Types;
//using System.Buffers;
//using System.Collections.Immutable;

//namespace BTrees.Benchmarks
//{
//    [SimpleJob(RuntimeMoniker.Net70)]
//    [MemoryDiagnoser]
//    public class DataNodeBenchmark
//    {
//        internal static class UniqueIdFactory
//        {
//            public static DbUniqueId[] Generate(
//                int length,
//                ArrayPool<DbUniqueId> bufferPool)
//            {
//                var ids = bufferPool.Rent(length); // new DbUniqueId[length];
//                for (var i = 0; i < length; ++i)
//                {
//                    ids[i] = DbUniqueId.NewId();
//                }

//                return ids;
//            }
//        }

//        internal static class RandomIntFactory
//        {
//            public static int[] Generate(int length)
//            {
//                var random = new Random(length);
//                var hashset = new HashSet<int>();
//                while (hashset.Count < length)
//                {
//#pragma warning disable IDE0058 // Expression value is never used
//                    hashset.Add(random.Next(0, length));
//#pragma warning restore IDE0058 // Expression value is never used
//                }

//                return hashset.ToArray();
//            }
//        }

//        [Params(1, 16, 32, 64, 128, 256, 512, 1024, 2048, 4096, 8192)]
//        public int KeyCount { get; set; }
//        private readonly ArrayPool<DbUniqueId> idBufferPool = ArrayPool<DbUniqueId>.Shared;
//        private DbUniqueId[]? values;
//        private int[]? keyIndexes;
//        private DbUniqueId[]? copyBenchArray;
//        private DbUniqueId[]? copySpanBenchArray;
//        //private DbUniqueId[]? keys;
//        //private readonly DataNode<DbUniqueId, DbUniqueId> node = DataNode<DbUniqueId, DbUniqueId>.Empty();

//        [GlobalCleanup]
//        public void Cleanup()
//        {
//            this.idBufferPool.Return(this.values ?? throw new InvalidOperationException());
//            this.idBufferPool.Return(this.copyBenchArray ?? throw new InvalidOperationException());
//            this.idBufferPool.Return(this.copySpanBenchArray ?? throw new InvalidOperationException());
//            //this.keys = null;
//        }

//        [GlobalSetup]
//        public void Setup()
//        {
//            //this.keys = UniqueIdFactory.Generate(this.KeyCount);
//            this.values = UniqueIdFactory.Generate(this.KeyCount, this.idBufferPool);
//            this.keyIndexes = RandomIntFactory.Generate(this.KeyCount);
//            this.copyBenchArray = this.idBufferPool.Rent(this.KeyCount);
//            this.copySpanBenchArray = this.idBufferPool.Rent(this.KeyCount);
//        }

//        //[Benchmark]
//        //public void ImmutableArrayInsertBench()
//        //{
//        //    var count = this.KeyCount;
//        //    var values = this.values ?? throw new InvalidOperationException();
//        //    var keyIndexes = this.keyIndexes ?? throw new InvalidOperationException();
//        //    var immutableArray = ImmutableArray<DbUniqueId>.Empty;

//        //    for (var i = 0; i < count; ++i)
//        //    {
//        //        var keyIndex = keyIndexes[i];
//        //        var value = values[keyIndex];
//        //        var insertIndex = immutableArray.BinarySearch(value);
//        //        insertIndex = insertIndex > 0 ? insertIndex : ~insertIndex;
//        //        immutableArray = immutableArray.Insert(insertIndex, value);
//        //    }
//        //}

//        //[Benchmark]
//        //public void ArrayShiftRightBench()
//        //{
//        //    var count = this.KeyCount;
//        //    var values = this.values ?? throw new InvalidOperationException();
//        //    var keyIndexes = this.keyIndexes ?? throw new InvalidOperationException();
//        //    var array = new DbUniqueId[count];

//        //    for (var i = 0; i < count; ++i)
//        //    {
//        //        var keyIndex = keyIndexes[i];
//        //        var value = values[keyIndex];
//        //        var insertIndex = Array.BinarySearch(array, 0, i, value);
//        //        insertIndex = insertIndex > 0 ? insertIndex : ~insertIndex;
//        //        for (var shift = i - 1; shift >= insertIndex; --shift)
//        //        {
//        //            array[shift + 1] = array[shift];
//        //        }

//        //        array[insertIndex] = value;
//        //    }
//        //}

//        [Benchmark(Baseline = true)]
//        public void CopyBench()
//        {
//            var count = this.KeyCount;
//            var values = this.values ?? throw new InvalidOperationException();
//            var keyIndexes = this.keyIndexes ?? throw new InvalidOperationException();
//            var array = this.copyBenchArray ?? throw new InvalidOperationException();

//            for (var i = 0; i < count; ++i)
//            {
//                var keyIndex = keyIndexes[i];
//                var value = values[keyIndex];

//                var insertIndex = Array.BinarySearch(array, 0, i, value);
//                insertIndex = insertIndex > 0 ? insertIndex : ~insertIndex;

//                Array.Copy(array, insertIndex, array, insertIndex + 1, i - insertIndex);
//                array[insertIndex] = value;
//            }
//        }

//        //[Benchmark]
//        //public void DualArrayBench()
//        //{
//        //    var count = this.KeyCount;
//        //    var values = this.values ?? throw new InvalidOperationException();
//        //    var keyIndexes = this.keyIndexes ?? throw new InvalidOperationException();
//        //    var array1 = new DbUniqueId[count];
//        //    var array2 = new DbUniqueId[count];
//        //    var current = 1;
//        //    var source = array1;
//        //    DbUniqueId[] destination;

//        //    for (var i = 0; i < count; ++i)
//        //    {
//        //        var keyIndex = keyIndexes[i];
//        //        var value = values[keyIndex];

//        //        var insertIndex = Array.BinarySearch(source, 0, i, value);
//        //        insertIndex = insertIndex > 0 ? insertIndex : ~insertIndex;

//        //        if (current == 1)
//        //        {
//        //            source = array1;
//        //            destination = array2;
//        //            current = 2;
//        //        }
//        //        else
//        //        {
//        //            source = array2;
//        //            destination = array1;
//        //            current = 1;
//        //        }

//        //        Array.Copy(source, 0, destination, 0, insertIndex);
//        //        Array.Copy(source, insertIndex, destination, insertIndex + 1, i - insertIndex);
//        //        destination[insertIndex] = value;
//        //        source = destination;
//        //    }
//        //}

//        //[Benchmark]
//        //public void DualSpanBench()
//        //{
//        //    var count = this.KeyCount;
//        //    var values = this.values ?? throw new InvalidOperationException();
//        //    var keyIndexes = this.keyIndexes ?? throw new InvalidOperationException();
//        //    var array1 = new Span<DbUniqueId>(new DbUniqueId[count]);
//        //    var array2 = new Span<DbUniqueId>(new DbUniqueId[count]);
//        //    var current = 1;
//        //    var source = array1;
//        //    Span<DbUniqueId> destination;

//        //    for (var i = 0; i < count; ++i)
//        //    {
//        //        var keyIndex = keyIndexes[i];
//        //        var value = values[keyIndex];

//        //        var insertIndex = source[..i].BinarySearch(value);
//        //        insertIndex = insertIndex > 0 ? insertIndex : ~insertIndex;

//        //        if (current == 1)
//        //        {
//        //            source = array1;
//        //            destination = array2;
//        //            current = 2;
//        //        }
//        //        else
//        //        {
//        //            source = array2;
//        //            destination = array1;
//        //            current = 1;
//        //        }

//        //        source[..insertIndex].CopyTo(destination);
//        //        source[insertIndex..i].CopyTo(destination[(insertIndex + 1)..]);
//        //        destination[insertIndex] = value;
//        //        source = destination;
//        //    }
//        //}

//        [Benchmark]
//        public void CopySpanBench()
//        {
//            var count = this.KeyCount;
//            var values = this.values ?? throw new InvalidOperationException();
//            var keyIndexes = this.keyIndexes ?? throw new InvalidOperationException();
//            var span = new Span<DbUniqueId>(this.copySpanBenchArray ?? throw new InvalidOperationException());

//            for (var i = 0; i < count; ++i)
//            {
//                var keyIndex = keyIndexes[i];
//                var value = values[keyIndex];

//                var insertIndex = span[..i].BinarySearch(value);
//                insertIndex = insertIndex > 0 ? insertIndex : ~insertIndex;

//                span[insertIndex..i].CopyTo(span[(insertIndex + 1)..]);
//                span[insertIndex] = value;
//            }
//        }

//        //[Benchmark]
//        //public void ImmutableSortedSetBench()
//        //{
//        //    var count = this.KeyCount;
//        //    var values = this.values ?? throw new InvalidOperationException();
//        //    var keyIndexes = this.keyIndexes ?? throw new InvalidOperationException();
//        //    var sortedSet = ImmutableSortedSet<DbUniqueId>.Empty;

//        //    for (var i = 0; i < count; ++i)
//        //    {
//        //        var keyIndex = keyIndexes[i];
//        //        var value = values[keyIndex];
//        //        sortedSet = sortedSet.Add(value);
//        //    }
//        //}

//        //[Benchmark]
//        //public void NewGuid()
//        //{
//        //    var guid = Guid.NewGuid();
//        //    var uniqueId = (DbUniqueId)guid;
//        //    if (uniqueId.Value != guid)
//        //    {
//        //        throw new InvalidCastException();
//        //    }
//        //}

//        //[Benchmark]
//        //public void InsertOne()
//        //{
//        //    this.node.Insert(Guid.NewGuid(), Guid.NewGuid());
//        //}

//        //[Benchmark]
//        //public void InsertMany()
//        //{
//        //    var node = DataNode<DbUniqueId, DbUniqueId>.Empty();
//        //    var values = this.values ?? throw new InvalidOperationException();
//        //    var keys = this.keys ?? throw new InvalidOperationException();
//        //    var count = this.KeyCount;

//        //    for (var key = 0; key < count; ++key)
//        //    {
//        //        node.Insert(keys[key], values[key]);
//        //    }

//        //    //Assert.Equal(keyCount + 1, node.Count());

//        //    //for (var key = 0; key < keyCount; ++key)
//        //    //{
//        //    //    Assert.True(node.ContainsKey(key));
//        //    //}

//        //    //for (var key = 0; key < keyCount; ++key)
//        //    //{
//        //    //    var values = node.Read(key);
//        //    //    Assert.Contains(Ids[key], values);
//        //    //}
//        //}
//    }
//}
