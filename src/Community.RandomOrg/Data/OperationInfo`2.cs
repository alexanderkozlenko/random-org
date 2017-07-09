namespace Community.RandomOrg.Data
{
    /// <summary>Encapsulates signed random data generation information.</summary>
    /// <typeparam name="TRandom">The type of random data container.</typeparam>
    /// <typeparam name="TValue">The type of random object.</typeparam>
    public sealed class OperationInfo<TRandom, TValue>
        where TRandom : SignedRandom<TValue>
    {
        internal OperationInfo(TRandom random, byte[] signature, long bitsUsed, long bitsLeft, long requestsLeft)
        {
            Random = random;
            Signature = signature;
            BitsUsed = bitsUsed;
            BitsLeft = bitsLeft;
            RequestsLeft = requestsLeft;
        }

        /// <summary>Gets the random objects and associated data.</summary>
        public TRandom Random { get; }

        /// <summary>Gets the signature of the random object, signed with RANDOM.ORG's private key.</summary>
        public byte[] Signature { get; }

        /// <summary>Gets an integer containing the (estimated) number of remaining true random bits available to the client.</summary>
        public long BitsLeft { get; }

        /// <summary>Gets an integer containing the (estimated) number of remaining API requests available to the client.</summary>
        public long RequestsLeft { get; }

        /// <summary>Gets an integer containing the number of true random bits used to complete this request.</summary>
        public long BitsUsed { get; }
    }
}