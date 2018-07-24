// © Alexander Kozlenko. Licensed under the MIT License.

using System;

namespace Community.RandomOrg
{
    /// <summary>Represents an error that occur during RANDOM.ORG result handling.</summary>
    public sealed class RandomOrgContractException : Exception
    {
        internal RandomOrgContractException(string requestId, string message)
            : base(message)
        {
            RequestId = requestId;
        }

        internal RandomOrgContractException(string requestId, string message, Exception inner)
            : base(message, inner)
        {
            RequestId = requestId;
        }

        /// <summary>Gets the request identifier.</summary>
        public string RequestId
        {
            get;
        }
    }
}