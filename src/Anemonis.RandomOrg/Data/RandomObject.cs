// © Alexander Kozlenko. Licensed under the MIT License.

using System;

namespace Anemonis.RandomOrg.Data
{
    /// <summary>Encapsulates the random objects and associated data.</summary>
    public abstract class RandomObject
    {
        private protected RandomObject()
        {
        }

        /// <summary>Gets or sets the timestamp at which the operation was completed.</summary>
        public DateTime CompletionTime
        {
            get;
            set;
        }
    }
}
