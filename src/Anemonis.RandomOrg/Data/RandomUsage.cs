// © Alexander Kozlenko. Licensed under the MIT License.

namespace Anemonis.RandomOrg.Data
{
    /// <summary>Encapsulates information related to the usage of the current API key.</summary>
    public sealed class RandomUsage
    {
        internal RandomUsage(ApiKeyStatus status, long bitsLeft, long requestsLeft)
        {
            Status = status;
            BitsLeft = bitsLeft;
            RequestsLeft = requestsLeft;
        }

        /// <summary>Gets the API key's current status.</summary>
        public ApiKeyStatus Status
        {
            get;
        }

        /// <summary>Gets an integer containing the (estimated) number of remaining true random bits available to the client from the daily quota.</summary>
        public long BitsLeft
        {
            get;
        }

        /// <summary>Gets an integer containing the (estimated) number of remaining API requests available to the client from the daily quota.</summary>
        public long RequestsLeft
        {
            get;
        }
    }
}
