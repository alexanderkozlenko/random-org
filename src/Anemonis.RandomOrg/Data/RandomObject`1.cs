﻿// © Alexander Kozlenko. Licensed under the MIT License.

using System.Collections.Generic;

namespace Anemonis.RandomOrg.Data
{
    /// <summary>Encapsulates the random objects and associated data.</summary>
    /// <typeparam name="T">The type of random object.</typeparam>
    public abstract class RandomObject<T> : RandomObject
    {
        private protected RandomObject()
        {
        }

        /// <summary>Gets or sets the sequence of objects requested.</summary>
        public IReadOnlyList<T> Data
        {
            get;
            set;
        }
    }
}
