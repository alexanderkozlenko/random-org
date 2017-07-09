using System;
using System.Collections.Generic;

namespace Community.RandomOrg.Data
{
    /// <summary>Encapsulates the signed random objects and associated data.</summary>
    /// <typeparam name="T">The type of random object.</typeparam>
    public abstract class SignedRandom<T>
    {
        /// <summary>Gets or sets the SHA-512 hash of the API key.</summary>
        public byte[] ApiKeyHash { get; set; }

        /// <summary>Gets or sets the timestamp at which the request was completed.</summary>
        public DateTime CompletionTime { get; set; }

        /// <summary>Gets or sets an integer containing the serial number associated with this response..</summary>
        public long SerialNumber { get; set; }

        /// <summary>Gets or sets the sequence of objects requested.</summary>
        public IReadOnlyList<T> Data { get; set; }
    }
}