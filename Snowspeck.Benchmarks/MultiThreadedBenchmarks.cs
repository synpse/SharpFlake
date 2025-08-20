using BenchmarkDotNet.Attributes;
using Snowspeck.Interfaces;
using Snowspeck.Services;

namespace Snowspeck.Benchmarks;

[MemoryDiagnoser]
[SimpleJob]
[HideColumns("Job", "LaunchCount", "WarmupCount", "IterationCount")]
public class MultiThreadedBenchmarks
{
    private readonly ISnowflakeGenerator<long> _signed;

    public MultiThreadedBenchmarks()
    {
        var options = new SnowflakeOptions { WorkerId = 1, Epoch = 1735689600000L };
        _signed = new SignedSnowflakeService(options);
    }

    [Params(1, 2, 4, 8, 16, 32)]
    public int Threads { get; set; }

    [Params(10_000)]
    public int OpsPerThread { get; set; }

    [Benchmark(Baseline = true)]
    public int Guid_NewGuid_Parallel()
    {
        int acc = 0;
        Parallel.For(0, Threads, _ =>
        {
            int local = 0;
            for (int i = 0; i < OpsPerThread; i++)
                local ^= Guid.NewGuid().GetHashCode();

            int init, computed;
            do
            {
                init = acc;
                computed = init ^ local;
            } while (Interlocked.CompareExchange(ref acc, computed, init) != init);
        });

        return acc;
    }

    [Benchmark]
    public long Snowflake_Signed_Parallel()
    {
        long acc = 0;
        Parallel.For(0, Threads, _ =>
        {
            long local = 0;
            for (int i = 0; i < OpsPerThread; i++)
                local ^= _signed.NextId();

            long init, computed;
            do
            {
                init = acc;
                computed = init ^ local;
            } while (Interlocked.CompareExchange(ref acc, computed, init) != init);
        });

        return acc;
    }
}
