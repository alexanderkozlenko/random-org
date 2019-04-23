// © Alexander Kozlenko. Licensed under the MIT License.

using System;

namespace Anemonis.RandomOrg
{
    /// <summary>Represents an error that occurs during processing RANDOM.ORG result.</summary>
    public sealed class RandomOrgClientException : Exception
    {
        internal RandomOrgClientException(string message)
            : base(message)
        {
        }

        internal RandomOrgClientException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
