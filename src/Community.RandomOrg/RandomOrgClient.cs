using System;
using System.Collections.Generic;
using System.Data.JsonRpc;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Community.RandomOrg.Data;

namespace Community.RandomOrg
{
    /// <summary>A RANDOM.ORG service client.</summary>
    public sealed partial class RandomOrgClient : IDisposable
    {
        private const string _RPC_GENERATE_SIMPLE_INTEGERS = "generateIntegers";
        private const string _RPC_GENERATE_SIMPLE_DECIMAL_FRACTIONS = "generateDecimalFractions";
        private const string _RPC_GENERATE_SIMPLE_GAUSSIANS = "generateGaussians";
        private const string _RPC_GENERATE_SIMPLE_STRINGS = "generateStrings";
        private const string _RPC_GENERATE_SIMPLE_UUIDS = "generateUUIDs";
        private const string _RPC_GENERATE_SIMPLE_BLOBS = "generateBlobs";
        private const string _RPC_GENERATE_SIGNED_INTEGERS = "generateSignedIntegers";
        private const string _RPC_GENERATE_SIGNED_DECIMAL_FRACTIONS = "generateSignedDecimalFractions";
        private const string _RPC_GENERATE_SIGNED_GAUSSIANS = "generateSignedGaussians";
        private const string _RPC_GENERATE_SIGNED_STRINGS = "generateSignedStrings";
        private const string _RPC_GENERATE_SIGNED_UUIDS = "generateSignedUUIDs";
        private const string _RPC_GENERATE_SIGNED_BLOBS = "generateSignedBlobs";
        private const string _RPC_GET_RESULT = "getResult";
        private const string _RPC_GET_USAGE = "getUsage";
        private const string _RPC_VERIFY_SIGNATUREE = "verifySignature";

        private static readonly MediaTypeHeaderValue _httpMediaTypeHeader = new MediaTypeHeaderValue("application/json");
        private static readonly JsonRpcSerializer _jsonRpcSerializer = CreateJsonRpcSerializer();
        private static readonly ResourceManager _resourceManager = CreateResourceManager();
        private static readonly Uri _serviceUri = new Uri("https://api.random.org/json-rpc/2/invoke", UriKind.Absolute);
        private static readonly IReadOnlyDictionary<Type, JsonRpcMethodScheme> _signedResultMethodSchemeBindings = CreateSignedResultMethodSchemeBindings();

        private readonly string _apiKey;
        private readonly HttpMessageInvoker _httpMessageInvoker;
        private readonly SemaphoreSlim _requestSemaphore = new SemaphoreSlim(1, 1);
        private readonly Dictionary<JsonRpcId, string> _rpcMethodNameBindings = new Dictionary<JsonRpcId, string>(1);
        private readonly Dictionary<JsonRpcId, JsonRpcMethodScheme> _rpcMethodSchemeBindings = new Dictionary<JsonRpcId, JsonRpcMethodScheme>(1);

        private DateTime? _advisoryTime;

        /// <summary>Initializes a new instance of the <see cref="RandomOrgClient" /> class.</summary>
        /// <param name="apiKey">The API key, which is used to track the true random bit usage for the client.</param>
        /// <exception cref="ArgumentNullException"><paramref name="apiKey" /> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentException"><paramref name="apiKey" /> is not of UUID format.</exception>
        public RandomOrgClient(string apiKey)
        {
            if (apiKey == null)
            {
                throw new ArgumentNullException(nameof(apiKey));
            }
            if (!Guid.TryParseExact(apiKey, "D", out var _))
            {
                throw new ArgumentException(_resourceManager.GetString("Client.ApiKeyFormatIsInvalid"), nameof(apiKey));
            }

            _apiKey = apiKey;
            _httpMessageInvoker = CreateHttpMessageInvoker();
        }

        /// <summary>Initializes a new instance of the <see cref="RandomOrgClient" /> class.</summary>
        /// <param name="httpMessageInvoker">The component for sending HTTP requests.</param>
        /// <exception cref="ArgumentNullException"><paramref name="httpMessageInvoker" /> is <see langword="null" />.</exception>
        public RandomOrgClient(HttpMessageInvoker httpMessageInvoker)
        {
            if (httpMessageInvoker == null)
            {
                throw new ArgumentNullException(nameof(httpMessageInvoker));
            }

            _httpMessageInvoker = httpMessageInvoker;
        }

        /// <summary>Initializes a new instance of the <see cref="RandomOrgClient" /> class.</summary>
        /// <param name="apiKey">The API key, which is used to track the true random bit usage for the client.</param>
        /// <param name="httpMessageInvoker">The component for sending HTTP requests.</param>
        /// <exception cref="ArgumentNullException"><paramref name="apiKey" /> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentException"><paramref name="apiKey" /> is not of UUID format.</exception>
        public RandomOrgClient(string apiKey, HttpMessageInvoker httpMessageInvoker)
        {
            if (apiKey == null)
            {
                throw new ArgumentNullException(nameof(apiKey));
            }
            if (httpMessageInvoker == null)
            {
                throw new ArgumentNullException(nameof(httpMessageInvoker));
            }
            if (!Guid.TryParseExact(apiKey, "D", out var _))
            {
                throw new ArgumentException(_resourceManager.GetString("Client.ApiKeyFormatIsInvalid"), nameof(apiKey));
            }

            _apiKey = apiKey;
            _httpMessageInvoker = httpMessageInvoker;
        }

        /// <summary>Returns information related to the usage of a given API key as an asynchronous operation.</summary>
        /// <returns>A <see cref="RandomUsage" /> instance.</returns>
        /// <exception cref="RandomOrgException">An error occurred during service method invocation.</exception>
        public Task<RandomUsage> GetUsageAsync()
        {
            return GetUsageAsync(CancellationToken.None);
        }

        /// <summary>Returns information related to the usage of a given API key as an asynchronous operation.</summary>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <returns>A <see cref="RandomUsage" /> instance.</returns>
        /// <exception cref="RandomOrgException">An error occurred during service method invocation.</exception>
        /// <exception cref="OperationCanceledException">The operation was canceled.</exception>
        public async Task<RandomUsage> GetUsageAsync(CancellationToken cancellationToken)
        {
            EnsureApiKeyIsSpecified();

            var @params = new RpcGetUsageParams
            {
                ApiKey = _apiKey
            };

            var result = await InvokeRandomOrgMethod<RpcGetUsageResult>(_RPC_GET_USAGE, @params, cancellationToken).ConfigureAwait(false);

            return new RandomUsage(result.Status, result.CreationTime, result.TotalBits, result.BitsLeft, result.TotalRequests, result.RequestsLeft);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void EnsureApiKeyIsSpecified()
        {
            if (_apiKey == null)
            {
                throw new InvalidOperationException(_resourceManager.GetString("Client.ApiKeyIsRequired"));
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void TransferValues<T>(SignedRandom<T> source, RpcSignedRandom<T> target)
        {
            target.Count = source.Data.Count;
            target.ApiKeyHash = source.ApiKeyHash;
            target.Data = source.Data;
            target.CompletionTime = source.CompletionTime;
            target.SerialNumber = source.SerialNumber;
            target.License.Type = source.License.Type;
            target.License.Text = source.License.Text;
            target.License.InfoUrl = source.License.InfoUrl;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void TransferValues<T>(RpcSignedRandom<T> source, SignedRandom<T> target)
        {
            target.ApiKeyHash = source.ApiKeyHash;
            target.Data = source.Data;
            target.CompletionTime = source.CompletionTime;
            target.SerialNumber = source.SerialNumber;
            target.UserData = source.UserData;
            target.License.Type = source.License.Type;
            target.License.Text = source.License.Text;
            target.License.InfoUrl = source.License.InfoUrl;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void TransferValues<T>(RpcRandom<T> source, Random<T> target)
        {
            target.Data = source.Data;
            target.CompletionTime = source.CompletionTime;
        }

        private async Task<TResult> InvokeRandomOrgMethod<TResult, TRandom, TValue>(string method, RpcGenerateParams @params, CancellationToken cancellationToken)
            where TResult : RpcRandomResult<TRandom, TValue>
            where TRandom : RpcRandom<TValue>
        {
            cancellationToken.ThrowIfCancellationRequested();

            await _requestSemaphore.WaitAsync(cancellationToken).ConfigureAwait(false);

            try
            {
                if (_advisoryTime.HasValue)
                {
                    var advisoryDelay = _advisoryTime.Value - DateTime.UtcNow;

                    if (advisoryDelay.Ticks > 0)
                    {
                        await Task.Delay(advisoryDelay, cancellationToken).ConfigureAwait(false);
                    }
                }

                var result = (TResult)await InvokeRandomOrgMethod(method, @params, null, cancellationToken).ConfigureAwait(false);

                _advisoryTime = result.Random.CompletionTime + result.AdvisoryDelay;

                return result;
            }
            finally
            {
                _requestSemaphore.Release();
            }
        }

        private async Task<TResult> InvokeRandomOrgMethod<TResult>(string method, RpcMethodParams @params, CancellationToken cancellationToken)
            where TResult : RpcMethodResult
        {
            cancellationToken.ThrowIfCancellationRequested();

            await _requestSemaphore.WaitAsync(cancellationToken).ConfigureAwait(false);

            try
            {
                return (TResult)await InvokeRandomOrgMethod(method, @params, null, cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                _requestSemaphore.Release();
            }
        }

        private async Task<object> InvokeRandomOrgMethod(string method, object @params, Type resultType, CancellationToken cancellationToken)
        {
            var jsonRpcRequest = new JsonRpcRequest(method, new JsonRpcId(Guid.NewGuid().ToString("D")), @params);
            var httpRequestString = _jsonRpcSerializer.SerializeRequest(jsonRpcRequest);

            var httpResponseString = default(string);

            using (var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, _serviceUri))
            {
                var httpRequestMessageContent = new StringContent(httpRequestString);

                httpRequestMessageContent.Headers.ContentType = _httpMediaTypeHeader;
                httpRequestMessage.Content = httpRequestMessageContent;

                using (var httpResponseMessage = await _httpMessageInvoker.SendAsync(httpRequestMessage, cancellationToken).ConfigureAwait(false))
                {
                    httpResponseMessage.EnsureSuccessStatusCode();

                    var contentType = httpResponseMessage.Content.Headers.ContentType;

                    if (contentType == null)
                    {
                        throw new HttpRequestException(_resourceManager.GetString("Service.ContentTypeIsNotSpecified"));
                    }
                    if (string.Compare(contentType.MediaType, _httpMediaTypeHeader.MediaType, StringComparison.OrdinalIgnoreCase) != 0)
                    {
                        throw new HttpRequestException(_resourceManager.GetString("Service.ContentTypeIsInvalid"));
                    }

                    var contentLength = httpResponseMessage.Content.Headers.ContentLength;

                    if (contentLength == null)
                    {
                        throw new HttpRequestException(_resourceManager.GetString("Service.ContentLengthIsNotSpecified"));
                    }

                    httpResponseString = await httpResponseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);

                    if (httpResponseString?.Length != contentLength)
                    {
                        throw new HttpRequestException(_resourceManager.GetString("Service.ContentLengthIsInvalid"));
                    }
                }
            }

            if (resultType != null)
            {
                _rpcMethodSchemeBindings[jsonRpcRequest.Id] = _signedResultMethodSchemeBindings[resultType];
            }
            else
            {
                _rpcMethodNameBindings[jsonRpcRequest.Id] = jsonRpcRequest.Method;
            }

            var responseData = default(JsonRpcData<JsonRpcResponse>);

            try
            {
                responseData = resultType != null ?
                    _jsonRpcSerializer.DeserializeResponsesData(httpResponseString, _rpcMethodSchemeBindings) :
                    _jsonRpcSerializer.DeserializeResponsesData(httpResponseString, _rpcMethodNameBindings);
            }
            finally
            {
                if (resultType != null)
                {
                    _rpcMethodSchemeBindings.Clear();
                }
                else
                {
                    _rpcMethodNameBindings.Clear();
                }
            }

            var jsonRpcResponse = responseData.GetSingleItem().GetMessage();

            if (!jsonRpcResponse.Success)
            {
                throw new RandomOrgException(jsonRpcResponse.Error.Code, jsonRpcResponse.Error.Message);
            }
            if (jsonRpcRequest.Id != jsonRpcResponse.Id)
            {
                throw new JsonRpcException(_resourceManager.GetString("Service.MessageIdentifierIsInvalid"));
            }

            return jsonRpcResponse.Result;
        }

        private static HttpMessageInvoker CreateHttpMessageInvoker()
        {
            var httpClientHandler = new HttpClientHandler
            {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
            };

            var httpClient = new HttpClient(httpClientHandler);

            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(_httpMediaTypeHeader.MediaType));
            httpClient.DefaultRequestHeaders.ExpectContinue = false;
            httpClient.Timeout = TimeSpan.FromMinutes(2);

            return httpClient;
        }

        private static JsonRpcSerializer CreateJsonRpcSerializer()
        {
            var scheme = new JsonRpcSerializerScheme
            {
                GenericErrorDataType = typeof(object[])
            };

            scheme.Methods[_RPC_GET_USAGE] =
                new JsonRpcMethodScheme(typeof(RpcGetUsageResult), typeof(object[]));
            scheme.Methods[_RPC_VERIFY_SIGNATUREE] =
                new JsonRpcMethodScheme(typeof(RpcVerifyResult), typeof(object[]));
            scheme.Methods[_RPC_GENERATE_SIMPLE_INTEGERS] =
                new JsonRpcMethodScheme(typeof(RpcSimpleRandomResult<int>), typeof(object[]));
            scheme.Methods[_RPC_GENERATE_SIMPLE_DECIMAL_FRACTIONS] =
                new JsonRpcMethodScheme(typeof(RpcSimpleRandomResult<decimal>), typeof(object[]));
            scheme.Methods[_RPC_GENERATE_SIMPLE_GAUSSIANS] =
                new JsonRpcMethodScheme(typeof(RpcSimpleRandomResult<decimal>), typeof(object[]));
            scheme.Methods[_RPC_GENERATE_SIMPLE_STRINGS] =
                new JsonRpcMethodScheme(typeof(RpcSimpleRandomResult<string>), typeof(object[]));
            scheme.Methods[_RPC_GENERATE_SIMPLE_UUIDS] =
                new JsonRpcMethodScheme(typeof(RpcSimpleRandomResult<Guid>), typeof(object[]));
            scheme.Methods[_RPC_GENERATE_SIMPLE_BLOBS] =
                new JsonRpcMethodScheme(typeof(RpcSimpleRandomResult<string>), typeof(object[]));
            scheme.Methods[_RPC_GENERATE_SIGNED_INTEGERS] =
                new JsonRpcMethodScheme(typeof(RpcSignedRandomResult<RpcSignedIntegersRandom, int>), typeof(object[]));
            scheme.Methods[_RPC_GENERATE_SIGNED_DECIMAL_FRACTIONS] =
                new JsonRpcMethodScheme(typeof(RpcSignedRandomResult<RpcSignedDecimalFractionsRandom, decimal>), typeof(object[]));
            scheme.Methods[_RPC_GENERATE_SIGNED_GAUSSIANS] =
                new JsonRpcMethodScheme(typeof(RpcSignedRandomResult<RpcSignedGaussiansRandom, decimal>), typeof(object[]));
            scheme.Methods[_RPC_GENERATE_SIGNED_STRINGS] =
                new JsonRpcMethodScheme(typeof(RpcSignedRandomResult<RpcSignedStringsRandom, string>), typeof(object[]));
            scheme.Methods[_RPC_GENERATE_SIGNED_UUIDS] =
                new JsonRpcMethodScheme(typeof(RpcSignedRandomResult<RpcSignedUuidsRandom, Guid>), typeof(object[]));
            scheme.Methods[_RPC_GENERATE_SIGNED_BLOBS] =
                new JsonRpcMethodScheme(typeof(RpcSignedRandomResult<RpcSignedBlobsRandom, string>), typeof(object[]));

            var settings = new JsonRpcSerializerSettings
            {
                JsonSerializerArrayPool = new JsonArrayPool()
            };

            return new JsonRpcSerializer(scheme, settings);
        }

        private static IReadOnlyDictionary<Type, JsonRpcMethodScheme> CreateSignedResultMethodSchemeBindings()
        {
            return new Dictionary<Type, JsonRpcMethodScheme>(6)
            {
                [typeof(RpcSignedRandomResult<RpcSignedIntegersRandom, int>)] =
                    new JsonRpcMethodScheme(typeof(RpcSignedRandomResult<RpcSignedIntegersRandom, int>), typeof(object[])),
                [typeof(RpcSignedRandomResult<RpcSignedDecimalFractionsRandom, decimal>)] =
                    new JsonRpcMethodScheme(typeof(RpcSignedRandomResult<RpcSignedDecimalFractionsRandom, decimal>), typeof(object[])),
                [typeof(RpcSignedRandomResult<RpcSignedGaussiansRandom, decimal>)] =
                    new JsonRpcMethodScheme(typeof(RpcSignedRandomResult<RpcSignedGaussiansRandom, decimal>), typeof(object[])),
                [typeof(RpcSignedRandomResult<RpcSignedStringsRandom, string>)] =
                    new JsonRpcMethodScheme(typeof(RpcSignedRandomResult<RpcSignedStringsRandom, string>), typeof(object[])),
                [typeof(RpcSignedRandomResult<RpcSignedUuidsRandom, Guid>)] =
                    new JsonRpcMethodScheme(typeof(RpcSignedRandomResult<RpcSignedUuidsRandom, Guid>), typeof(object[])),
                [typeof(RpcSignedRandomResult<RpcSignedBlobsRandom, string>)] =
                    new JsonRpcMethodScheme(typeof(RpcSignedRandomResult<RpcSignedBlobsRandom, string>), typeof(object[]))
            };
        }

        private static ResourceManager CreateResourceManager()
        {
            var assembly = typeof(RandomOrgClient).GetTypeInfo().Assembly;

            return new ResourceManager($"{assembly.GetName().Name}.Resources.Strings", assembly);
        }

        /// <summary>Releases all resources used by the current instance of the <see cref="RandomOrgClient" />.</summary>
        public void Dispose()
        {
            _httpMessageInvoker.Dispose();
            _requestSemaphore.Dispose();
        }
    }
}