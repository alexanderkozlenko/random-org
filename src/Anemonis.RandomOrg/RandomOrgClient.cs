// © Alexander Kozlenko. Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text.RegularExpressions;
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
        private static readonly string s_contentTypeHeaderValue = $"{JsonRpcTransport.MediaType}; charset={JsonRpcTransport.Charset}";
        private static readonly string s_userAgentHeaderValue = CreateUserAgentHeaderValue();

        private static readonly Uri s_serviceUri = new("https://api.random.org/json-rpc/3/invoke", UriKind.Absolute);
        private static readonly JsonSerializer s_jsonSerializer = CreateJsonSerializer();
        private static readonly Dictionary<string, JsonRpcResponseContract> s_responseContracts = CreateJsonRpcContracts();

        private readonly string _apiKey;
        private readonly JsonRpcContractResolver _jsonRpcContractResolver = CreateJsonRpcContractResolver();
        private readonly JsonRpcSerializer _jsonRpcSerializer;
        private readonly HttpMessageInvoker _httpInvoker;

        /// <summary>Initializes a new instance of the <see cref="RandomOrgClient" /> class.</summary>
        /// <param name="apiKey">The API key, which is used to track the true random bit usage for the client.</param>
        /// <param name="httpInvoker">The component for sending HTTP requests.</param>
        /// <exception cref="ArgumentException"><paramref name="apiKey" /> is not of UUID format (32 digits separated by hyphens).</exception>
        /// <exception cref="ArgumentNullException"><paramref name="apiKey" /> or <paramref name="httpInvoker" /> is <see langword="null" />.</exception>
        public RandomOrgClient(string apiKey, HttpMessageInvoker httpInvoker)
        {
            if (apiKey is null)
            {
                throw new ArgumentNullException(nameof(apiKey));
            }
            if (!Guid.TryParseExact(apiKey, "D", out _))
            {
                throw new ArgumentException(Strings.GetString("client.api_key.invalid_format"), nameof(apiKey));
            }
            if (httpInvoker is null)
            {
                throw new ArgumentNullException(nameof(httpInvoker));
            }

            _apiKey = apiKey;
            _httpInvoker = httpInvoker;
            _jsonRpcSerializer = new(_jsonRpcContractResolver, s_jsonSerializer);
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
            settings.Converters.Add(new TimeSpanConverter());

            return JsonSerializer.Create(settings);
        }

        private static string CreateUserAgentHeaderValue()
        {
            var packageAssembly = Assembly.GetExecutingAssembly();
            var packageName = packageAssembly.GetName().Name;
            var productVersionAttribute = packageAssembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>();
            var productVersion = Regex.Match(productVersionAttribute.InformationalVersion, @"^\d+\.\d+", RegexOptions.Singleline).Value;

            return $"{nameof(Anemonis)}/{productVersion} (nuget:{packageName})";
        }

        private static HttpMessageInvoker CreateHttpInvoker()
        {
            var httpHandler = new HttpClientHandler
            {
                AllowAutoRedirect = false,
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
            };

            return new HttpClient(httpHandler);
        }

        private static JsonRpcContractResolver CreateJsonRpcContractResolver()
        {
            var resolver = new JsonRpcContractResolver();
            var enumerator = s_responseContracts.GetEnumerator();

            while (enumerator.MoveNext())
            {
                resolver.AddResponseContract(enumerator.Current.Key, enumerator.Current.Value);
            }

            return resolver;
        }

        private static Dictionary<string, JsonRpcResponseContract> CreateJsonRpcContracts()
        {
            return new(16, StringComparer.Ordinal)
            {
                ["getUsage"] = new(typeof(RpcRandomUsageResult)),
                ["generateIntegers"] = new(typeof(RpcRandomResult<int>)),
                ["generateIntegerSequences"] = new(typeof(RpcRandomResult<IReadOnlyList<int>>)),
                ["generateDecimalFractions"] = new(typeof(RpcRandomResult<decimal>)),
                ["generateGaussians"] = new(typeof(RpcRandomResult<decimal>)),
                ["generateStrings"] = new(typeof(RpcRandomResult<string>)),
                ["generateUUIDs"] = new(typeof(RpcRandomResult<Guid>)),
                ["generateBlobs"] = new(typeof(RpcRandomResult<byte[]>)),
                ["generateSignedIntegers"] = new(typeof(RpcSignedRandomResult<RpcIntegersRandom, int>)),
                ["generateSignedIntegerSequences"] = new(typeof(RpcSignedRandomResult<RpcIntegerSequencesRandom, IReadOnlyList<int>>)),
                ["generateSignedDecimalFractions"] = new(typeof(RpcSignedRandomResult<RpcDecimalFractionsRandom, decimal>)),
                ["generateSignedGaussians"] = new(typeof(RpcSignedRandomResult<RpcGaussiansRandom, decimal>)),
                ["generateSignedStrings"] = new(typeof(RpcSignedRandomResult<RpcStringsRandom, string>)),
                ["generateSignedUUIDs"] = new(typeof(RpcSignedRandomResult<RpcUuidsRandom, Guid>)),
                ["generateSignedBlobs"] = new(typeof(RpcSignedRandomResult<RpcBlobsRandom, byte[]>)),
                ["verifySignature"] = new(typeof(RpcVerifyResult))
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

            if (source.License.InfoUrl is not null)
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

            if (source.License.InfoUrl is not null)
            {
                target.License.InfoUrl = new(source.License.InfoUrl);
            }
        }

        private Dictionary<string, object> CreateGenerationParameters(int capacity)
        {
            return new(capacity + 1, StringComparer.Ordinal)
            {
                ["apiKey"] = _apiKey
            };
        }

        private async Task<TResult> InvokeServiceMethodAsync<TResult, TRandom, TValue>(string method, IReadOnlyDictionary<string, object> parameters, CancellationToken cancellationToken)
            where TResult : RpcRandomResultObject<TRandom, TValue>
            where TRandom : RpcRandomObject<TValue>
        {
            var jsonRpcRequest = new JsonRpcRequest(new JsonRpcId(Guid.NewGuid().ToString()), method, parameters);
            var jsonRpcResponse = await SendJsonRpcRequestAsync(jsonRpcRequest, cancellationToken).ConfigureAwait(false);
            var jsonRpcResponseResult = (TResult)jsonRpcResponse.Result;

            return jsonRpcResponseResult;
        }

        private async Task<TResult> InvokeServiceMethodAsync<TResult>(string method, IReadOnlyDictionary<string, object> parameters, CancellationToken cancellationToken)
            where TResult : RpcMethodResult
        {
            var jsonRpcRequest = new JsonRpcRequest(new JsonRpcId(Guid.NewGuid().ToString()), method, parameters);
            var jsonRpcResponse = await SendJsonRpcRequestAsync(jsonRpcRequest, cancellationToken).ConfigureAwait(false);

            return (TResult)jsonRpcResponse.Result;
        }

        private async Task<JsonRpcResponse> SendJsonRpcRequestAsync(JsonRpcRequest request, CancellationToken cancellationToken)
        {
            var requestId = request.Id;

            using (var requestStream = new MemoryStream())
            {
                _jsonRpcSerializer.SerializeRequest(request, requestStream);

                requestStream.Position = 0;

                using (var httpRequest = new HttpRequestMessage(HttpMethod.Post, s_serviceUri))
                {
                    var requestContent = new StreamContent(requestStream);

                    requestContent.Headers.Add("Content-Type", s_contentTypeHeaderValue);

                    httpRequest.Content = requestContent;
                    httpRequest.Headers.Date = DateTime.UtcNow;
                    httpRequest.Headers.ExpectContinue = false;
                    httpRequest.Headers.Add("Accept", JsonRpcTransport.MediaType);
                    httpRequest.Headers.Add("Accept-Charset", JsonRpcTransport.Charset);
                    httpRequest.Headers.Add("User-Agent", s_userAgentHeaderValue);

                    using (var httpResponse = await _httpInvoker.SendAsync(httpRequest, cancellationToken).ConfigureAwait(false))
                    {
                        if (httpResponse.StatusCode != HttpStatusCode.OK)
                        {
                            throw new RandomOrgProtocolException(httpResponse.StatusCode, Strings.GetString("protocol.http.status_code.invalid_value"));
                        }

                        var contentTypeHeaderValue = httpResponse.Content.Headers.ContentType;

                        if (contentTypeHeaderValue is null)
                        {
                            throw new RandomOrgProtocolException(httpResponse.StatusCode, Strings.GetString("protocol.http.headers.content_type.invalid_value"));
                        }
                        if (!contentTypeHeaderValue.MediaType.Equals(JsonRpcTransport.MediaType, StringComparison.OrdinalIgnoreCase))
                        {
                            throw new RandomOrgProtocolException(httpResponse.StatusCode, Strings.GetString("protocol.http.headers.content_type.invalid_value"));
                        }
                        if ((contentTypeHeaderValue.CharSet is not null) && (string.Compare(contentTypeHeaderValue.CharSet, JsonRpcTransport.Charset, StringComparison.OrdinalIgnoreCase) != 0))
                        {
                            throw new RandomOrgProtocolException(httpResponse.StatusCode, Strings.GetString("protocol.http.headers.content_type.invalid_value"));
                        }

                        var responseData = default(JsonRpcData<JsonRpcResponse>);

                        using (var responseStream = await httpResponse.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false))
                        {
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
                        if (response.Result is null)
                        {
                            throw new RandomOrgClientException(Strings.GetString("protocol.random.message.invalid_value"));
                        }

                        return response;
                    }
                }
            }
        }

        /// <inheritdoc />
        public void Dispose()
        {
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
            var parameters = new Dictionary<string, object>(1, StringComparer.Ordinal)
            {
                ["apiKey"] = _apiKey
            };

            var result = await InvokeServiceMethodAsync<RpcRandomUsageResult>("getUsage", parameters, cancellationToken).ConfigureAwait(false);

            return new(result.Status, result.BitsLeft, result.RequestsLeft);
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
            if (random is null)
            {
                throw new ArgumentNullException(nameof(random));
            }
            if (signature is null)
            {
                throw new ArgumentNullException(nameof(signature));
            }
            if (random.Data is null)
            {
                throw new ArgumentException(Strings.GetString("client.verify.data.not_specified"), nameof(random));
            }
            if (random.License.Type is null)
            {
                throw new ArgumentException(Strings.GetString("client.verify.license.type.not_specified"), nameof(random));
            }

            var rpcRandomParam = default(RpcRandomObject);

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
                        if (xRandom.Parameters.Minimums is null)
                        {
                            throw new ArgumentException(Strings.GetString("random.sequence.minimums.not_specified"), nameof(random));
                        }
                        if (xRandom.Parameters.Maximums is null)
                        {
                            throw new ArgumentException(Strings.GetString("random.sequence.maximums.not_specified"), nameof(random));
                        }
                        if (xRandom.Parameters.Replacements is null)
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
                            if (xRandom.Data[i] is null)
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
                        if (xRandom.Parameters.Characters is null)
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
    }
}
