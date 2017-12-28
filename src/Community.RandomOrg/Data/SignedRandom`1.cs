namespace Community.RandomOrg.Data
{
    /// <summary>Encapsulates signed random objects and associated data.</summary>
    /// <typeparam name="T">The type of random object.</typeparam>
    public abstract class SignedRandom<T> : Random<T>
    {
        internal SignedRandom()
        {
            License = new License();
        }

        /// <summary>Gets or sets the SHA-512 hash of the API key.</summary>
        public byte[] ApiKeyHash
        {
            get;
            set;
        }

        /// <summary>Gets or sets an integer containing the serial number associated with this random information.</summary>
        public long SerialNumber
        {
            get;
            set;
        }

        /// <summary>Gets or sets an optional string that is included into signed data from generation parameters.</summary>
        public string UserData
        {
            get;
            set;
        }

        /// <summary>Gets an object describing the license terms under which the random values can be used.</summary>
        public License License
        {
            get;
        }
    }
}