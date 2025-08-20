using BenchmarkDotNet.Attributes;
using Snowspeck.Interfaces;
using Snowspeck.Services;

namespace Snowspeck.Benchmarks;

[MemoryDiagnoser]
[SimpleJob]
[HideColumns("Job", "LaunchCount", "WarmupCount", "IterationCount", "Threads")]
public class SingleThreadedBenchmarks
{
    private readonly ISnowflakeGenerator<long> _signed;
    private readonly ISnowflakeGenerator<ulong> _unsigned;

    public SingleThreadedBenchmarks()
    {
        var options = new SnowflakeOptions { WorkerId = 1, Epoch = 1735689600000L };
        _signed = new SignedSnowflakeService(options);
        _unsigned = new UnsignedSnowflakeService(options);
    }

    [Benchmark(Baseline = true)]
    public Guid Guid_NewGuid() => Guid.NewGuid();

    [Benchmark]
    public long Snowflake_Signed() => _signed.NextId();

    [Benchmark]
    public ulong Snowflake_Unsigned() => _unsigned.NextId();
}
