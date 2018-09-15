// © Alexander Kozlenko. Licensed under the MIT License.

using System;

namespace Anemonis.RandomOrg.Data
{
    /// <summary>Represents an object describing the license terms under which the random values given in the data can be used.</summary>
    public sealed class RandomLicense
    {
        internal RandomLicense()
        {
        }

        /// <summary>Gets or sets license type.</summary>
        public string Type
        {
            get;
            set;
        }

        /// <summary>Gets or sets license description.</summary>
        public string Text
        {
            get;
            set;
        }

        /// <summary>Gets or sets an URL with license information.</summary>
        public Uri InfoUrl
        {
            get;
            set;
        }
    }
}