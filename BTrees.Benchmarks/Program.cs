using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Environments;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;
using BenchmarkDotNet.Toolchains.InProcess.Emit;
using BTrees.Benchmarks;

var config = DefaultConfig.Instance
    .AddJob(Job
         .LongRun
         .WithPlatform(Platform.X64)
         .WithRuntime(CoreRuntime.Core70)
         //.WithRuntime(CoreRuntime.Core80)
         .WithToolchain(InProcessEmitToolchain.Instance));
//         .WithLaunchCount(1)
//.WithToolchain(InProcessNoEmitToolchain.Instance));

// var _ = BenchmarkRunner.Run<StackAllocBenchmark>(config);
var _ = BenchmarkRunner.Run<DataPageWriteBenchmark>(config);
_ = BenchmarkRunner.Run<DataPageReadBenchmark>(config);

