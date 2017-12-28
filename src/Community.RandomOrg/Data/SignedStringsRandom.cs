namespace Community.RandomOrg.Data
{
    /// <summary>Encapsulates the signed random strings and associated data.</summary>
    public sealed class SignedStringsRandom : SignedRandom<string>
    {
        /// <summary>Initializes a new instance of the <see cref="SignedStringsRandom" /> class.</summary>
        public SignedStringsRandom()
        {
        }

        /// <summary>Gets or sets the length of each string.</summary>
        public int Length
        {
            get;
            set;
        }

        /// <summary>Gets or sets a string that contains the set of characters that are allowed to occur in the random strings.</summary>
        public string Characters
        {
            get;
            set;
        }

        /// <summary>Gets or sets a value which specifies whether the random strings should be picked with replacement.</summary>
        public bool Replacement
        {
            get;
            set;
        }
    }
}