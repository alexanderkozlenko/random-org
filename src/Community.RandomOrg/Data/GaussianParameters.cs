// © Alexander Kozlenko. Licensed under the MIT License.

namespace Community.RandomOrg.Data
{
    /// <summary>Encapsulates random numbers from a Gaussian distribution generation parameters.</summary>
    public sealed class GaussianParameters : RandomParameters
    {
        /// <summary>Initializes a new instance of the <see cref="GaussianParameters" /> class.</summary>
        public GaussianParameters()
        {
        }

        /// <summary>Gets or sets the distribution's mean.</summary>
        public decimal Mean
        {
            get;
            set;
        }

        /// <summary>Gets or sets the distribution's standard deviation.</summary>
        public decimal StandardDeviation
        {
            get;
            set;
        }

        /// <summary>Gets or sets the number of significant digits to use.</summary>
        public int SignificantDigits
        {
            get;
            set;
        }
    }
}