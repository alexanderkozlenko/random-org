// © Alexander Kozlenko. Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Anemonis.JsonRpc;
using Anemonis.RandomOrg.Converters;
using Anemonis.RandomOrg.Data;
using Anemonis.RandomOrg.DataRpc;
using Anemonis.RandomOrg.Resources;

using Newtonsoft.Json;

namespace Anemonis.RandomOrg
{
    /// <summary>Represents RANDOM.ORG service client.</summary>
    public sealed partial class RandomOrgClient : IDisposable
    {
        private static readonly MediaTypeHeaderValue _mediaTypeHeaderValue = MediaTypeHeaderValue.Parse("application/json; charset=utf-8");
        private static readonly MediaTypeWithQualityHeaderValue _mediaTypeWithQualityHeaderValue = MediaTypeWithQualityHeaderValue.Parse("application/json; charset=utf-8");
        private static readonly Uri _serviceUri = new Uri("https://api.random.org/json-rpc/2/invoke", UriKind.Absolute);
        private static readonly JsonSerializer _jsonSerializer = CreateJsonSerializer();
        private static readonly Dictionary<string, JsonRpcResponseContract> _responseContracts = CreateJsonRpcContracts();

        private readonly string _apiKey;
        private readonly SemaphoreSlim _invocationSemaphore = new SemaphoreSlim(1, 1);
        private readonly SpinLock _advisoryTimeLock = new SpinLock(false);
        private readonly JsonRpcContractResolver _jsonRpcContractResolver = CreateJsonRpcContractResolver();
        private readonly JsonRpcSerializer _jsonRpcSerializer;
        private readonly HttpMessageInvoker _httpInvoker;

        private DateTime _advisoryTime = new DateTime(0L, DateTimeKind.Utc);

        /// <summary>Initializes a new instance of the <see cref="RandomOrgClient" /> class.</summary>
        /// <param name="apiKey">The API key, which is used to track the true random bit usage for the client.</param>
        /// <param name="httpInvoker">The component for sending HTTP requests.</param>
        /// <exception cref="ArgumentException"><paramref name="apiKey" /> is not of UUID format (32 digits separated by hyphens).</exception>
        /// <exception cref="ArgumentNullException"><paramref name="apiKey" /> or <paramref name="httpInvoker" /> is <see langword="null" />.</exception>
        public RandomOrgClient(string apiKey, HttpMessageInvoker httpInvoker)
        {
            if (apiKey == null)
            {
                throw new ArgumentNullException(nameof(apiKey));
            }
            if (!Guid.TryParseExact(apiKey, "D", out _))
            {
                throw new ArgumentException(Strings.GetString("client.api_key.invalid_format"), nameof(apiKey));
            }
            if (httpInvoker == null)
            {
                throw new ArgumentNullException(nameof(httpInvoker));
            }

            _apiKey = apiKey;
            _httpInvoker = httpInvoker;
            _jsonRpcSerializer = new JsonRpcSerializer(_jsonRpcContractResolver, _jsonSerializer);
        }

        /// <summary>Initializes a new instance of the <see cref="RandomOrgClient" /> class.</summary>
        /// <param name="apiKey">The API key, which is used to track the true random bit usage for the client.</param>
        /// <exception cref="ArgumentException"><paramref name="apiKey" /> is not of UUID format (32 digits separated by hyphens).</exception>
        /// <exception cref="ArgumentNullException"><paramref name="apiKey" /> is <see langword="null" />.</exception>
        public RandomOrgClient(string apiKey)
            : this(apiKey, CreateHttpInvoker())
        {
        }

        private static JsonSerializer CreateJsonSerializer()
        {
            var settings = new JsonSerializerSettings
            {
                MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
                DateFormatString = "yyyy-MM-dd HH:mm:ss.FFFFFFFK",
                DateTimeZoneHandling = DateTimeZoneHandling.Utc
            };

            settings.Converters.Add(new RandomOrgFloatConverter());
            settings.Converters.Add(new ApiKeyStatusConverter());

            return JsonSerializer.Create(settings);
        }

        private static HttpMessageInvoker CreateHttpInvoker()
        {
            var httpHandler = new HttpClientHandler
            {
                AllowAutoRedirect = false,
                AutomaticDecompression = DecompressionMethods.GZip
            };

            var httpClient = new HttpClient(httpHandler);

            httpClient.DefaultRequestHeaders.Accept.Add(_mediaTypeWithQualityHeaderValue);
            httpClient.DefaultRequestHeaders.ExpectContinue = false;
            httpClient.Timeout = TimeSpan.FromMinutes(2);

            return httpClient;
        }

        private static JsonRpcContractResolver CreateJsonRpcContractResolver()
        {
            var resolver = new JsonRpcContractResolver();
            var enumerator = _responseContracts.GetEnumerator();

            while (enumerator.MoveNext())
            {
                resolver.AddResponseContract(enumerator.Current.Key, enumerator.Current.Value);
            }

            return resolver;
        }

        private static Dictionary<string, JsonRpcResponseContract> CreateJsonRpcContracts()
        {
            return new Dictionary<string, JsonRpcResponseContract>(16, StringComparer.Ordinal)
            {
                ["getUsage"] = new JsonRpcResponseContract(typeof(RpcRandomUsageResult)),
                ["generateIntegers"] = new JsonRpcResponseContract(typeof(RpcRandomResult<int>)),
                ["generateIntegerSequences"] = new JsonRpcResponseContract(typeof(RpcRandomResult<IReadOnlyList<int>>)),
                ["generateDecimalFractions"] = new JsonRpcResponseContract(typeof(RpcRandomResult<decimal>)),
                ["generateGaussians"] = new JsonRpcResponseContract(typeof(RpcRandomResult<decimal>)),
                ["generateStrings"] = new JsonRpcResponseContract(typeof(RpcRandomResult<string>)),
                ["generateUUIDs"] = new JsonRpcResponseContract(typeof(RpcRandomResult<Guid>)),
                ["generateBlobs"] = new JsonRpcResponseContract(typeof(RpcRandomResult<byte[]>)),
                ["generateSignedIntegers"] = new JsonRpcResponseContract(typeof(RpcSignedRandomResult<RpcIntegersRandom, int>)),
                ["generateSignedIntegerSequences"] = new JsonRpcResponseContract(typeof(RpcSignedRandomResult<RpcIntegerSequencesRandom, IReadOnlyList<int>>)),
                ["generateSignedDecimalFractions"] = new JsonRpcResponseContract(typeof(RpcSignedRandomResult<RpcDecimalFractionsRandom, decimal>)),
                ["generateSignedGaussians"] = new JsonRpcResponseContract(typeof(RpcSignedRandomResult<RpcGaussiansRandom, decimal>)),
                ["generateSignedStrings"] = new JsonRpcResponseContract(typeof(RpcSignedRandomResult<RpcStringsRandom, string>)),
                ["generateSignedUUIDs"] = new JsonRpcResponseContract(typeof(RpcSignedRandomResult<RpcUuidsRandom, Guid>)),
                ["generateSignedBlobs"] = new JsonRpcResponseContract(typeof(RpcSignedRandomResult<RpcBlobsRandom, byte[]>)),
                ["verifySignature"] = new JsonRpcResponseContract(typeof(RpcVerifyResult))
            };
        }

        private static void TransferRandom<TData>(RpcRandom<TData> source, Random<TData> target)
        {
            target.Data = source.Data;
            target.CompletionTime = source.CompletionTime;
        }

        private static void TransferRandom<TData, TParameters>(SignedRandom<TData, TParameters> source, RpcSignedRandom<TData> target)
            where TParameters : RandomParameters, new()
        {
            target.Data = source.Data;
            target.ApiKeyHash = source.ApiKeyHash;
            target.CompletionTime = source.CompletionTime;
            target.SerialNumber = source.SerialNumber;
            target.UserData = source.UserData;
            target.License.Type = source.License.Type;
            target.License.Text = source.License.Text;

            if (source.License.InfoUrl != null)
            {
                target.License.InfoUrl = source.License.InfoUrl.OriginalString;
            }
        }

        private static void TransferRandom<TData, TParameters>(RpcSignedRandom<TData> source, SignedRandom<TData, TParameters> target)
            where TParameters : RandomParameters, new()
        {
            target.Data = source.Data;
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

        private Dictionary<string, object> CreateGenerationParameters(int capacity)
        {
            return new Dictionary<string, object>(capacity + 1, StringComparer.Ordinal)
            {
                ["apiKey"] = _apiKey
            };
        }

        private async Task<TResult> InvokeServiceMethodAsync<TResult, TRandom, TValue>(string method, IReadOnlyDictionary<string, object> parameters, CancellationToken cancellationToken)
            where TResult : RpcRandomResultObject<TRandom, TValue>
            where TRandom : RpcRandomObject<TValue>
        {
            await _invocationSemaphore.WaitAsync(cancellationToken).ConfigureAwait(false);

            try
            {
                var advisoryDelay = 0L;
                var advisoryTimeLockTaken = false;

                _advisoryTimeLock.Enter(ref advisoryTimeLockTaken);

                advisoryDelay = (_advisoryTime - DateTime.UtcNow).Ticks;

                if (advisoryTimeLockTaken)
                {
                    _advisoryTimeLock.Exit();
                }
                if (advisoryDelay > 0L)
                {
                    await Task.Delay(TimeSpan.FromTicks(advisoryDelay), cancellationToken).ConfigureAwait(false);
                }

                var jsonRpcRequest = new JsonRpcRequest(new JsonRpcId(Guid.NewGuid().ToString()), method, parameters);
                var jsonRpcResponse = await SendJsonRpcRequestAsync(jsonRpcRequest, cancellationToken).ConfigureAwait(false);
                var jsonRpcResponseResult = (TResult)jsonRpcResponse.Result;

                advisoryTimeLockTaken = false;

                _advisoryTimeLock.Enter(ref advisoryTimeLockTaken);
                _advisoryTime = jsonRpcResponseResult.Random.CompletionTime.AddMilliseconds(jsonRpcResponseResult.AdvisoryDelay);

                if (advisoryTimeLockTaken)
                {
                    _advisoryTimeLock.Exit();
                }

                return jsonRpcResponseResult;
            }
            finally
            {
                _invocationSemaphore.Release();
            }
        }

        private async Task<TResult> InvokeServiceMethodAsync<TResult>(string method, IReadOnlyDictionary<string, object> parameters, CancellationToken cancellationToken)
            where TResult : RpcMethodResult
        {
            await _invocationSemaphore.WaitAsync(cancellationToken).ConfigureAwait(false);

            try
            {
                var jsonRpcRequest = new JsonRpcRequest(new JsonRpcId(Guid.NewGuid().ToString()), method, parameters);
                var jsonRpcResponse = await SendJsonRpcRequestAsync(jsonRpcRequest, cancellationToken).ConfigureAwait(false);

                return (TResult)jsonRpcResponse.Result;
            }
            finally
            {
                _invocationSemaphore.Release();
            }
        }

        private async Task<JsonRpcResponse> SendJsonRpcRequestAsync(JsonRpcRequest request, CancellationToken cancellationToken)
        {
            var requestId = request.Id;

            using (var requestStream = new MemoryStream())
            {
                _jsonRpcSerializer.SerializeRequest(request, requestStream);

                cancellationToken.ThrowIfCancellationRequested();
                requestStream.Position = 0;

                using (var httpRequest = new HttpRequestMessage(HttpMethod.Post, _serviceUri))
                {
                    var requestContent = new StreamContent(requestStream);

                    requestContent.Headers.ContentType = _mediaTypeHeaderValue;
                    httpRequest.Content = requestContent;

                    using (var httpResponse = await _httpInvoker.SendAsync(httpRequest, cancellationToken).ConfigureAwait(false))
                    {
                        if (httpResponse.StatusCode != HttpStatusCode.OK)
                        {
                            throw new RandomOrgProtocolException(httpResponse.StatusCode, Strings.GetString("protocol.http.status_code.invalid_value"));
                        }

                        var contentTypeHeaderValue = httpResponse.Content.Headers.ContentType;

                        if (contentTypeHeaderValue == null)
                        {
                            throw new RandomOrgProtocolException(httpResponse.StatusCode, Strings.GetString("protocol.http.headers.content_type.invalid_value"));
                        }
                        if (!contentTypeHeaderValue.MediaType.Equals(_mediaTypeHeaderValue.MediaType, StringComparison.OrdinalIgnoreCase))
                        {
                            throw new RandomOrgProtocolException(httpResponse.StatusCode, Strings.GetString("protocol.http.headers.content_type.invalid_value"));
                        }
                        if ((contentTypeHeaderValue.CharSet != null) && (string.Compare(contentTypeHeaderValue.CharSet, Encoding.UTF8.WebName, StringComparison.OrdinalIgnoreCase) != 0))
                        {
                            throw new RandomOrgProtocolException(httpResponse.StatusCode, Strings.GetString("protocol.http.headers.content_type.invalid_value"));
                        }

                        var responseData = default(JsonRpcData<JsonRpcResponse>);

                        using (var responseStream = await httpResponse.Content.ReadAsStreamAsync().ConfigureAwait(false))
                        {
                            cancellationToken.ThrowIfCancellationRequested();

                            _jsonRpcContractResolver.AddResponseBinding(requestId, request.Method);

                            try
                            {
                                responseData = await _jsonRpcSerializer.DeserializeResponseDataAsync(responseStream, cancellationToken).ConfigureAwait(false);
                            }
                            catch (JsonException e)
                            {
                                throw new RandomOrgClientException(Strings.GetString("protocol.rpc.message.invalid_value"), e);
                            }
                            catch (JsonRpcException e)
                            {
                                throw new RandomOrgClientException(Strings.GetString("protocol.rpc.message.invalid_value"), e);
                            }
                            finally
                            {
                                _jsonRpcContractResolver.RemoveResponseBinding(requestId);
                            }
                        }

                        if (responseData.IsBatch)
                        {
                            throw new RandomOrgProtocolException(httpResponse.StatusCode, Strings.GetString("protocol.random.message.invalid_value"));
                        }

                        var responseItem = responseData.Item;

                        if (!responseItem.IsValid)
                        {
                            throw new RandomOrgClientException(Strings.GetString("protocol.random.message.invalid_value"), responseItem.Exception);
                        }

                        var response = responseItem.Message;

                        if (!response.Success)
                        {
                            throw new RandomOrgException(request.Method, response.Error.Code, response.Error.Message);
                        }
                        if (response.Result == null)
                        {
                            throw new RandomOrgClientException(Strings.GetString("protocol.random.message.invalid_value"));
                        }

                        return response;
                    }
                }
            }
        }

        /// <summary>Releases all resources used by the current instance of the <see cref="RandomOrgClient" />.</summary>
        public void Dispose()
        {
            _advisoryTime = new DateTime(0L, DateTimeKind.Utc);
            _invocationSemaphore.Dispose();
            _httpInvoker.Dispose();
        }

        /// <summary>Returns usage information of the current API key as an asynchronous operation.</summary>
        /// <param name="cancellationToken">The cancellation token for canceling the operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result is API key usage information.</returns>
        /// <exception cref="OperationCanceledException">The operation was canceled.</exception>
        /// <exception cref="RandomOrgClientException">An error occurred during processing RANDOM.ORG service method result.</exception>
        /// <exception cref="RandomOrgException">An error occurred during invocation of the RANDOM.ORG service method.</exception>
        /// <exception cref="RandomOrgProtocolException">An error occurred during communication with the RANDOM.ORG service.</exception>
        public async Task<RandomUsage> GetUsageAsync(CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var parameters = new Dictionary<string, object>(1, StringComparer.Ordinal)
            {
                ["apiKey"] = _apiKey
            };

            var result = await InvokeServiceMethodAsync<RpcRandomUsageResult>("getUsage", parameters, cancellationToken).ConfigureAwait(false);

            return new RandomUsage(result.Status, result.BitsLeft, result.RequestsLeft);
        }

        /// <summary>Verifies the signature of signed random objects and associated data as an asynchronous operation.</summary>
        /// <typeparam name="TValue">The type of random object.</typeparam>
        /// <typeparam name="TParameters">The type of random parameters.</typeparam>
        /// <param name="random">The signed random objects and associated data.</param>
        /// <param name="signature">The signature from the same response that the random data originates from.</param>
        /// <param name="cancellationToken">The cancellation token for canceling the operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result is a value, indicating if the random objects are authentic.</returns>
        /// <exception cref="ArgumentException">Random data, or license type, or a random parameter is <see langword="null" />.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="random" /> or <paramref name="signature" /> is <see langword="null" />.</exception>
        /// <exception cref="OperationCanceledException">The operation was canceled.</exception>
        /// <exception cref="RandomOrgClientException">An error occurred during processing RANDOM.ORG service method result.</exception>
        /// <exception cref="RandomOrgException">An error occurred during invocation of the RANDOM.ORG service method.</exception>
        /// <exception cref="RandomOrgProtocolException">An error occurred during communication with the RANDOM.ORG service.</exception>
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
            if (random.License.Type == null)
            {
                throw new ArgumentException(Strings.GetString("client.verify.license.type.not_specified"), nameof(random));
            }

            cancellationToken.ThrowIfCancellationRequested();

            var rpcRandomParam = default(object);

            switch (random)
            {
                case SignedRandom<int, IntegerParameters> xRandom:
                    {
                        var rpcRandom = new RpcIntegersRandom
                        {
                            Method = "generateSignedIntegers",
                            Count = xRandom.Data.Count,
                            Minimum = xRandom.Parameters.Minimum,
                            Maximum = xRandom.Parameters.Maximum,
                            Replacement = xRandom.Parameters.Replacement,
                            Base = 10
                        };

                        TransferRandom(xRandom, rpcRandom);

                        rpcRandomParam = rpcRandom;
                    }
                    break;
                case SignedRandom<IReadOnlyList<int>, IntegerSequenceParameters> xRandom:
                    {
                        if (xRandom.Parameters.Minimums == null)
                        {
                            throw new ArgumentException(Strings.GetString("random.sequence.minimums.not_specified"), nameof(random));
                        }
                        if (xRandom.Parameters.Maximums == null)
                        {
                            throw new ArgumentException(Strings.GetString("random.sequence.maximums.not_specified"), nameof(random));
                        }
                        if (xRandom.Parameters.Replacements == null)
                        {
                            throw new ArgumentException(Strings.GetString("random.sequence.replacements.not_specified"), nameof(random));
                        }

                        var count = xRandom.Data.Count;

                        if ((count != xRandom.Parameters.Minimums.Count) ||
                            (count != xRandom.Parameters.Maximums.Count) ||
                            (count != xRandom.Parameters.Replacements.Count))
                        {
                            throw new ArgumentException(Strings.GetString("random.sequence.arguments.different_size"), nameof(random));
                        }

                        var lengths = new int[count];
                        var bases = new int[count];

                        for (var i = 0; i < count; i++)
                        {
                            if (xRandom.Data[i] == null)
                            {
                                throw new ArgumentException(Strings.GetString("random.sequence.sequence.not_specified"), nameof(random));
                            }

                            lengths[i] = xRandom.Data[i].Count;
                            bases[i] = 10;
                        }

                        var rpcRandom = new RpcIntegerSequencesRandom
                        {
                            Method = "generateSignedIntegerSequences",
                            Count = count,
                            Lengths = lengths,
                            Minimums = xRandom.Parameters.Minimums,
                            Maximums = xRandom.Parameters.Maximums,
                            Replacements = xRandom.Parameters.Replacements,
                            Bases = bases
                        };

                        TransferRandom(xRandom, rpcRandom);

                        rpcRandomParam = rpcRandom;
                    }
                    break;
                case SignedRandom<decimal, DecimalFractionParameters> xRandom:
                    {
                        var rpcRandom = new RpcDecimalFractionsRandom
                        {
                            Method = "generateSignedDecimalFractions",
                            Count = xRandom.Data.Count,
                            DecimalPlaces = xRandom.Parameters.DecimalPlaces,
                            Replacement = xRandom.Parameters.Replacement
                        };

                        TransferRandom(xRandom, rpcRandom);

                        rpcRandomParam = rpcRandom;
                    }
                    break;
                case SignedRandom<decimal, GaussianParameters> xRandom:
                    {
                        var rpcRandom = new RpcGaussiansRandom
                        {
                            Method = "generateSignedGaussians",
                            Count = xRandom.Data.Count,
                            Mean = xRandom.Parameters.Mean,
                            StandardDeviation = xRandom.Parameters.StandardDeviation,
                            SignificantDigits = xRandom.Parameters.SignificantDigits
                        };

                        TransferRandom(xRandom, rpcRandom);

                        rpcRandomParam = rpcRandom;
                    }
                    break;
                case SignedRandom<string, StringParameters> xRandom:
                    {
                        if (xRandom.Parameters.Characters == null)
                        {
                            throw new ArgumentException(Strings.GetString("random.string.characters.not_specified"), nameof(random));
                        }

                        var rpcRandom = new RpcStringsRandom
                        {
                            Method = "generateSignedStrings",
                            Count = xRandom.Data.Count,
                            Length = xRandom.Parameters.Length,
                            Characters = xRandom.Parameters.Characters,
                            Replacement = xRandom.Parameters.Replacement
                        };

                        TransferRandom(xRandom, rpcRandom);

                        rpcRandomParam = rpcRandom;
                    }
                    break;
                case SignedRandom<Guid, UuidParameters> xRandom:
                    {
                        var rpcRandom = new RpcUuidsRandom
                        {
                            Method = "generateSignedUUIDs",
                            Count = xRandom.Data.Count
                        };

                        TransferRandom(xRandom, rpcRandom);

                        rpcRandomParam = rpcRandom;
                    }
                    break;
                case SignedRandom<byte[], BlobParameters> xRandom:
                    {
                        var rpcRandom = new RpcBlobsRandom
                        {
                            Method = "generateSignedBlobs",
                            Count = xRandom.Data.Count,
                            Size = xRandom.Parameters.Size * 8,
                            Format = "base64"
                        };

                        TransferRandom(xRandom, rpcRandom);

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
                ["signature"] = signature
            };

            var result = await InvokeServiceMethodAsync<RpcVerifyResult>(
                "verifySignature", parameters, cancellationToken).ConfigureAwait(false);

            return result.Authenticity;
        }

        /// <summary>Gets the nearest allowed time in UTC for random values generation.</summary>
        public DateTime GenerateAdvisoryTime
        {
            get
            {
                var advisoryTime = default(DateTime);
                var advisoryTimeLockTaken = false;

                _advisoryTimeLock.Enter(ref advisoryTimeLockTaken);

                advisoryTime = _advisoryTime;

                if (advisoryTimeLockTaken)
                {
                    _advisoryTimeLock.Exit();
                }

                return advisoryTime;
            }
        }
    }
}
