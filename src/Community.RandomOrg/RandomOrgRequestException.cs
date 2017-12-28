using System.Net;
using System.Net.Http;

namespace Community.RandomOrg
{
    /// <summary>Represents an error for an unsuccessful HTTP request to RANDOM.ORG.</summary>
    public sealed class RandomOrgRequestException : HttpRequestException
    {
        internal RandomOrgRequestException(string message, string rpcMethod, HttpStatusCode statusCode, string reasonPhrase)
            : base(message)
        {
            RpcMethod = rpcMethod;
            StatusCode = statusCode;
            ReasonPhrase = reasonPhrase;
        }

        /// <summary>Gets the RPC method name of the HTTP response.</summary>
        public string RpcMethod
        {
            get;
        }

        /// <summary>Gets the reason phrase of the HTTP response.</summary>
        public string ReasonPhrase
        {
            get;
        }

        /// <summary>Gets the status code of the HTTP response.</summary>
        public HttpStatusCode StatusCode
        {
            get;
        }
    }
}