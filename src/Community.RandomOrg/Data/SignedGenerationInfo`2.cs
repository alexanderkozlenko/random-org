using System;

namespace Community.RandomOrg.Data
{
    /// <summary>Encapsulates signed random data generation information.</summary>
    /// <typeparam name="TRandom">The type of random data container.</typeparam>
    /// <typeparam name="TValue">The type of random object.</typeparam>
    public sealed class SignedGenerationInfo<TRandom, TValue> : GenerationInfo<TRandom, TValue>
        where TRandom : SignedRandom<TValue>
    {
        /// <summary>Initializes a new instance of the <see cref="SignedGenerationInfo{TRandom, TValue}" /> class.</summary>
        /// <param name="random">The random objects and associated data.</param>
        /// <param name="bitsUsed">An integer containing the number of true random bits used to complete this operation.</param>
        /// <param name="bitsLeft">An integer containing the (estimated) number of remaining true random bits available to the client.</param>
        /// <param name="requestsLeft">An integer containing the (estimated) number of remaining API requests available to the client.</param>
        /// <param name="signature">The signature of the random object, signed with RANDOM.ORG's private key.</param>
        /// <exception cref="ArgumentNullException"><paramref name="random" /> or <paramref name="signature" /> is <see langword="null" />.</exception>
        public SignedGenerationInfo(TRandom random, long bitsUsed, long bitsLeft, long requestsLeft, byte[] signature)
            : base(random, bitsUsed, bitsLeft, requestsLeft)
        {
            if (signature == null)
            {
                throw new ArgumentNullException(nameof(signature));
            }

            Signature = signature;
        }

        /// <summary>Gets the signature of the random object, signed with RANDOM.ORG's private key.</summary>
        public byte[] Signature { get; }
    }
}