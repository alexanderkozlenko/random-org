using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Community.RandomOrg.Tests.Internal
{
    internal sealed class TestHttpHandler : HttpMessageHandler
    {
        private readonly Func<HttpRequestMessage, Task<HttpResponseMessage>> _handler;

        public TestHttpHandler(Func<HttpRequestMessage, Task<HttpResponseMessage>> handler = null)
        {
            _handler = handler;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (_handler == null)
            {
                throw new InvalidOperationException("Request processing is not available");
            }

            TraceRequest(request);

            return _handler.Invoke(request);
        }

        [Conditional("DEBUG")]
        private static void TraceRequest(HttpRequestMessage request)
        {
            Trace.WriteLine(request.ToString());
        }
    }
}