using System;
using System.Linq;
using System.Reflection;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Loggers;
using BenchmarkDotNet.Running;

namespace Community.RandomOrg.Benchmarks.Framework
{
    /// <summary>Benchmark suite runner.</summary>
    internal static class BenchmarkSuiteRunner
    {
        /// <summary>Runs benchmark suites from the specified assembly.</summary>
        /// <param name="assembly">Assembly to search benchmark suites in.</param>
        /// <param name="configuration">Benchmark running configuration.</param>
        /// <exception cref="ArgumentNullException"><paramref name="assembly" /> or <paramref name="configuration" /> is <see langword="null" />.</exception>
        public static void Run(Assembly assembly, IConfig configuration)
        {
            if (assembly == null)
            {
                throw new ArgumentNullException(nameof(assembly));
            }
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            var suites = assembly.GetExportedTypes()
                .Select(type => (Type: type, Attribute: type.GetCustomAttribute<BenchmarkSuiteAttribute>()))
                .Where(tuple => tuple.Attribute != null)
                .Select(tuple => (tuple.Type, tuple.Attribute.Name))
                .OrderBy(tuple => tuple.Name)
                .ToArray();

            WriteLine(configuration, $"Found {suites.Length} benchmark suite(s)");

            foreach (var suite in suites)
            {
                WriteLine(configuration, $"Running benchmark suite \"{suite.Name}\"...");

                BenchmarkRunner.Run(suite.Type, configuration);
            }
        }

        private static void WriteLine(IConfig configuration, string text)
        {
            foreach (var logger in configuration.GetLoggers())
            {
                logger.WriteLine(LogKind.Default, text);
            }
        }
    }
}