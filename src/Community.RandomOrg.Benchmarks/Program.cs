using System.Linq;
using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Engines;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Loggers;
using BenchmarkDotNet.Reports;
using BenchmarkDotNet.Validators;
using Community.RandomOrg.Benchmarks.Framework;

namespace Community.RandomOrg.Benchmarks
{
    public static class Program
    {
        public static void Main()
        {
            var configuration = ManualConfig.CreateEmpty();

            configuration.Add(JitOptimizationsValidator.DontFailOnError);
            configuration.Add(Job.Dry.With(RunStrategy.Throughput).WithTargetCount(2));
            configuration.Add(MemoryDiagnoser.Default);
            configuration.Add(ConsoleLogger.Default);
            configuration.Add(new SimpleBenchmarkExporter());
            configuration.Add(DefaultConfig.Instance.GetColumnProviders().ToArray());
            configuration.Set(SummaryStyle.Default.WithSizeUnit(SizeUnit.B));

            BenchmarkSuiteRunner.Run(typeof(Program).Assembly, configuration);
        }
    }
}