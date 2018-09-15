// © Alexander Kozlenko. Licensed under the MIT License.

namespace Anemonis.RandomOrg.Data
{
    /// <summary>Encapsulates random decimal fractions generation parameters.</summary>
    public sealed class DecimalFractionParameters : RandomParameters
    {
        /// <summary>Initializes a new instance of the <see cref="DecimalFractionParameters" /> class.</summary>
        public DecimalFractionParameters()
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