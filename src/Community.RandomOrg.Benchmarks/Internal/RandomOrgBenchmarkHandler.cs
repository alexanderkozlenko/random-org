using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Community.RandomOrg.Benchmarks.Internal
{
    internal sealed class RandomOrgBenchmarkHandler : HttpMessageHandler
    {
        private static readonly MediaTypeHeaderValue _mediaTypeHeaderValue = new MediaTypeHeaderValue("application/json");

        private readonly IReadOnlyDictionary<string, string> _contents;

        public RandomOrgBenchmarkHandler(IReadOnlyDictionary<string, string> contents)
        {
            if (contents == null)
            {
                throw new ArgumentNullException(nameof(contents));
            }

            _contents = contents;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var requestToken = JObject.Parse(await request.Content.ReadAsStringAsync());
            var responseContent = new StringContent(_contents[(string)requestToken["method"]].Replace("{id}", (string)requestToken["id"]));

            responseContent.Headers.ContentType = _mediaTypeHeaderValue;

            return new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = responseContent
            };
        }
    }
}