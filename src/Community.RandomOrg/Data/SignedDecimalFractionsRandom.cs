namespace Community.RandomOrg.Data
{
    /// <summary>Encapsulates the signed random decimal fractions and associated data.</summary>
    public sealed class SignedDecimalFractionsRandom : SignedRandom<decimal>
    {
        /// <summary>Initializes a new instance of the <see cref="SignedDecimalFractionsRandom" /> class.</summary>
        public SignedDecimalFractionsRandom()
        {
        }

        /// <summary>Gets or sets the number of decimal places to use.</summary>
        public int DecimalPlaces
        {
            get;
            set;
        }

        /// <summary>Gets or sets a value which specifies whether the random numbers should be picked with replacement.</summary>
        public bool Replacement
        {
            get;
            set;
        }
    }
}