﻿// © Alexander Kozlenko. Licensed under the MIT License.

using System;

namespace Anemonis.RandomOrg.Data
{
    /// <summary>Encapsulates signed random data generation result.</summary>
    /// <typeparam name="TValue">The type of random object.</typeparam>
    /// <typeparam name="TParameters">The type of random parameters.</typeparam>
    public sealed class SignedRandomResult<TValue, TParameters> : RandomResultObject<SignedRandom<TValue, TParameters>, TValue>
        where TParameters : RandomParameters, new()
    {
        private readonly byte[] _signature;

        internal SignedRandomResult(SignedRandom<TValue, TParameters> random, long bitsUsed, long bitsLeft, long requestsLeft, byte[] signature, TimeSpan advisoryDelay)
            : base(random, bitsUsed, bitsLeft, requestsLeft, advisoryDelay)
        {
            _signature = signature;
        }

        /// <summary>Gets a SHA-512 digest of the JSON representation of a <see cref="SignedRandom{TValue, TParameters}" /> object, which has been signed with RANDOM.ORG's private key.</summary>
        /// <returns>A copy of <see cref="byte" /> array representing the SHA-512 digest.</returns>
        public byte[] GetSignature()
        {
            return (byte[])_signature.Clone();
        }
    }
}
