namespace Community.RandomOrg.Data
{
    /// <summary>Encapsulates signed random data generation result.</summary>
    /// <typeparam name="TValue">The type of random object.</typeparam>
    /// <typeparam name="TParameters">The type of random parameters.</typeparam>
    public sealed class SignedRandomResult<TValue, TParameters> : RandomResultObject<SignedRandom<TValue, TParameters>, TValue>
        where TParameters : RandomParameters, new()
    {
        internal SignedRandomResult(SignedRandom<TValue, TParameters> random, long bitsUsed, long bitsLeft, long requestsLeft, byte[] signature)
            : base(random, bitsUsed, bitsLeft, requestsLeft)
        {
            Signature = signature;
        }

        /// <summary>Gets the signature of the random object, signed with RANDOM.ORG's private key.</summary>
        public byte[] Signature
        {
            get;
        }
    }
}