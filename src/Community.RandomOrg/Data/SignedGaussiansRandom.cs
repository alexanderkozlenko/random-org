namespace Community.RandomOrg.Data
{
    /// <summary>Encapsulates the signed random numbers from a Gaussian distribution and associated data.</summary>
    public sealed class SignedGaussiansRandom : SignedRandom<decimal>
    {
        /// <summary>Initializes a new instance of the <see cref="SignedGaussiansRandom" /> class.</summary>
        public SignedGaussiansRandom()
        {
        }

        /// <summary>Gets or sets the distribution's mean.</summary>
        public decimal Mean { get; set; }

        /// <summary>Gets or sets the distribution's standard deviation.</summary>
        public decimal StandardDeviation { get; set; }

        /// <summary>Gets or sets the number of significant digits to use.</summary>
        public int SignificantDigits { get; set; }
    }
}