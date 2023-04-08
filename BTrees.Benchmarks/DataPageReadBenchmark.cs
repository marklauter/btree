using BenchmarkDotNet.Attributes;
using BTrees.Pages;
using BTrees.Types;

namespace BTrees.Benchmarks
{
    [RankColumn]
    [MemoryDiagnoser]
    public class DataPageReadBenchmark
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

        private RightOptimizedDataPage<DbInt32, DbInt32>? rightOptimizedDataPage;
        private AppendOnlyDataPage<DbInt32, DbInt32>? appendOnlyDataPage;

        [Params(2048, 4096, 8192)]
        public int KeyCount { get; set; }

        [GlobalSetup]
        public void Setup()
        {
            this.values = RandomIntFactory.Generate(this.KeyCount);
            this.rightOptimizedDataPage = this.FillRightOptimizedDataPage();
            this.appendOnlyDataPage = this.FillAppendOnlyDataPage();
        }

        public RightOptimizedDataPage<DbInt32, DbInt32> FillRightOptimizedDataPage()
        {
            var count = this.KeyCount;
            var values = (this.values ?? throw new InvalidOperationException()).AsSpan();
            var dp = new RightOptimizedDataPage<DbInt32, DbInt32>(count);

            for (var i = 0; i < count; ++i)
            {
                dp.Add(i, values[i]);
            }

            return dp;
        }

        public AppendOnlyDataPage<DbInt32, DbInt32> FillAppendOnlyDataPage()
        {
            var count = this.KeyCount;
            var values = (this.values ?? throw new InvalidOperationException()).AsSpan();
            var dp = new AppendOnlyDataPage<DbInt32, DbInt32>(count);

            for (var i = 0; i < count; ++i)
            {
                var value = values[i];
                dp.Add(i, value);
            }

            return dp;
        }

        [Benchmark(Baseline = true)]
        public int RightOptimizedDataPage_ContainsKey()
        {
            var count = this.KeyCount;
            var keysFound = 0;
            for (var i = 0; i < count; ++i)
            {
                keysFound = this.rightOptimizedDataPage.ContainsKey(i)
                    ? keysFound + 1
                    : keysFound;
            }

            return keysFound;
        }

        [Benchmark]
        public int AppendOnlyDataPage_ContainsKey()
        {
            var count = this.KeyCount;
            var keysFound = 0;
            for (var i = 0; i < count; ++i)
            {
                keysFound = this.appendOnlyDataPage.ContainsKey(i)
                    ? keysFound + 1
                    : keysFound;
            }

            return keysFound;
        }
    }
}
