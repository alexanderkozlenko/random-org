namespace Community.RandomOrg.Data
{
    /// <summary>Encapsulates the signed random integers and associated data.</summary>
    public sealed class SignedIntegersRandom : SignedRandom<int>
    {
        /// <summary>Initializes a new instance of the <see cref="SignedIntegersRandom" /> class.</summary>
        public SignedIntegersRandom()
        {
        }

        /// <summary>Gets or sets the lower boundary for the range from which the random numbers will be picked.</summary>
        public int Minimum { get; set; }

        /// <summary>Gets or sets the upper boundary for the range from which the random numbers will be picked.</summary>
        public int Maximum { get; set; }

        /// <summary>Gets or sets a value which specifies whether the random numbers should be picked with replacement.</summary>
        public bool Replacement { get; set; }
    }
}