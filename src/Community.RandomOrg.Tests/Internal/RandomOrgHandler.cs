using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Community.RandomOrg.Tests.Internal
{
    internal sealed class RandomOrgHandler : HttpMessageHandler
    {
        private readonly Func<HttpRequestMessage, Task<HttpResponseMessage>> _handler;

        public RandomOrgHandler(Func<HttpRequestMessage, Task<HttpResponseMessage>> handler = null)
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