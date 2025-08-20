using BenchmarkDotNet.Running;

namespace Snowspeck.Benchmarks;

internal abstract class Program
{
    private static void Main()
    {
        BenchmarkRunner.Run([
            typeof(SingleThreadedBenchmarks),
            typeof(MultiThreadedBenchmarks)
        ]);
    }
}
