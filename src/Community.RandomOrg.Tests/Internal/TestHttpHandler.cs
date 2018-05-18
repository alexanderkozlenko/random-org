using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Xunit.Abstractions;

namespace Community.RandomOrg.Tests.Internal
{
    internal sealed class TestHttpHandler : HttpMessageHandler
    {
        private readonly ITestOutputHelper _output;
        private readonly Func<HttpRequestMessage, Task<HttpResponseMessage>> _handler;

        public TestHttpHandler()
        {
        }

        public TestHttpHandler(ITestOutputHelper output, Func<HttpRequestMessage, Task<HttpResponseMessage>> handler)
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