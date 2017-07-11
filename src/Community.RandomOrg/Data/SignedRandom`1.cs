namespace Community.RandomOrg.Data
{
    /// <summary>Encapsulates signed random objects and associated data.</summary>
    /// <typeparam name="T">The type of random object.</typeparam>
    public abstract class SignedRandom<T> : Random<T>
    {
        internal SignedRandom()
        {
        }

        /// <summary>Gets or sets the SHA-512 hash of the API key.</summary>
        public byte[] ApiKeyHash { get; set; }

        /// <summary>Gets or sets an integer containing the serial number associated with this random information.</summary>
        public long SerialNumber { get; set; }
    }
}