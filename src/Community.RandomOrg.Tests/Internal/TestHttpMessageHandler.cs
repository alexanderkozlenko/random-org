using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Community.RandomOrg.Tests.Internal
{
    /// <summary>An HTTP mesage handler for unit testing.</summary>
    internal sealed class TestHttpMessageHandler : HttpMessageHandler
    {
        private readonly Func<HttpRequestMessage, Task<HttpResponseMessage>> _handler;

        /// <summary>Initializes a new instance of the <see cref="TestHttpMessageHandler" /> class.</summary>
        /// <param name="handler">The handler function.</param>
        public TestHttpMessageHandler(Func<HttpRequestMessage, Task<HttpResponseMessage>> handler = null)
        {
            _handler = handler;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (_handler == null)
            {
                throw new InvalidOperationException("Request processing is not available");
            }

            return _handler.Invoke(request);
        }
    }
}