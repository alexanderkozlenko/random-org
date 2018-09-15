// © Alexander Kozlenko. Licensed under the MIT License.

namespace Anemonis.RandomOrg.Data
{
    /// <summary>Encapsulates random strings generation parameters.</summary>
    public sealed class StringParameters : RandomParameters
    {
        /// <summary>Initializes a new instance of the <see cref="StringParameters" /> class.</summary>
        public StringParameters()
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