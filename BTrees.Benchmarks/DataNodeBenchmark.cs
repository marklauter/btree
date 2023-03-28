using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using BTrees.Nodes;
using BTrees.Types;

namespace BTrees.Benchmarks
{
    //[RPlotExporter]
    [SimpleJob(RuntimeMoniker.Net70, baseline: true)]
    [SimpleJob(RuntimeMoniker.NativeAot70)]
    public class DataNodeBenchmark
    {
        internal static class UniqueIdFactory
        {
            public static DbUniqueId[] Generate(int length)
            {
                var ids = new DbUniqueId[length];
                for (var i = 0; i < length; ++i)
                {
                    ids[i] = DbUniqueId.NewId();
                }

                return ids;
            }
        }

        [Params(1, 16, 32, 64, 128, 256, 512, 1024)]
        public int KeyCount { get; set; }
        private DbUniqueId[]? values;
        private DbUniqueId[]? keys;
        //private readonly DataNode<DbUniqueId, DbUniqueId> node = DataNode<DbUniqueId, DbUniqueId>.Empty();

        [GlobalSetup]
        public void Setup()
        {
            this.keys = UniqueIdFactory.Generate(this.KeyCount);
            this.values = UniqueIdFactory.Generate(this.KeyCount);
        }

        //[Benchmark]
        //public void NewGuid()
        //{
        //    var guid = Guid.NewGuid();
        //    var uniqueId = (DbUniqueId)guid;
        //    if (uniqueId.Value != guid)
        //    {
        //        throw new InvalidCastException();
        //    }
        //}

        //[Benchmark]
        //public void InsertOne()
        //{
        //    this.node.Insert(Guid.NewGuid(), Guid.NewGuid());
        //}

        [Benchmark]
        public void InsertMany()
        {
            var node = DataNode<DbUniqueId, DbUniqueId>.Empty();
            var values = this.values ?? throw new InvalidOperationException();
            var keys = this.keys ?? throw new InvalidOperationException();
            var count = this.KeyCount;

            for (var key = 0; key < count; ++key)
            {
                node.Insert(keys[key], values[key]);
            }

            //Assert.Equal(keyCount + 1, node.Count());

            //for (var key = 0; key < keyCount; ++key)
            //{
            //    Assert.True(node.ContainsKey(key));
            //}

            //for (var key = 0; key < keyCount; ++key)
            //{
            //    var values = node.Read(key);
            //    Assert.Contains(Ids[key], values);
            //}
        }
    }
}
