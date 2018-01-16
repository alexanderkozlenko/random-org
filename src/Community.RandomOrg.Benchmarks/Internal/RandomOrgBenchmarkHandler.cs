using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Community.RandomOrg.Benchmarks.Internal
{
    /// <summary>A benchmark HTTP mesage handler for the <see cref="RandomOrgClient" />.</summary>
    internal sealed class RandomOrgBenchmarkHandler : HttpMessageHandler
    {
        private readonly string _content;

        /// <summary>Initializes a new instance of the <see cref="RandomOrgBenchmarkHandler" /> class.</summary>
        /// <param name="content">The handler response.</param>
        /// <exception cref="ArgumentNullException"><paramref name="content" /> is <see langword="null" />.</exception>
        public RandomOrgBenchmarkHandler(string content)
        {
            if (content == null)
            {
                throw new ArgumentNullException(nameof(content));
            }

            _content = content;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var requestToken = JObject.Parse(await request.Content.ReadAsStringAsync().ConfigureAwait(false));
            var content = _content.Replace("{id}", (string)requestToken["id"]);

            return new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(content, Encoding.UTF8, "application/json")
            };
        }
    }
}