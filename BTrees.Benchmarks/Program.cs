using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Environments;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;
using BenchmarkDotNet.Toolchains.InProcess.Emit;
using BTrees.Benchmarks;

var config = DefaultConfig.Instance
    .AddJob(Job
         .MediumRun
         .WithPlatform(Platform.X64)
         .WithRuntime(CoreRuntime.Core70)
         .WithRuntime(CoreRuntime.Core80)
         .WithToolchain(InProcessEmitToolchain.Instance));
//         .WithLaunchCount(1)

//.WithToolchain(InProcessNoEmitToolchain.Instance));

var _ = BenchmarkRunner.Run<StackAllocBenchmark>(config);
