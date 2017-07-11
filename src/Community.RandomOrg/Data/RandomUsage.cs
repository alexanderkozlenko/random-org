using System;

namespace Community.RandomOrg.Data
{
    /// <summary>Encapsulates information related to the usage of the current API key.</summary>
    public sealed class RandomUsage
    {
        internal RandomUsage(ApiKeyStatus status, DateTime creationTime, long totalBits, long bitsLeft, long totalRequests, long requestsLeft)
        {
            Status = status;
            CreationTime = creationTime;
            TotalBits = totalBits;
            BitsLeft = bitsLeft;
            TotalRequests = totalRequests;
            RequestsLeft = requestsLeft;
        }

        /// <summary>Gets the API key's current status.</summary>
        public ApiKeyStatus Status { get; }

        /// <summary>Gets the timestamp at which the API key was created.</summary>
        public DateTime CreationTime { get; }

        /// <summary>Gets an integer containing the (estimated) number of remaining true random bits available to the client.</summary>
        public long BitsLeft { get; }

        /// <summary>Gets an integer containing the (estimated) number of remaining API requests available to the client.</summary>
        public long RequestsLeft { get; }

        /// <summary>Gets an integer containing the number of bits used by this API key since it was created.</summary>
        public long TotalBits { get; }

        /// <summary>Gets an integer containing the number of requests used by this API key since it was created.</summary>
        public long TotalRequests { get; }
    }
}