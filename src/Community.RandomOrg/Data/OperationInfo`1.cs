namespace Community.RandomOrg.Data
{
    /// <summary>Encapsulates random data generation information.</summary>
    /// <typeparam name="T">The type of random object.</typeparam>
    public sealed class OperationInfo<T>
    {
        internal OperationInfo(Random<T> random, long bitsUsed, long bitsLeft, long requestsLeft)
        {
            Random = random;
            BitsUsed = bitsUsed;
            BitsLeft = bitsLeft;
            RequestsLeft = requestsLeft;
        }

        /// <summary>Gets the random objects and associated data.</summary>
        public Random<T> Random { get; }

        /// <summary>Gets an integer containing the (estimated) number of remaining true random bits available to the client.</summary>
        public long BitsLeft { get; }

        /// <summary>Gets an integer containing the (estimated) number of remaining API requests available to the client.</summary>
        public long RequestsLeft { get; }

        /// <summary>Gets an integer containing the number of true random bits used to complete this request.</summary>
        public long BitsUsed { get; }
    }
}