using System;
using System.Collections.Generic;
using System.Data.JsonRpc;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Community.RandomOrg.Data;

namespace Community.RandomOrg
{
    /// <summary>RANDOM.ORG service client.</summary>
    public sealed partial class RandomOrgClient : IDisposable
    {
        private const string _HTTP_MEDIA_TYPE = "application/json";
        private const string _RPC_GET_USAGE = "getUsage";
        private const string _RPC_VERIFY_SIGNATUREE = "verifySignature";
        private const string _RPC_GENERATE_PURE_INTEGERS = "generateIntegers";
        private const string _RPC_GENERATE_PURE_DECIMAL_FRACTIONS = "generateDecimalFractions";
        private const string _RPC_GENERATE_PURE_GAUSSIANS = "generateGaussians";
        private const string _RPC_GENERATE_PURE_STRINGS = "generateStrings";
        private const string _RPC_GENERATE_PURE_UUIDS = "generateUUIDs";
        private const string _RPC_GENERATE_PURE_BLOBS = "generateBlobs";
        private const string _RPC_GENERATE_SIGNED_INTEGERS = "generateSignedIntegers";
        private const string _RPC_GENERATE_SIGNED_DECIMAL_FRACTIONS = "generateSignedDecimalFractions";
        private const string _RPC_GENERATE_SIGNED_GAUSSIANS = "generateSignedGaussians";
        private const string _RPC_GENERATE_SIGNED_STRINGS = "generateSignedStrings";
        private const string _RPC_GENERATE_SIGNED_UUIDS = "generateSignedUUIDs";
        private const string _RPC_GENERATE_SIGNED_BLOBS = "generateSignedBlobs";

        private static readonly ResourceManager _resourceManager = CreateResourceManager();
        private static readonly JsonRpcSerializer _jsonRpcSerializer = CreateJsonRpcSerializer();
        private static readonly Uri _serviceUri = new Uri("https://api.random.org/json-rpc/1/invoke", UriKind.Absolute);

        private readonly string _apiKey;
        private readonly HttpMessageInvoker _httpMessageInvoker;
        private readonly SemaphoreSlim _requestSemaphore = new SemaphoreSlim(1, 1);

        private DateTime? _advisoryTime;

        /// <summary>Initializes a new instance of the <see cref="RandomOrgClient" /> class.</summary>
        /// <param name="apiKey">The API key, which is used to track the true random bit usage for the client.</param>
        /// <param name="httpMessageInvoker">The component for sending HTTP requests.</param>
        /// <exception cref="ArgumentNullException"><paramref name="apiKey" /> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentException"><paramref name="apiKey" /> is not of UUID format.</exception>
        public RandomOrgClient(string apiKey, HttpMessageInvoker httpMessageInvoker = null)
        {
            if (apiKey == null)
            {
                throw new ArgumentNullException(nameof(apiKey));
            }
            if (!Guid.TryParseExact(apiKey, "D", out var apiKeyGuid))
            {
                throw new ArgumentException(_resourceManager.GetString("ApiKeyFormatError"), nameof(apiKey));
            }

            _apiKey = apiKey;
            _httpMessageInvoker = httpMessageInvoker ?? CreateHttpMessageInvoker();
        }

        /// <summary>Returns information related to the usage of a given API key as an asynchronous operation.</summary>
        /// <returns>A <see cref="RandomUsage" /> instance.</returns>
        public Task<RandomUsage> GetUsageAsync()
        {
            return GetUsageAsync(CancellationToken.None);
        }

        /// <summary>Returns information related to the usage of a given API key as an asynchronous operation.</summary>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <returns>A <see cref="RandomUsage" /> instance.</returns>
        public async Task<RandomUsage> GetUsageAsync(CancellationToken cancellationToken)
        {
            var @params = new RpcGetUsageParams
            {
                ApiKey = _apiKey
            };

            var result = await InvokeRandomOrgMethod<RpcGetUsageResult>(_RPC_GET_USAGE, @params, cancellationToken).ConfigureAwait(false);

            return new RandomUsage(result.Status, result.CreationTime, result.TotalBits, result.BitsLeft, result.TotalRequests, result.RequestsLeft);
        }

        private static void TransferValues<T>(SignedRandom<T> source, RpcSignedRandom<T> target)
        {
            target.Count = source.Data.Count;
            target.ApiKeyHash = source.ApiKeyHash;
            target.Data = source.Data;
            target.CompletionTime = source.CompletionTime;
            target.SerialNumber = source.SerialNumber;
        }

        private static void TransferValues<T>(RpcSignedRandom<T> source, SignedRandom<T> target)
        {
            target.ApiKeyHash = source.ApiKeyHash;
            target.Data = source.Data;
            target.CompletionTime = source.CompletionTime;
            target.SerialNumber = source.SerialNumber;
        }

        private static void TransferValues<T>(RpcRandom<T> source, Random<T> target)
        {
            target.Data = source.Data;
            target.CompletionTime = source.CompletionTime;
        }

        private async Task<T> InvokeRandomOrgMethod<T>(string method, RpcMethodParams @params, CancellationToken cancellationToken)
            where T : RpcMethodResult
        {
            await _requestSemaphore.WaitAsync().ConfigureAwait(false);

            try
            {
                var advisoryDelayAware = typeof(IAdvisoryDelayAware).GetTypeInfo().IsAssignableFrom(typeof(T).GetTypeInfo());

                if (advisoryDelayAware && _advisoryTime.HasValue)
                {
                    var advisoryDelay = _advisoryTime.Value - DateTime.UtcNow;

                    if (advisoryDelay.Ticks > 0)
                    {
                        await Task.Delay(advisoryDelay).ConfigureAwait(false);
                    }
                }

                var jsonRpcRequest = new JsonRpcRequest(method, Guid.NewGuid().ToString(), @params);

                var bindings = new Dictionary<JsonRpcId, string>(1)
                {
                    [jsonRpcRequest.Id] = jsonRpcRequest.Method
                };
                var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, _serviceUri)
                {
                    Content = new StringContent(_jsonRpcSerializer.SerializeRequest(jsonRpcRequest), Encoding.UTF8, _HTTP_MEDIA_TYPE)
                };

                var httpResponseMessage = await _httpMessageInvoker.SendAsync(httpRequestMessage, cancellationToken).ConfigureAwait(false);

                httpResponseMessage.EnsureSuccessStatusCode();

                var mediaType = httpResponseMessage.Content.Headers.ContentType?.MediaType;

                if (string.Compare(mediaType, _HTTP_MEDIA_TYPE, StringComparison.OrdinalIgnoreCase) != 0)
                {
                    throw new InvalidOperationException(_resourceManager.GetString("MediaTypeError"));
                }

                var httpResponseContent = await httpResponseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);
                var responseData = _jsonRpcSerializer.DeserializeResponsesData(httpResponseContent, bindings);
                var message = responseData.GetSingleItem().GetMessage();

                if (!message.Success)
                {
                    if (advisoryDelayAware)
                    {
                        _advisoryTime = null;
                    }

                    throw new RandomOrgException(message.Error.Code, message.Error.Message);
                }

                var result = (T)message.Result;

                if (result is IAdvisoryDelayAware advisoryDelayInfo)
                {
                    if (advisoryDelayInfo.AdvisoryDelay.Ticks > 0)
                    {
                        _advisoryTime = DateTime.UtcNow + advisoryDelayInfo.AdvisoryDelay;
                    }
                    else
                    {
                        _advisoryTime = null;
                    }
                }

                return result;
            }
            finally
            {
                _requestSemaphore.Release();
            }
        }

        private static HttpMessageInvoker CreateHttpMessageInvoker()
        {
            var httpClientHandler = new HttpClientHandler
            {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
            };

            var httpClient = new HttpClient(httpClientHandler);

            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(_HTTP_MEDIA_TYPE));
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
            scheme.Methods[_RPC_GENERATE_PURE_INTEGERS] =
                new JsonRpcMethodScheme(typeof(RpcRandomResult<int>), typeof(object[]));
            scheme.Methods[_RPC_GENERATE_PURE_DECIMAL_FRACTIONS] =
                new JsonRpcMethodScheme(typeof(RpcRandomResult<decimal>), typeof(object[]));
            scheme.Methods[_RPC_GENERATE_PURE_GAUSSIANS] =
                new JsonRpcMethodScheme(typeof(RpcRandomResult<decimal>), typeof(object[]));
            scheme.Methods[_RPC_GENERATE_PURE_STRINGS] =
                new JsonRpcMethodScheme(typeof(RpcRandomResult<string>), typeof(object[]));
            scheme.Methods[_RPC_GENERATE_PURE_UUIDS] =
                new JsonRpcMethodScheme(typeof(RpcRandomResult<Guid>), typeof(object[]));
            scheme.Methods[_RPC_GENERATE_PURE_BLOBS] =
                new JsonRpcMethodScheme(typeof(RpcRandomResult<string>), typeof(object[]));
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

        private static ResourceManager CreateResourceManager()
        {
            var assembly = typeof(RandomOrgClient).GetTypeInfo().Assembly;

            return new ResourceManager($"{assembly.GetName().Name}.Resources.Strings", assembly);
        }

        /// <summary>Releases the unmanaged resources and disposes of the managed resources used by the component for sending HTTP requests.</summary>
        public void Dispose()
        {
            _httpMessageInvoker.Dispose();
        }
    }
}