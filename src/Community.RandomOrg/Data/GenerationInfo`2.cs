using System;

namespace Community.RandomOrg.Data
{
    /// <summary>Encapsulates random data generation information.</summary>
    /// <typeparam name="TRandom">The type of random data container.</typeparam>
    /// <typeparam name="TValue">The type of random object.</typeparam>
    public abstract class GenerationInfo<TRandom, TValue>
        where TRandom : Random<TValue>
    {
        internal GenerationInfo(TRandom random, long bitsUsed, long bitsLeft, long requestsLeft)
        {
            if (random == null)
            {
                throw new ArgumentNullException(nameof(random));
            }

            Random = random;
            BitsUsed = bitsUsed;
            BitsLeft = bitsLeft;
            RequestsLeft = requestsLeft;
        }

        /// <summary>Gets the random objects and associated data.</summary>
        public TRandom Random { get; }

        /// <summary>Gets an integer containing the (estimated) number of remaining true random bits available to the client.</summary>
        public long BitsLeft { get; }

        /// <summary>Gets an integer containing the (estimated) number of remaining API requests available to the client.</summary>
        public long RequestsLeft { get; }

        /// <summary>Gets an integer containing the number of true random bits used to complete this operation.</summary>
        public long BitsUsed { get; }
    }
}