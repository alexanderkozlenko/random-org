using System;
using System.Collections.Generic;

namespace Community.RandomOrg.Data
{
    /// <summary>Encapsulates the random objects and associated data.</summary>
    /// <typeparam name="T">The type of random object.</typeparam>
    public sealed class Random<T>
    {
        /// <summary>Gets or sets the timestamp at which the request was completed.</summary>
        public DateTime CompletionTime { get; set; }

        /// <summary>Gets or sets the sequence of objects requested.</summary>
        public IReadOnlyList<T> Data { get; set; }
    }
}