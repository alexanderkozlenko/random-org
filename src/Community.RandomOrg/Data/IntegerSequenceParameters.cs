namespace Community.RandomOrg.Data
{
    /// <summary>Encapsulates random integers generation parameters.</summary>
    public sealed class IntegerSequenceParameters : RandomParameters
    {
        /// <summary>Initializes a new instance of the <see cref="IntegerSequenceParameters" /> class.</summary>
        public IntegerSequenceParameters()
        {
        }

        /// <summary>Gets or sets the lower boundary for the range from which the random numbers will be picked.</summary>
        public int[] Minimums
        {
            get;
            set;
        }

        /// <summary>Gets or sets the upper boundary for the range from which the random numbers will be picked.</summary>
        public int[] Maximums
        {
            get;
            set;
        }

        /// <summary>Gets or sets a value which specifies whether the random numbers should be picked with replacement.</summary>
        public bool[] Replacements
        {
            get;
            set;
        }
    }
}