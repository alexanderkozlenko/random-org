using System.Net;
using System.Net.Http;

namespace Community.RandomOrg
{
    /// <summary>Represents an error for an unsuccessful HTTP request to RANDOM.ORG.</summary>
    public sealed class RandomOrgHttpRequestException : HttpRequestException
    {
        internal RandomOrgHttpRequestException(string message, HttpStatusCode statusCode, string reasonPhrase)
            : base(message)
        {
            StatusCode = statusCode;
            ReasonPhrase = reasonPhrase;
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