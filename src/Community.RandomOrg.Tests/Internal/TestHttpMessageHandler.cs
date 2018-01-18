using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Xunit.Abstractions;

namespace Community.RandomOrg.Tests.Internal
{
    /// <summary>An HTTP mesage handler for unit testing.</summary>
    internal sealed class TestHttpMessageHandler : HttpMessageHandler
    {
        private readonly ITestOutputHelper _output;
        private readonly Func<HttpRequestMessage, Task<HttpResponseMessage>> _handler;

        /// <summary>Initializes a new instance of the <see cref="TestHttpMessageHandler" /> class.</summary>
        public TestHttpMessageHandler()
        {
        }

        /// <summary>Initializes a new instance of the <see cref="TestHttpMessageHandler" /> class.</summary>
        /// <param name="output">The test output provider.</param>
        /// <param name="handler">The handler function.</param>
        public TestHttpMessageHandler(ITestOutputHelper output, Func<HttpRequestMessage, Task<HttpResponseMessage>> handler)
        {
            _output = output;
            _handler = handler;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (_handler == null)
            {
                throw new InvalidOperationException("Request processing is not available");
            }

            _output?.WriteLine(request.ToString());

            return _handler.Invoke(request);
        }
    }
}