using System;
using System.Collections.Generic;
using System.Data.JsonRpc;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Community.RandomOrg.Data;
using Community.RandomOrg.Internal;
using Community.RandomOrg.Resources;

namespace Community.RandomOrg
{
    /// <summary>A RANDOM.ORG service client.</summary>
    public sealed partial class RandomOrgClient : IDisposable
    {
        private static readonly MediaTypeHeaderValue _mediaTypeHeaderValue = new MediaTypeHeaderValue("application/json");
        private static readonly JsonRpcSerializer _jsonRpcSerializer = new JsonRpcSerializer(null, new Dictionary<JsonRpcId, JsonRpcResponseContract>(1));
        private static readonly Uri _serviceUri = new Uri("https://api.random.org/json-rpc/2/invoke", UriKind.Absolute);
        private static readonly IReadOnlyDictionary<Type, JsonRpcResponseContract> _contracts = CreateContracts();

        private readonly string _apiKey;
        private readonly HttpMessageInvoker _httpMessageInvoker;
        private readonly SemaphoreSlim _requestSemaphore = new SemaphoreSlim(1, 1);

        private DateTime? _advisoryTime;

        /// <summary>Initializes a new instance of the <see cref="RandomOrgClient" /> class.</summary>
        /// <param name="apiKey">The API key, which is used to track the true random bit usage for the client.</param>
        /// <param name="httpMessageInvoker">The component for sending HTTP requests.</param>
        /// <exception cref="ArgumentException"><paramref name="apiKey" /> is not of UUID format.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="apiKey" /> is <see langword="null" />.</exception>
        public RandomOrgClient(string apiKey, HttpMessageInvoker httpMessageInvoker = null)
        {
            if (apiKey == null)
            {
                throw new ArgumentNullException(nameof(apiKey));
            }
            if (!Guid.TryParseExact(apiKey, "D", out var _))
            {
                throw new ArgumentException(Strings.GetString("client.api_key.invalid_format"), nameof(apiKey));
            }

            _apiKey = apiKey;
            _httpMessageInvoker = httpMessageInvoker ?? CreateHttpMessageInvoker();
        }

        /// <summary>Returns information related to the usage of a given API key as an asynchronous operation.</summary>
        /// <param name="cancellationToken">The cancellation token for canceling the operation.</param>
        /// <returns>A <see cref="RandomUsage" /> instance.</returns>
        /// <exception cref="OperationCanceledException">The operation was canceled.</exception>
        /// <exception cref="RandomOrgContractException">An error occurred during service result handling.</exception>
        /// <exception cref="RandomOrgException">An error occurred during service method invocation.</exception>
        /// <exception cref="RandomOrgRequestException">An error occurred during HTTP request execution.</exception>
        public async Task<RandomUsage> GetUsageAsync(CancellationToken cancellationToken = default)
        {
            var parameters = new Dictionary<string, object>(1, StringComparer.Ordinal)
            {
                ["apiKey"] = _apiKey
            };

            var response = await InvokeAccountServiceMethodAsync<RpcRandomUsageResult>("getUsage", parameters, cancellationToken).ConfigureAwait(false);

            return new RandomUsage(response.Status, response.BitsLeft, response.RequestsLeft);
        }

        /// <summary>Verifies the signature of signed random objects and associated data.</summary>
        /// <typeparam name="TValue">The type of random object.</typeparam>
        /// <typeparam name="TParameters">The type of random parameters.</typeparam>
        /// <param name="random">The signed random objects and associated data.</param>
        /// <param name="signature">The signature from the same response that the random data originates from.</param>
        /// <param name="cancellationToken">The cancellation token for canceling the operation.</param>
        /// <returns>A value, indicating if the random objects are authentic.</returns>
        /// <exception cref="ArgumentException"><paramref name="random" /> data is <see langword="null" />.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="random" /> or <paramref name="signature" /> is <see langword="null" />.</exception>
        /// <exception cref="OperationCanceledException">The operation was canceled.</exception>
        /// <exception cref="RandomOrgContractException">An error occurred during service result handling.</exception>
        /// <exception cref="RandomOrgException">An error occurred during service method invocation.</exception>
        /// <exception cref="RandomOrgRequestException">An error occurred during HTTP request execution.</exception>
        public async Task<bool> VerifySignatureAsync<TValue, TParameters>(SignedRandom<TValue, TParameters> random, byte[] signature, CancellationToken cancellationToken = default)
            where TParameters : RandomParameters, new()
        {
            if (random == null)
            {
                throw new ArgumentNullException(nameof(random));
            }
            if (signature == null)
            {
                throw new ArgumentNullException(nameof(signature));
            }
            if (random.Data == null)
            {
                throw new ArgumentException(Strings.GetString("client.verify.data.not_specified"), nameof(random));
            }

            var rpcRandomParam = default(object);

            switch (random)
            {
                case SignedRandom<int, IntegerParameters> x:
                    {
                        var rpcRandom = new RpcIntegersRandom
                        {
                            Method = "generateSignedIntegers",
                            Count = x.Data.Count,
                            Minimum = x.Parameters.Minimum,
                            Maximum = x.Parameters.Maximum,
                            Replacement = x.Parameters.Replacement,
                            Base = 10,
                            UserData = x.UserData,
                            Data = x.Data
                        };

                        TransferSignedRandomData(x, rpcRandom);

                        rpcRandomParam = rpcRandom;
                    }
                    break;
                case SignedRandom<int[], IntegerSequenceParameters> x:
                    {
                        if (x.Parameters.Minimums == null)
                        {
                            throw new ArgumentException(Strings.GetString("random.sequence.minimums.not_specified"), nameof(random));
                        }
                        if (x.Parameters.Maximums == null)
                        {
                            throw new ArgumentException(Strings.GetString("random.sequence.maximums.not_specified"), nameof(random));
                        }
                        if (x.Parameters.Replacements == null)
                        {
                            throw new ArgumentException(Strings.GetString("random.sequence.replacements.not_specified"), nameof(random));
                        }

                        if ((x.Data.Count != x.Parameters.Minimums.Count) ||
                            (x.Data.Count != x.Parameters.Maximums.Count) ||
                            (x.Data.Count != x.Parameters.Replacements.Count))
                        {
                            throw new ArgumentException(Strings.GetString("random.sequence.arguments.different_size"), nameof(random));
                        }

                        var counts = new int[x.Data.Count];
                        var minimums = new int[x.Parameters.Minimums.Count];
                        var maximums = new int[x.Parameters.Maximums.Count];
                        var replacements = new bool[x.Parameters.Replacements.Count];
                        var bases = new int[counts.Length];

                        for (var i = 0; i < counts.Length; i++)
                        {
                            if (x.Data[i] != null)
                            {
                                counts[i] = x.Data[i].Length;
                            }

                            minimums[i] = x.Parameters.Minimums[i];
                            maximums[i] = x.Parameters.Maximums[i];
                            replacements[i] = x.Parameters.Replacements[i];
                            bases[i] = 10;
                        }

                        var rpcRandom = new RpcIntegerSequencesRandom
                        {
                            Method = "generateSignedIntegerSequences",
                            Counts = counts,
                            Minimums = minimums,
                            Maximums = maximums,
                            Replacements = replacements,
                            Bases = bases,
                            UserData = x.UserData,
                            Data = x.Data
                        };

                        TransferSignedRandomData(x, rpcRandom);

                        rpcRandomParam = rpcRandom;
                    }
                    break;
                case SignedRandom<decimal, DecimalFractionParameters> x:
                    {
                        var rpcRandom = new RpcDecimalFractionsRandom
                        {
                            Method = "generateSignedDecimalFractions",
                            Count = x.Data.Count,
                            DecimalPlaces = x.Parameters.DecimalPlaces,
                            Replacement = x.Parameters.Replacement,
                            UserData = x.UserData,
                            Data = x.Data
                        };

                        TransferSignedRandomData(x, rpcRandom);

                        rpcRandomParam = rpcRandom;
                    }
                    break;
                case SignedRandom<decimal, GaussianParameters> x:
                    {
                        var rpcRandom = new RpcGaussiansRandom
                        {
                            Method = "generateSignedGaussians",
                            Count = x.Data.Count,
                            Mean = x.Parameters.Mean,
                            StandardDeviation = x.Parameters.StandardDeviation,
                            SignificantDigits = x.Parameters.SignificantDigits,
                            UserData = x.UserData,
                            Data = x.Data
                        };

                        TransferSignedRandomData(x, rpcRandom);

                        rpcRandomParam = rpcRandom;
                    }
                    break;
                case SignedRandom<string, StringParameters> x:
                    {
                        var rpcRandom = new RpcStringsRandom
                        {
                            Method = "generateSignedStrings",
                            Count = x.Data.Count,
                            Length = x.Parameters.Length,
                            Characters = x.Parameters.Characters,
                            Replacement = x.Parameters.Replacement,
                            UserData = x.UserData,
                            Data = x.Data
                        };

                        TransferSignedRandomData(x, rpcRandom);

                        rpcRandomParam = rpcRandom;
                    }
                    break;
                case SignedRandom<Guid, UuidParameters> x:
                    {
                        var rpcRandom = new RpcUuidsRandom
                        {
                            Method = "generateSignedUUIDs",
                            Count = x.Data.Count,
                            UserData = x.UserData,
                            Data = x.Data
                        };

                        TransferSignedRandomData(x, rpcRandom);

                        rpcRandomParam = rpcRandom;
                    }
                    break;
                case SignedRandom<byte[], BlobParameters> x:
                    {
                        var rpcData = new string[x.Data.Count];

                        for (var i = 0; i < rpcData.Length; i++)
                        {
                            if (x.Data[i] != null)
                            {
                                rpcData[i] = Convert.ToBase64String(x.Data[i]);
                            }
                        }

                        var rpcRandom = new RpcBlobsRandom
                        {
                            Method = "generateSignedBlobs",
                            Count = x.Data.Count,
                            Size = x.Parameters.Size * 8,
                            Format = "base64",
                            UserData = x.UserData,
                            Data = rpcData
                        };

                        TransferSignedRandomData(x, rpcRandom);

                        rpcRandomParam = rpcRandom;
                    }
                    break;
                default:
                    {
                        throw new NotSupportedException(Strings.GetString("client.verify.random.invalid_type"));
                    }
            }

            var parameters = new Dictionary<string, object>(2, StringComparer.Ordinal)
            {
                ["random"] = rpcRandomParam,
                ["signature"] = Convert.ToBase64String(signature)
            };

            var response = await InvokeAccountServiceMethodAsync<RpcVerifyResult>(
                "verifySignature", parameters, cancellationToken).ConfigureAwait(false);

            return response.Authenticity;
        }

        private static void TransferSignedRandomData<TSource, TTarget, TParameters>(SignedRandom<TSource, TParameters> source, RpcSignedRandom<TTarget> target)
            where TParameters : RandomParameters, new()
        {
            target.ApiKeyHash = source.ApiKeyHash;
            target.CompletionTime = source.CompletionTime;
            target.SerialNumber = source.SerialNumber;
            target.License.Type = source.License.Type;
            target.License.Text = source.License.Text;

            if (source.License.InfoUrl != null)
            {
                target.License.InfoUrl = source.License.InfoUrl.OriginalString;
            }
        }

        private static void TransferSignedRandomData<TSource, TTarget, TParameters>(RpcSignedRandom<TSource> source, SignedRandom<TTarget, TParameters> target)
            where TParameters : RandomParameters, new()
        {
            target.ApiKeyHash = source.ApiKeyHash;
            target.CompletionTime = source.CompletionTime;
            target.SerialNumber = source.SerialNumber;
            target.UserData = source.UserData;
            target.License.Type = source.License.Type;
            target.License.Text = source.License.Text;

            if (source.License.InfoUrl != null)
            {
                target.License.InfoUrl = new Uri(source.License.InfoUrl);
            }
        }

        private async Task<TResult> InvokeGenerationServiceMethodAsync<TResult, TRandom, TValue>(string method, IReadOnlyDictionary<string, object> parameters, CancellationToken cancellationToken)
            where TResult : RpcRandomResultObject<TRandom, TValue>
            where TRandom : RpcRandomObject<TValue>
        {
            await _requestSemaphore.WaitAsync(cancellationToken).ConfigureAwait(false);

            try
            {
                if (_advisoryTime != null)
                {
                    var advisoryDelay = _advisoryTime.Value - DateTime.UtcNow;

                    if (advisoryDelay.Ticks > 0)
                    {
                        await Task.Delay(advisoryDelay, cancellationToken).ConfigureAwait(false);
                    }
                }

                var result = await InvokeServiceMethodAsync<TResult>(method, parameters, cancellationToken).ConfigureAwait(false);

                _advisoryTime = result.Random.CompletionTime + TimeSpan.FromMilliseconds(result.AdvisoryDelay);

                return result;
            }
            finally
            {
                _requestSemaphore.Release();
            }
        }

        private async Task<TResult> InvokeAccountServiceMethodAsync<TResult>(string method, IReadOnlyDictionary<string, object> parameters, CancellationToken cancellationToken)
            where TResult : RpcMethodResult
        {
            await _requestSemaphore.WaitAsync(cancellationToken).ConfigureAwait(false);

            try
            {
                return await InvokeServiceMethodAsync<TResult>(method, parameters, cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                _requestSemaphore.Release();
            }
        }

        private async Task<TResult> InvokeServiceMethodAsync<TResult>(string method, IReadOnlyDictionary<string, object> parameters, CancellationToken cancellationToken)
            where TResult : RpcMethodResult
        {
            var jsonRpcRequest = new JsonRpcRequest(method, new JsonRpcId(Guid.NewGuid().ToString("D")), parameters);

            using (var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, _serviceUri))
            {
                var httpRequestString = _jsonRpcSerializer.SerializeRequest(jsonRpcRequest);

                cancellationToken.ThrowIfCancellationRequested();

                var httpRequestContent = new StringContent(httpRequestString);

                httpRequestContent.Headers.ContentType = _mediaTypeHeaderValue;
                httpRequestMessage.Content = httpRequestContent;

                using (var httpResponseMessage = await _httpMessageInvoker.SendAsync(httpRequestMessage, cancellationToken).ConfigureAwait(false))
                {
                    if (!httpResponseMessage.IsSuccessStatusCode)
                    {
                        throw new RandomOrgRequestException(Strings.GetString("protocol.http.status_code.invalid_value"), httpResponseMessage.StatusCode);
                    }

                    var contentType = httpResponseMessage.Content.Headers.ContentType;

                    if ((contentType == null) || (string.Compare(contentType.MediaType, _mediaTypeHeaderValue.MediaType, StringComparison.OrdinalIgnoreCase) != 0))
                    {
                        throw new RandomOrgRequestException(Strings.GetString("protocol.http.headers.invalid_set"), httpResponseMessage.StatusCode);
                    }

                    var contentLength = httpResponseMessage.Content.Headers.ContentLength;

                    if (contentLength == null)
                    {
                        throw new RandomOrgRequestException(Strings.GetString("protocol.http.headers.invalid_set"), httpResponseMessage.StatusCode);
                    }

                    var httpResponseString = await httpResponseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);

                    cancellationToken.ThrowIfCancellationRequested();

                    if (httpResponseString?.Length != contentLength)
                    {
                        throw new RandomOrgRequestException(Strings.GetString("protocol.http.headers.invalid_set"), httpResponseMessage.StatusCode);
                    }

                    _jsonRpcSerializer.DynamicResponseBindings[jsonRpcRequest.Id] = _contracts[typeof(TResult)];

                    var responseData = default(JsonRpcData<JsonRpcResponse>);

                    try
                    {
                        responseData = _jsonRpcSerializer.DeserializeResponseData(httpResponseString);
                    }
                    catch (JsonRpcException e)
                    {
                        throw new RandomOrgContractException(method, Strings.GetString("protocol.rpc.message.invalid_value"), e);
                    }
                    finally
                    {
                        _jsonRpcSerializer.DynamicResponseBindings.Remove(jsonRpcRequest.Id);
                    }

                    cancellationToken.ThrowIfCancellationRequested();

                    if (!responseData.IsSingle)
                    {
                        throw new RandomOrgContractException(method, Strings.GetString("protocol.random.message.invalid_value"));
                    }

                    var jsonRpcItem = responseData.SingleItem;

                    if (!jsonRpcItem.IsValid)
                    {
                        throw new RandomOrgContractException(method, Strings.GetString("protocol.random.message.invalid_value"), jsonRpcItem.Exception);
                    }

                    var jsonRpcResponse = jsonRpcItem.Message;

                    if (!jsonRpcResponse.Success)
                    {
                        throw new RandomOrgException(method, jsonRpcResponse.Error.Code, jsonRpcResponse.Error.Message);
                    }
                    if (jsonRpcRequest.Id != jsonRpcResponse.Id)
                    {
                        throw new RandomOrgContractException(method, Strings.GetString("protocol.rpc.id.invalid_value"));
                    }
                    if (jsonRpcResponse.Result == null)
                    {
                        throw new RandomOrgContractException(method, Strings.GetString("protocol.random.message.invalid_value"));
                    }

                    return (TResult)jsonRpcResponse.Result;
                }
            }
        }

        private static HttpMessageInvoker CreateHttpMessageInvoker()
        {
            var httpHandler = new HttpClientHandler
            {
                AllowAutoRedirect = false,
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
            };

            var httpClient = new HttpClient(httpHandler);

            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(_mediaTypeHeaderValue.MediaType));
            httpClient.DefaultRequestHeaders.ExpectContinue = false;
            httpClient.Timeout = TimeSpan.FromMinutes(2);

            return httpClient;
        }

        private static IReadOnlyDictionary<Type, JsonRpcResponseContract> CreateContracts()
        {
            var types = new[]
            {
                typeof(RpcRandomUsageResult),
                typeof(RpcRandomResult<int>),
                typeof(RpcRandomResult<int[]>),
                typeof(RpcRandomResult<string>),
                typeof(RpcRandomResult<decimal>),
                typeof(RpcRandomResult<Guid>),
                typeof(RpcSignedRandomResult<RpcIntegersRandom, int>),
                typeof(RpcSignedRandomResult<RpcIntegerSequencesRandom, int[]>),
                typeof(RpcSignedRandomResult<RpcDecimalFractionsRandom, decimal>),
                typeof(RpcSignedRandomResult<RpcGaussiansRandom, decimal>),
                typeof(RpcSignedRandomResult<RpcStringsRandom, string>),
                typeof(RpcSignedRandomResult<RpcUuidsRandom, Guid>),
                typeof(RpcSignedRandomResult<RpcBlobsRandom, string>),
                typeof(RpcVerifyResult)
            };

            var result = new Dictionary<Type, JsonRpcResponseContract>(types.Length);

            for (var i = 0; i < types.Length; i++)
            {
                result[types[i]] = new JsonRpcResponseContract(types[i]);
            }

            return result;
        }

        /// <summary>Releases all resources used by the current instance of the <see cref="RandomOrgClient" />.</summary>
        public void Dispose()
        {
            _httpMessageInvoker.Dispose();
            _requestSemaphore.Dispose();
            _jsonRpcSerializer.Dispose();
        }
    }
}