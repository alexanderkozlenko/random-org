using System;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Community.RandomOrg.Tests.Stubbing
{
    /// <summary>Represents an HTTP message handler stub.</summary>
    [DebuggerStepThrough]
    internal sealed class HttpMessageHandlerStub : HttpMessageHandler
    {
        private readonly string _mediaType;
        private readonly Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> _messageHandler;
        private readonly Func<string, Task<string>> _contentHandler;

        /// <summary>Initializes a new instance of the <see cref="HttpMessageHandlerStub" /> class.</summary>
        /// <param name="handler">The stub HTTP message handler.</param>
        /// <exception cref="ArgumentNullException"><paramref name="handler" /> is <see langword="null" />.</exception>
        public HttpMessageHandlerStub(Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> handler)
        {
            _messageHandler = handler ?? throw new ArgumentNullException(nameof(handler));
        }

        /// <summary>Initializes a new instance of the <see cref="HttpMessageHandlerStub" /> class.</summary>
        /// <param name="handler">The stub HTTP message content handler.</param>
        /// <param name="mediaType">The media type of the content.</param>
        /// <exception cref="ArgumentNullException"><paramref name="handler" /> or <paramref name="mediaType" /> is <see langword="null" />.</exception>
        public HttpMessageHandlerStub(Func<string, Task<string>> handler, string mediaType)
        {
            _contentHandler = handler ?? throw new ArgumentNullException(nameof(handler)); ;
            _mediaType = mediaType ?? throw new ArgumentNullException(nameof(mediaType));
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (_messageHandler != null)
            {
                return await _messageHandler.Invoke(request, cancellationToken).ConfigureAwait(false);
            }
            else
            {
                var requestContent = await request.Content.ReadAsStringAsync().ConfigureAwait(false);
                var responseContent = await _contentHandler.Invoke(requestContent).ConfigureAwait(false);

                var response = new HttpResponseMessage
                {
                    RequestMessage = request,
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(responseContent, Encoding.UTF8, _mediaType),
                    Version = new Version(1, 1)
                };

                response.Content.Headers.ContentType = new MediaTypeHeaderValue(_mediaType);

                return response;
            }
        }
    }
}