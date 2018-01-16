using System;
using System.Collections.Generic;
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
        /// <param name="configuration">Benchmark runninng configuration.</param>
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

            var suites = new List<(Type, string)>();
            var types = assembly.GetExportedTypes();

            for (var i = 0; i < types.Length; i++)
            {
                var attribute = types[i].GetCustomAttribute<BenchmarkSuiteAttribute>();

                if (attribute != null)
                {
                    suites.Add((types[i], attribute.Name));
                }
            }

            suites.Sort();

            WriteLine(configuration, $"Found {suites.Count} benchmark suite(s)");

            for (var i = 0; i < suites.Count; i++)
            {
                var (type, name) = suites[i];

                WriteLine(configuration, $"Running benchmark suite \"{name}\"...");

                BenchmarkRunner.Run(type, configuration);
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