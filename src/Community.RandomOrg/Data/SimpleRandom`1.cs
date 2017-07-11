namespace Community.RandomOrg.Data
{
    /// <summary>Encapsulates simple random objects and associated data.</summary>
    /// <typeparam name="T">The type of random object.</typeparam>
    public sealed class SimpleRandom<T> : Random<T>
    {
        /// <summary>Initializes a new instance of the <see cref="Random{T}" /> class.</summary>
        public SimpleRandom()
        {
        }
    }
}