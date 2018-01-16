using System;

namespace Community.RandomOrg.Benchmarks.Framework
{
    /// <summary>Represents the benchmark suite attribute.</summary>
    [AttributeUsage(AttributeTargets.Class)]
    internal sealed class BenchmarkSuiteAttribute : Attribute
    {
        /// <summary>Initializes a new instance of the <see cref="BenchmarkSuiteAttribute" /> class.</summary>
        /// <param name="name">The name of the suite.</param>
        /// <exception cref="ArgumentNullException"><paramref name="name" /> is <see langword="null" />.</exception>
        public BenchmarkSuiteAttribute(string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            Name = name;
        }

        /// <summary>Gets the name of the suite.</summary>
        public string Name
        {
            get;
        }
    }
}