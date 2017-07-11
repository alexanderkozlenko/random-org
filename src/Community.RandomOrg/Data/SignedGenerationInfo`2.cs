namespace Community.RandomOrg.Data
{
    /// <summary>Encapsulates signed random data generation information.</summary>
    /// <typeparam name="TRandom">The type of random data container.</typeparam>
    /// <typeparam name="TValue">The type of random object.</typeparam>
    public sealed class SignedGenerationInfo<TRandom, TValue> : GenerationInfo<TRandom, TValue>
        where TRandom : SignedRandom<TValue>
    {
        internal SignedGenerationInfo(TRandom random, long bitsUsed, long bitsLeft, long requestsLeft, byte[] signature)
            : base(random, bitsUsed, bitsLeft, requestsLeft)
        {
            Signature = signature;
        }

        /// <summary>Gets the signature of the random object, signed with RANDOM.ORG's private key.</summary>
        public byte[] Signature { get; }
    }
}