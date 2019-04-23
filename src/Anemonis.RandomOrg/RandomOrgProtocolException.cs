// © Alexander Kozlenko. Licensed under the MIT License.

using System.Net;

using Anemonis.JsonRpc;

namespace Anemonis.RandomOrg
{
    /// <summary>Represents an error that occurs during communication with the RANDOM.ORG service.</summary>
    public sealed class RandomOrgProtocolException : JsonRpcException
    {
        internal RandomOrgProtocolException(HttpStatusCode statusCode, string message)
            : base(message)
        {
            StatusCode = statusCode;
        }

        /// <summary>Gets the status code of the HTTP response.</summary>
        public HttpStatusCode StatusCode
        {
            get;
        }
    }
}
