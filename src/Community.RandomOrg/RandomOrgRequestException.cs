using System.Net;
using System.Net.Http;

namespace Community.RandomOrg
{
    /// <summary>Represents an error for an unsuccessful HTTP request to RANDOM.ORG service.</summary>
    public sealed class RandomOrgRequestException : HttpRequestException
    {
        internal RandomOrgRequestException(HttpStatusCode statusCode, string message)
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