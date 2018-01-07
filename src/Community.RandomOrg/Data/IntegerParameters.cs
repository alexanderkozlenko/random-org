namespace Community.RandomOrg.Data
{
    /// <summary>Encapsulates random integers generation parameters.</summary>
    public sealed class IntegerParameters : RandomParameters
    {
        /// <summary>Initializes a new instance of the <see cref="IntegerParameters" /> class.</summary>
        public IntegerParameters()
        {
        }

        /// <summary>Gets or sets the lower boundary for the range from which the random numbers will be picked.</summary>
        public int Minimum
        {
            get;
            set;
        }

        /// <summary>Gets or sets the upper boundary for the range from which the random numbers will be picked.</summary>
        public int Maximum
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