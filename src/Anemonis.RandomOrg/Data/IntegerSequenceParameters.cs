// © Alexander Kozlenko. Licensed under the MIT License.

using System.Collections.Generic;

namespace Anemonis.RandomOrg.Data
{
    /// <summary>Encapsulates random integers generation parameters.</summary>
    public sealed class IntegerSequenceParameters : RandomParameters
    {
        /// <summary>Initializes a new instance of the <see cref="IntegerSequenceParameters" /> class.</summary>
        public IntegerSequenceParameters()
        {
        }

        /// <summary>Gets or sets a collection specifying the lower boundaries for the requested sequences.</summary>
        public IReadOnlyList<int> Minimums
        {
            get;
            set;
        }

        /// <summary>Gets or sets a collection specifying the upper boundaries for the requested sequences.</summary>
        public IReadOnlyList<int> Maximums
        {
            get;
            set;
        }

        /// <summary>Gets or sets a collection specifying for each requested sequence whether the random numbers in that sequence should be picked with replacement.</summary>
        public IReadOnlyList<bool> Replacements
        {
            get;
            set;
        }
    }
}
