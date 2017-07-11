namespace Community.RandomOrg.Data
{
    /// <summary>Encapsulates simple random data generation information.</summary>
    /// <typeparam name="T">The type of random object.</typeparam>
    public sealed class SimpleGenerationInfo<T> : GenerationInfo<SimpleRandom<T>, T>
    {
        internal SimpleGenerationInfo(SimpleRandom<T> random, long bitsUsed, long bitsLeft, long requestsLeft)
            : base(random, bitsUsed, bitsLeft, requestsLeft)
        {
        }
    }
}