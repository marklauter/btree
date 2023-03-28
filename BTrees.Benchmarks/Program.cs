// See https://aka.ms/new-console-template for more information
using BenchmarkDotNet.Running;

var _ = BenchmarkRunner.Run(typeof(Program).Assembly);
