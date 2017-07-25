using System;

namespace Community.RandomOrg.Data
{
    /// <summary>Encapsulates simple random data generation information.</summary>
    /// <typeparam name="T">The type of random object.</typeparam>
    public sealed class SimpleGenerationInfo<T> : GenerationInfo<SimpleRandom<T>, T>
    {
        /// <summary>Initializes a new instance of the <see cref="SimpleGenerationInfo{T}" /> class.</summary>
        /// <param name="random">The random objects and associated data.</param>
        /// <param name="bitsUsed">An integer containing the number of true random bits used to complete this operation.</param>
        /// <param name="bitsLeft">An integer containing the (estimated) number of remaining true random bits available to the client.</param>
        /// <param name="requestsLeft">An integer containing the (estimated) number of remaining API requests available to the client.</param>
        /// <exception cref="ArgumentNullException"><paramref name="random" /> is <see langword="null" />.</exception>
        public SimpleGenerationInfo(SimpleRandom<T> random, long bitsUsed, long bitsLeft, long requestsLeft)
            : base(random, bitsUsed, bitsLeft, requestsLeft)
        {
        }
    }
}