﻿using System;
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
    /// <summary>Represents RANDOM.ORG service client.</summary>
    public sealed partial class RandomOrgClient : IDisposable
    {
        private static readonly MediaTypeHeaderValue _mediaTypeValue = new MediaTypeHeaderValue("application/json");
        private static readonly MediaTypeWithQualityHeaderValue _mediaTypeWithQualityValue = new MediaTypeWithQualityHeaderValue("application/json");
        private static readonly Uri _serviceUri = new Uri("https://api.random.org/json-rpc/2/invoke", UriKind.Absolute);
        private static readonly IDictionary<string, JsonRpcResponseContract> _contracts = CreateContracts();

        private readonly string _apiKey;
        private readonly HttpMessageInvoker _httpInvoker;
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

        private readonly JsonRpcSerializer _serializer = new JsonRpcSerializer(
            EmptyDictionary<string, JsonRpcRequestContract>.Instance,
            _contracts,
            new Dictionary<JsonRpcId, string>(1),
            EmptyDictionary<JsonRpcId, JsonRpcResponseContract>.Instance);

        private DateTime? _advisoryTime;

        /// <summary>Initializes a new instance of the <see cref="RandomOrgClient" /> class.</summary>
        /// <param name="apiKey">The API key, which is used to track the true random bit usage for the client.</param>
        /// <exception cref="ArgumentException"><paramref name="apiKey" /> is not of UUID format (32 digits separated by hyphens).</exception>
        /// <exception cref="ArgumentNullException"><paramref name="apiKey" /> is <see langword="null" />.</exception>
        public RandomOrgClient(string apiKey)
        {
            if (apiKey == null)
            {
                throw new ArgumentNullException(nameof(apiKey));
            }
            if (!VerifyApiKey(apiKey))
            {
                throw new ArgumentException(Strings.GetString("client.api_key.invalid_format"), nameof(apiKey));
            }

            _apiKey = apiKey;
            _httpInvoker = CreateHttpInvoker();
        }

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
            if (!VerifyApiKey(apiKey))
            {
                throw new ArgumentException(Strings.GetString("client.api_key.invalid_format"), nameof(apiKey));
            }
            if (httpInvoker == null)
            {
                throw new ArgumentNullException(nameof(httpInvoker));
            }

            _apiKey = apiKey;
            _httpInvoker = httpInvoker;
        }

        /// <summary>Releases all resources used by the current instance of the <see cref="RandomOrgClient" />.</summary>
        public void Dispose()
        {
            _httpInvoker.Dispose();
            _semaphore.Dispose();
            _serializer.Dispose();
            _advisoryTime = null;
        }

        /// <summary>Returns usage information of the current API key as an asynchronous operation.</summary>
        /// <param name="cancellationToken">The cancellation token for canceling the operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result is API key usage information.</returns>
        /// <exception cref="OperationCanceledException">The operation was canceled.</exception>
        /// <exception cref="RandomOrgContractException">An error occurred during service result handling.</exception>
        /// <exception cref="RandomOrgException">An error occurred during service method invocation.</exception>
        /// <exception cref="RandomOrgRequestException">An error occurred during HTTP request execution.</exception>
        public async Task<RandomUsage> GetUsageAsync(CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

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
        /// <returns>A task that represents the asynchronous operation. The task result is a value, indicating if the random objects are authentic.</returns>
        /// <exception cref="ArgumentException">Random data, or license type, or a random parameter is <see langword="null" />.</exception>
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
                ["signature"] = Convert.ToBase64String(signature)
            };

            var response = await InvokeAccountServiceMethodAsync<RpcVerifyResult>(
                "verifySignature", parameters, cancellationToken).ConfigureAwait(false);

            return response.Authenticity;
        }

        private Dictionary<string, object> CreateGenerationParameters(int capacity)
        {
            return new Dictionary<string, object>(capacity + 1, StringComparer.Ordinal)
            {
                ["apiKey"] = _apiKey
            };
        }

        private static void TransferRandom<TData>(Random<TData> source, RpcRandom<TData> target)
        {
            target.Data = source.Data;
            target.CompletionTime = source.CompletionTime;
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

        private async Task<TResult> InvokeGenerationServiceMethodAsync<TResult, TRandom, TValue>(string method, IReadOnlyDictionary<string, object> parameters, CancellationToken cancellationToken)
            where TResult : RpcRandomResultObject<TRandom, TValue>
            where TRandom : RpcRandomObject<TValue>
        {
            await _semaphore.WaitAsync(cancellationToken).ConfigureAwait(false);

            try
            {
                if (_advisoryTime != null)
                {
                    var advisoryDelay = (_advisoryTime.Value - DateTime.UtcNow).Ticks;

                    if (advisoryDelay > 0)
                    {
                        await Task.Delay(TimeSpan.FromTicks(Math.Min(advisoryDelay, TimeSpan.TicksPerDay)), cancellationToken).ConfigureAwait(false);
                    }
                }

                var jsonRpcResponse = await InvokeServiceMethodAsync(CreateRequest(method, parameters), cancellationToken).ConfigureAwait(false);

                var result = (TResult)jsonRpcResponse.Result;

                _advisoryTime = result.Random.CompletionTime + TimeSpan.FromMilliseconds(result.AdvisoryDelay);

                return result;
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private async Task<TResult> InvokeAccountServiceMethodAsync<TResult>(string method, IReadOnlyDictionary<string, object> parameters, CancellationToken cancellationToken)
            where TResult : RpcMethodResult
        {
            await _semaphore.WaitAsync(cancellationToken).ConfigureAwait(false);

            try
            {
                var jsonRpcResponse = await InvokeServiceMethodAsync(CreateRequest(method, parameters), cancellationToken).ConfigureAwait(false);

                return (TResult)jsonRpcResponse.Result;
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private async Task<JsonRpcResponse> InvokeServiceMethodAsync(JsonRpcRequest request, CancellationToken cancellationToken)
        {
            var requestString = _serializer.SerializeRequest(request);

            cancellationToken.ThrowIfCancellationRequested();

            using (var requestMessage = new HttpRequestMessage(HttpMethod.Post, _serviceUri))
            {
                var requestContent = new StringContent(requestString);

                requestContent.Headers.ContentType = _mediaTypeValue;
                requestMessage.Content = requestContent;

                using (var responseMessage = await _httpInvoker.SendAsync(requestMessage, cancellationToken).ConfigureAwait(false))
                {
                    if (responseMessage.StatusCode != HttpStatusCode.OK)
                    {
                        throw new RandomOrgRequestException(responseMessage.StatusCode, Strings.GetString("protocol.http.status_code.invalid_value"));
                    }

                    var contentType = responseMessage.Content.Headers.ContentType;

                    if ((contentType == null) || (string.Compare(contentType.MediaType, _mediaTypeValue.MediaType, StringComparison.OrdinalIgnoreCase) != 0))
                    {
                        throw new RandomOrgRequestException(responseMessage.StatusCode, Strings.GetString("protocol.http.headers.invalid_values"));
                    }

                    var contentLength = responseMessage.Content.Headers.ContentLength;

                    if (contentLength == null)
                    {
                        throw new RandomOrgRequestException(responseMessage.StatusCode, Strings.GetString("protocol.http.headers.invalid_values"));
                    }

                    var responseString = await responseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);

                    cancellationToken.ThrowIfCancellationRequested();

                    if (responseString?.Length != contentLength)
                    {
                        throw new RandomOrgRequestException(responseMessage.StatusCode, Strings.GetString("protocol.http.headers.invalid_values"));
                    }

                    _serializer.StaticResponseBindings[request.Id] = request.Method;

                    var responseData = default(JsonRpcData<JsonRpcResponse>);

                    try
                    {
                        responseData = _serializer.DeserializeResponseData(responseString);
                    }
                    catch (JsonRpcException e)
                    {
                        throw new RandomOrgContractException(request.Id.ToString(), Strings.GetString("protocol.rpc.message.invalid_value"), e);
                    }
                    finally
                    {
                        _serializer.StaticResponseBindings.Remove(request.Id);
                    }

                    cancellationToken.ThrowIfCancellationRequested();

                    if (responseData.IsBatch)
                    {
                        throw new RandomOrgContractException(request.Id.ToString(), Strings.GetString("protocol.random.message.invalid_value"));
                    }

                    var responseItem = responseData.Item;

                    if (!responseItem.IsValid)
                    {
                        throw new RandomOrgContractException(request.Id.ToString(), Strings.GetString("protocol.random.message.invalid_value"), responseItem.Exception);
                    }

                    var response = responseItem.Message;

                    if (!response.Success)
                    {
                        throw new RandomOrgException(request.Method, response.Error.Code, response.Error.Message);
                    }
                    if (response.Result == null)
                    {
                        throw new RandomOrgContractException(request.Id.ToString(), Strings.GetString("protocol.random.message.invalid_value"));
                    }

                    return response;
                }
            }
        }

        private static JsonRpcRequest CreateRequest(string method, IReadOnlyDictionary<string, object> parameters)
        {
            return new JsonRpcRequest(method, new JsonRpcId(Guid.NewGuid().ToString("D")), parameters);
        }

        private static HttpMessageInvoker CreateHttpInvoker()
        {
            var httpHandler = new HttpClientHandler
            {
                AllowAutoRedirect = false,
                AutomaticDecompression = DecompressionMethods.GZip
            };

            var httpClient = new HttpClient(httpHandler);

            httpClient.DefaultRequestHeaders.Accept.Add(_mediaTypeWithQualityValue);
            httpClient.DefaultRequestHeaders.ExpectContinue = false;
            httpClient.Timeout = TimeSpan.FromMinutes(2);

            return httpClient;
        }

        private static IDictionary<string, JsonRpcResponseContract> CreateContracts()
        {
            return new Dictionary<string, JsonRpcResponseContract>(16)
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

        private static bool VerifyApiKey(string apiKey)
        {
            return Guid.TryParseExact(apiKey, "D", out var _);
        }
    }
}