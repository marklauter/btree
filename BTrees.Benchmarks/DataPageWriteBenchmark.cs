using BenchmarkDotNet.Attributes;
using BTrees.Pages;
using BTrees.Types;

namespace BTrees.Benchmarks
{
    [RankColumn]
    [MemoryDiagnoser]
    public class DataPageWriteBenchmark
    {
        internal static class RandomIntFactory
        {
            public static int[] Generate(int length)
            {
                var random = new Random(DateTime.UtcNow.Microsecond);
                var hashset = new HashSet<int>();
                while (hashset.Count < length)
                {
                    var _ = hashset.Add(random.Next(0, length));
                }

                return hashset.ToArray();
            }
        }

        private int[]? values;
        private int[]? keys;

        [Params(2048, 4096, 8192)]
        public int KeyCount { get; set; }

        [GlobalSetup]
        public void Setup()
        {
            this.values = RandomIntFactory.Generate(this.KeyCount);
            this.keys = RandomIntFactory.Generate(this.KeyCount);
        }

        //[Benchmark]
        //public DataPage<DbInt32, DbInt32> DataPage_Insert()
        //{
        //    var count = this.KeyCount;
        //    var values = (this.values ?? throw new InvalidOperationException()).AsSpan();
        //    var keys = (this.keys ?? throw new InvalidOperationException()).AsSpan();
        //    var dp = DataPage<DbInt32, DbInt32>.Empty;

        //    for (var i = 0; i < count; ++i)
        //    {
        //        var key = keys[i];
        //        var value = values[i];
        //        dp = dp.Insert(key, value);
        //    }

        //    return dp;
        //}

        //[Benchmark]
        //public RightOptimizedDataPage<DbInt32, DbInt32> RightOptimizedDataPage_Add_With_Insert()
        //{
        //    var count = this.KeyCount;
        //    var values = (this.values ?? throw new InvalidOperationException()).AsSpan();
        //    var keys = (this.keys ?? throw new InvalidOperationException()).AsSpan();
        //    var dp = new RightOptimizedDataPage<DbInt32, DbInt32>(count);

        //    for (var i = 0; i < count; ++i)
        //    {
        //        var key = keys[i];
        //        var value = values[i];
        //        dp.Add(key, value);
        //    }

        //    return dp;
        //}

        [Benchmark(Baseline = true)]
        public RightOptimizedDataPage<DbInt32, DbInt32> RightOptimizedDataPage_Add_With_Append()
        {
            var count = this.KeyCount;
            var values = (this.values ?? throw new InvalidOperationException()).AsSpan();
            var keys = (this.keys ?? throw new InvalidOperationException()).AsSpan();
            var dp = new RightOptimizedDataPage<DbInt32, DbInt32>(count);

            for (var i = 0; i < count; ++i)
            {
                var _ = keys[i];
                var value = values[i];
                dp.Add(i, value);
            }

            return dp;
        }

        [Benchmark]
        public AppendOnlyDataPage<DbInt32, DbInt32> AppendOnlyDataPage()
        {
            var count = this.KeyCount;
            var values = (this.values ?? throw new InvalidOperationException()).AsSpan();
            var keys = (this.keys ?? throw new InvalidOperationException()).AsSpan();
            var dp = new AppendOnlyDataPage<DbInt32, DbInt32>(count);

            for (var i = 0; i < count; ++i)
            {
                var _ = keys[i];
                var value = values[i];
                dp.Add(i, value);
            }

            return dp;
        }
    }
}
