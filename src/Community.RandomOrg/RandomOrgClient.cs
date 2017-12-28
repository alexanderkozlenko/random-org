using System;
using System.Collections.Generic;
using System.Data.JsonRpc;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Community.RandomOrg.Data;
using Community.RandomOrg.Resources;

namespace Community.RandomOrg
{
    /// <summary>A RANDOM.ORG service client.</summary>
    public sealed partial class RandomOrgClient : IRandomOrgClient
    {
        private static readonly MediaTypeHeaderValue _mediaTypeHeaderValue = new MediaTypeHeaderValue("application/json");
        private static readonly JsonRpcSerializer _jsonRpcSerializer = CreateJsonRpcSerializer();
        private static readonly Uri _serviceUri = new Uri("https://api.random.org/json-rpc/2/invoke", UriKind.Absolute);
        private static readonly IReadOnlyDictionary<Type, JsonRpcMethodScheme> _signedResultMethodSchemeBindings = CreateSignedResultMethodSchemeBindings();

        private readonly string _apiKey;
        private readonly HttpMessageInvoker _httpMessageInvoker;
        private readonly SemaphoreSlim _requestSemaphore = new SemaphoreSlim(1, 1);
        private readonly Dictionary<JsonRpcId, string> _methodNameBindings = new Dictionary<JsonRpcId, string>(1);
        private readonly Dictionary<JsonRpcId, JsonRpcMethodScheme> _methodSchemeBindings = new Dictionary<JsonRpcId, JsonRpcMethodScheme>(1);

        private DateTime? _advisoryTime;

        /// <summary>Initializes a new instance of the <see cref="RandomOrgClient" /> class.</summary>
        /// <param name="apiKey">The API key, which is used to track the true random bit usage for the client.</param>
        /// <param name="httpMessageInvoker">The component for sending HTTP requests.</param>
        /// <exception cref="ArgumentException"><paramref name="apiKey" /> is not of UUID format.</exception>
        public RandomOrgClient(string apiKey = null, HttpMessageInvoker httpMessageInvoker = null)
        {
            if ((apiKey != null) && !Guid.TryParseExact(apiKey, "D", out var _))
            {
                throw new ArgumentException(Strings.GetString("client.api_key.invalid_format"), nameof(apiKey));
            }

            _apiKey = apiKey;
            _httpMessageInvoker = httpMessageInvoker ?? CreateHttpMessageInvoker();
        }

        /// <summary>Returns information related to the usage of a given API key as an asynchronous operation.</summary>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <returns>A <see cref="RandomUsage" /> instance.</returns>
        /// <exception cref="InvalidOperationException">The API key is not specified.</exception>
        /// <exception cref="OperationCanceledException">The operation was canceled.</exception>
        /// <exception cref="RandomOrgException">An error occurred during service method invocation.</exception>
        /// <exception cref="RandomOrgRequestException">An HTTP error occurred during service method invocation.</exception>
        public async Task<RandomUsage> GetUsageAsync(CancellationToken cancellationToken)
        {
            if (_apiKey == null)
            {
                throw new InvalidOperationException(Strings.GetString("client.api_key.required"));
            }

            var @params = new Dictionary<string, object>(1, StringComparer.Ordinal)
            {
                ["apiKey"] = _apiKey
            };

            var result = await InvokeRandomOrgMethod<RpcGetUsageResult>("getUsage", @params, cancellationToken).ConfigureAwait(false);

            return new RandomUsage(result.Status, result.CreationTime, result.TotalBits, result.BitsLeft, result.TotalRequests, result.RequestsLeft);
        }

        /// <summary>Retrieves previously generated signed results (which are stored for 24 hours) as an asynchronous operation.</summary>
        /// <typeparam name="TRandom">The type of random data container.</typeparam>
        /// <typeparam name="TValue">The type of random object.</typeparam>
        /// <param name="serialNumber">The integer containing the serial number associated with this random information.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <returns>A <see cref="SignedGenerationInfo{TRandom, TValue}" /> instance.</returns>
        /// <exception cref="JsonRpcException">The type of random data container is invalid.</exception>
        /// <exception cref="InvalidOperationException">The API key is not specified.</exception>
        /// <exception cref="OperationCanceledException">The operation was canceled.</exception>
        /// <exception cref="RandomOrgException">An error occurred during service method invocation.</exception>
        /// <exception cref="RandomOrgRequestException">An HTTP error occurred during service method invocation.</exception>
        public async Task<SignedGenerationInfo<TRandom, TValue>> GetResultAsync<TRandom, TValue>(
            long serialNumber, CancellationToken cancellationToken)
            where TRandom : SignedRandom<TValue>
        {
            if (_apiKey == null)
            {
                throw new InvalidOperationException(Strings.GetString("client.api_key.required"));
            }

            var @params = new Dictionary<string, object>(2, StringComparer.Ordinal)
            {
                ["apiKey"] = _apiKey,
                ["serialNumber"] = serialNumber
            };

            var resultType = default(Type);

            if (typeof(TRandom) == typeof(SignedIntegersRandom))
            {
                resultType = typeof(RpcSignedRandomResult<RpcSignedIntegersRandom, int>);
            }
            else if (typeof(TRandom) == typeof(SignedDecimalFractionsRandom))
            {
                resultType = typeof(RpcSignedRandomResult<RpcSignedDecimalFractionsRandom, decimal>);
            }
            else if (typeof(TRandom) == typeof(SignedGaussiansRandom))
            {
                resultType = typeof(RpcSignedRandomResult<RpcSignedGaussiansRandom, decimal>);
            }
            else if (typeof(TRandom) == typeof(SignedStringsRandom))
            {
                resultType = typeof(RpcSignedRandomResult<RpcSignedStringsRandom, string>);
            }
            else if (typeof(TRandom) == typeof(SignedUuidsRandom))
            {
                resultType = typeof(RpcSignedRandomResult<RpcSignedUuidsRandom, Guid>);
            }
            else if (typeof(TRandom) == typeof(SignedBlobsRandom))
            {
                resultType = typeof(RpcSignedRandomResult<RpcSignedBlobsRandom, string>);
            }

            var result = await InvokeRandomOrgMethod("getResult", @params, resultType, cancellationToken).ConfigureAwait(false);
            var generationInfo = default(SignedGenerationInfo<TRandom, TValue>);

            if (typeof(TRandom) == typeof(SignedIntegersRandom))
            {
                var typedResult = (RpcSignedRandomResult<RpcSignedIntegersRandom, int>)result;

                var typedRandom = new SignedIntegersRandom
                {
                    Minimum = (int)typedResult.Random.Minimum,
                    Maximum = (int)typedResult.Random.Maximum,
                    Replacement = typedResult.Random.Replacement
                };

                TransferValues(typedResult.Random, typedRandom);

                var typedGenerationInfo = new SignedGenerationInfo<SignedIntegersRandom, int>(
                    typedRandom, typedResult.BitsUsed, typedResult.BitsLeft, typedResult.RequestsLeft, typedResult.Signature);

                generationInfo = (SignedGenerationInfo<TRandom, TValue>)(object)typedGenerationInfo;
            }
            else if (typeof(TRandom) == typeof(SignedDecimalFractionsRandom))
            {
                var typedResult = (RpcSignedRandomResult<RpcSignedDecimalFractionsRandom, decimal>)result;

                var typedRandom = new SignedDecimalFractionsRandom
                {
                    DecimalPlaces = (int)typedResult.Random.DecimalPlaces,
                    Replacement = typedResult.Random.Replacement
                };

                TransferValues(typedResult.Random, typedRandom);

                var typedGenerationInfo = new SignedGenerationInfo<SignedDecimalFractionsRandom, decimal>(
                    typedRandom, typedResult.BitsUsed, typedResult.BitsLeft, typedResult.RequestsLeft, typedResult.Signature);

                generationInfo = (SignedGenerationInfo<TRandom, TValue>)(object)typedGenerationInfo;
            }
            else if (typeof(TRandom) == typeof(SignedGaussiansRandom))
            {
                var typedResult = (RpcSignedRandomResult<RpcSignedGaussiansRandom, decimal>)result;

                var typedRandom = new SignedGaussiansRandom
                {
                    Mean = typedResult.Random.Mean,
                    StandardDeviation = typedResult.Random.StandardDeviation,
                    SignificantDigits = (int)typedResult.Random.SignificantDigits
                };

                TransferValues(typedResult.Random, typedRandom);

                var typedGenerationInfo = new SignedGenerationInfo<SignedGaussiansRandom, decimal>(
                    typedRandom, typedResult.BitsUsed, typedResult.BitsLeft, typedResult.RequestsLeft, typedResult.Signature);

                generationInfo = (SignedGenerationInfo<TRandom, TValue>)(object)typedGenerationInfo;
            }
            else if (typeof(TRandom) == typeof(SignedStringsRandom))
            {
                var typedResult = (RpcSignedRandomResult<RpcSignedStringsRandom, string>)result;

                var typedRandom = new SignedStringsRandom
                {
                    Length = (int)typedResult.Random.Length,
                    Characters = typedResult.Random.Characters,
                    Replacement = typedResult.Random.Replacement
                };

                TransferValues(typedResult.Random, typedRandom);

                var typedGenerationInfo = new SignedGenerationInfo<SignedStringsRandom, string>(
                    typedRandom, typedResult.BitsUsed, typedResult.BitsLeft, typedResult.RequestsLeft, typedResult.Signature);

                generationInfo = (SignedGenerationInfo<TRandom, TValue>)(object)typedGenerationInfo;
            }
            else if (typeof(TRandom) == typeof(SignedUuidsRandom))
            {
                var typedResult = (RpcSignedRandomResult<RpcSignedUuidsRandom, Guid>)result;
                var typedRandom = new SignedUuidsRandom();

                TransferValues(typedResult.Random, typedRandom);

                var typedGenerationInfo = new SignedGenerationInfo<SignedUuidsRandom, Guid>(
                    typedRandom, typedResult.BitsUsed, typedResult.BitsLeft, typedResult.RequestsLeft, typedResult.Signature);

                generationInfo = (SignedGenerationInfo<TRandom, TValue>)(object)typedGenerationInfo;
            }
            else if (typeof(TRandom) == typeof(SignedBlobsRandom))
            {
                var typedResult = (RpcSignedRandomResult<RpcSignedBlobsRandom, string>)result;

                var typedRandom = new SignedBlobsRandom
                {
                    ApiKeyHash = typedResult.Random.ApiKeyHash,
                    CompletionTime = typedResult.Random.CompletionTime,
                    SerialNumber = typedResult.Random.SerialNumber,
                    Size = (int)typedResult.Random.Size
                };

                var data = new byte[typedResult.Random.Data.Count][];

                for (var i = 0; i < data.Length; i++)
                {
                    data[i] = Convert.FromBase64String(typedResult.Random.Data[i]);
                }

                typedRandom.Data = data;

                var typedGenerationInfo = new SignedGenerationInfo<SignedBlobsRandom, byte[]>(
                    typedRandom, typedResult.BitsUsed, typedResult.BitsLeft, typedResult.RequestsLeft, typedResult.Signature);

                generationInfo = (SignedGenerationInfo<TRandom, TValue>)(object)typedGenerationInfo;
            }

            return generationInfo;
        }

        /// <summary>Verifies the signature of signed random objects and associated data.</summary>
        /// <typeparam name="T">The type of random object.</typeparam>
        /// <param name="random">The signed random objects and associated data.</param>
        /// <param name="signature">The signature from the same response that the random data originates from.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <returns>A value, indicating if the random objects are authentic.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="random" /> or <paramref name="signature" /> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentException"><paramref name="random" /> or <paramref name="signature" /> has invalid values.</exception>
        /// <exception cref="OperationCanceledException">The operation was canceled.</exception>
        /// <exception cref="RandomOrgException">An error occurred during service method invocation.</exception>
        /// <exception cref="RandomOrgRequestException">An HTTP error occurred during service method invocation.</exception>
        public async Task<bool> VerifySignatureAsync<T>(
            SignedRandom<T> random, byte[] signature, CancellationToken cancellationToken)
        {
            if (random == null)
            {
                throw new ArgumentNullException(nameof(random));
            }
            if (signature == null)
            {
                throw new ArgumentNullException(nameof(signature));
            }
            if (random.ApiKeyHash == null)
            {
                throw new ArgumentException(Strings.GetString("client.verify.api_key_hash.not_specified"), nameof(random));
            }
            if (random.ApiKeyHash.Length != 64)
            {
                throw new ArgumentException(Strings.GetString("client.verify.api_key_hash.invalid_length"), nameof(random));
            }
            if (random.Data == null)
            {
                throw new ArgumentException(Strings.GetString("client.verify.data.not_specified"), nameof(random));
            }
            if (random.Data.Count == 0)
            {
                throw new ArgumentException(Strings.GetString("client.verify.data.empty_sequence"), nameof(random));
            }
            if (random.CompletionTime == default)
            {
                throw new ArgumentException(Strings.GetString("client.verify.completion_time.not_specified"), nameof(random));
            }
            if (random.SerialNumber == 0L)
            {
                throw new ArgumentException(Strings.GetString("client.verify.serial_number.not_specified"), nameof(random));
            }
            if (signature.Length == 0)
            {
                throw new ArgumentException(Strings.GetString("client.verify.signature.invalid_length"), nameof(signature));
            }

            var randomType = random.GetType();
            var randomParam = default(object);

            if (randomType == typeof(SignedIntegersRandom))
            {
                var typedRandom = (SignedIntegersRandom)(object)random;

                if ((typedRandom.Data.Count < 1) || (typedRandom.Data.Count > 10000))
                {
                    throw new ArgumentException(Strings.GetString("random.integer.count.invalid_range"), nameof(random));
                }
                if ((typedRandom.Minimum < -1000000000) || (typedRandom.Minimum > 1000000000))
                {
                    throw new ArgumentException(Strings.GetString("random.integer.lower_boundary.invalid_range"), nameof(random));
                }
                if ((typedRandom.Maximum < -1000000000) || (typedRandom.Maximum > 1000000000))
                {
                    throw new ArgumentException(Strings.GetString("random.integer.upper_boundary.invalid_range"), nameof(random));
                }

                var rpcRandom = new RpcSignedIntegersRandom
                {
                    Method = "generateSignedIntegers",
                    Minimum = typedRandom.Minimum,
                    Maximum = typedRandom.Maximum,
                    Replacement = typedRandom.Replacement,
                    Base = 10L
                };

                TransferValues(typedRandom, rpcRandom);

                randomParam = rpcRandom;
            }
            else if (randomType == typeof(SignedDecimalFractionsRandom))
            {
                var typedRandom = (SignedDecimalFractionsRandom)(object)random;

                if ((typedRandom.Data.Count < 1) || (typedRandom.Data.Count > 10000))
                {
                    throw new ArgumentException(Strings.GetString("random.decimal_fraction.count.invalid_range"), nameof(random));
                }
                if ((typedRandom.DecimalPlaces < 1) || (typedRandom.DecimalPlaces > 20))
                {
                    throw new ArgumentException(Strings.GetString("random.decimal_fraction.decimal_places.invalid_range"), nameof(random));
                }

                var rpcRandom = new RpcSignedDecimalFractionsRandom
                {
                    Method = "generateSignedDecimalFractions",
                    DecimalPlaces = typedRandom.DecimalPlaces,
                    Replacement = typedRandom.Replacement
                };

                TransferValues(typedRandom, rpcRandom);

                randomParam = rpcRandom;
            }
            else if (randomType == typeof(SignedGaussiansRandom))
            {
                var typedRandom = (SignedGaussiansRandom)(object)random;

                if ((typedRandom.Data.Count < 1) || (typedRandom.Data.Count > 10000))
                {
                    throw new ArgumentException(Strings.GetString("random.gaussian.count.invalid_range"), nameof(random));
                }
                if ((typedRandom.Mean < -1000000) || (typedRandom.Mean > 1000000))
                {
                    throw new ArgumentException(Strings.GetString("random.gaussian.mean.invalid_range"), nameof(random));
                }
                if ((typedRandom.StandardDeviation < -1000000) || (typedRandom.StandardDeviation > 1000000))
                {
                    throw new ArgumentException(Strings.GetString("random.gaussian.standard_deviation.invalid_range"), nameof(random));
                }
                if ((typedRandom.SignificantDigits < 2) || (typedRandom.SignificantDigits > 20))
                {
                    throw new ArgumentException(Strings.GetString("random.gaussian.significant_digits.invalid_range"), nameof(random));
                }

                var rpcRandom = new RpcSignedGaussiansRandom
                {
                    Method = "generateSignedGaussians",
                    Mean = typedRandom.Mean,
                    StandardDeviation = typedRandom.StandardDeviation,
                    SignificantDigits = typedRandom.SignificantDigits
                };

                TransferValues(typedRandom, rpcRandom);

                randomParam = rpcRandom;
            }
            else if (randomType == typeof(SignedStringsRandom))
            {
                var typedRandom = (SignedStringsRandom)(object)random;

                if ((typedRandom.Data.Count < 1) || (typedRandom.Data.Count > 10000))
                {
                    throw new ArgumentException(Strings.GetString("random.string.count.invalid_range"), nameof(random));
                }
                if ((typedRandom.Length < 1) || (typedRandom.Length > 20))
                {
                    throw new ArgumentException(Strings.GetString("random.string.length.invalid_range"), nameof(random));
                }
                if (typedRandom.Characters == null)
                {
                    throw new ArgumentException(Strings.GetString("random.string.characters.not_specified"), nameof(random));
                }
                if ((typedRandom.Characters.Length < 1) || (typedRandom.Characters.Length > 80))
                {
                    throw new ArgumentException(Strings.GetString("random.string.characters.length.invalid_range"), nameof(random));
                }

                for (var i = 0; i < typedRandom.Data.Count; i++)
                {
                    if (typedRandom.Data[i] == null)
                    {
                        throw new ArgumentException(string.Format(Strings.GetString("random.string.not_specified"), i), nameof(random));
                    }
                }

                var rpcRandom = new RpcSignedStringsRandom
                {
                    Method = "generateSignedStrings",
                    Length = typedRandom.Length,
                    Characters = typedRandom.Characters,
                    Replacement = typedRandom.Replacement
                };

                TransferValues(typedRandom, rpcRandom);

                randomParam = rpcRandom;
            }
            else if (randomType == typeof(SignedUuidsRandom))
            {
                var typedRandom = (SignedUuidsRandom)(object)random;

                if ((typedRandom.Data.Count < 1) || (typedRandom.Data.Count > 1000))
                {
                    throw new ArgumentException(Strings.GetString("random.uuid.count.invalid_range"), nameof(random));
                }

                var rpcRandom = new RpcSignedUuidsRandom
                {
                    Method = "generateSignedUUIDs"
                };

                TransferValues(typedRandom, rpcRandom);

                randomParam = rpcRandom;
            }
            else if (randomType == typeof(SignedBlobsRandom))
            {
                var typedRandom = (SignedBlobsRandom)(object)random;

                if ((typedRandom.Data.Count < 1) || (typedRandom.Data.Count > 100))
                {
                    throw new ArgumentException(Strings.GetString("random.blob.count.invalid_range"), nameof(random));
                }
                if ((typedRandom.Size < 1) || (typedRandom.Size > 1048576))
                {
                    throw new ArgumentException(Strings.GetString("random.blob.size.invalid_range"), nameof(random));
                }
                if (typedRandom.Size % 8 != 0)
                {
                    throw new ArgumentException(Strings.GetString("random.blob.size.invalid_division"), nameof(random));
                }
                if (typedRandom.Data.Count * typedRandom.Size > 1048576)
                {
                    throw new ArgumentException(Strings.GetString("random.blob.invalid_total_size"), nameof(random));
                }

                var rpcRandom = new RpcSignedBlobsRandom
                {
                    Method = "generateSignedBlobs",
                    Count = typedRandom.Data.Count,
                    ApiKeyHash = typedRandom.ApiKeyHash,
                    CompletionTime = typedRandom.CompletionTime,
                    SerialNumber = typedRandom.SerialNumber,
                    Size = typedRandom.Size,
                    Format = "base64"
                };

                var rpcData = new string[typedRandom.Data.Count];

                for (var i = 0; i < rpcData.Length; i++)
                {
                    rpcData[i] = Convert.ToBase64String(typedRandom.Data[i]);
                }

                rpcRandom.Data = rpcData;

                randomParam = rpcRandom;
            }
            else
            {
                throw new NotSupportedException(Strings.GetString("client.verify.random.invalid_type"));
            }

            var @params = new Dictionary<string, object>(2, StringComparer.Ordinal)
            {
                ["random"] = randomParam,
                ["signature"] = Convert.ToBase64String(signature)
            };

            var result = await InvokeRandomOrgMethod<RpcVerifyResult>(
                "verifySignature", @params, cancellationToken).ConfigureAwait(false);

            return result.Authenticity;
        }

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

        private static void TransferValues<T>(RpcRandom<T> source, Random<T> target)
        {
            target.Data = source.Data;
            target.CompletionTime = source.CompletionTime;
        }

        private async Task<TResult> InvokeRandomOrgMethod<TResult, TRandom, TValue>(string method, IReadOnlyDictionary<string, object> @params, CancellationToken cancellationToken)
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

        private async Task<TResult> InvokeRandomOrgMethod<TResult>(string method, IReadOnlyDictionary<string, object> @params, CancellationToken cancellationToken)
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

        private async Task<object> InvokeRandomOrgMethod(string method, IReadOnlyDictionary<string, object> @params, Type resultType, CancellationToken cancellationToken)
        {
            var jsonRpcRequest = new JsonRpcRequest(method, new JsonRpcId(Guid.NewGuid().ToString("D")), @params);
            var httpRequestString = _jsonRpcSerializer.SerializeRequest(jsonRpcRequest);
            var httpResponseString = default(string);

            using (var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, _serviceUri))
            {
                var httpRequestMessageContent = new StringContent(httpRequestString);

                httpRequestMessageContent.Headers.ContentType = _mediaTypeHeaderValue;
                httpRequestMessage.Content = httpRequestMessageContent;

                using (var httpResponseMessage = await _httpMessageInvoker.SendAsync(httpRequestMessage, cancellationToken).ConfigureAwait(false))
                {
                    if (!httpResponseMessage.IsSuccessStatusCode)
                    {
                        throw new RandomOrgRequestException(Strings.GetString("protocol.http.status_code.invalid_value"),
                            method, httpResponseMessage.StatusCode, httpResponseMessage.ReasonPhrase);
                    }

                    var contentType = httpResponseMessage.Content.Headers.ContentType;

                    if (contentType == null)
                    {
                        throw new RandomOrgRequestException(Strings.GetString("protocol.content.type.not_specified"),
                            method, httpResponseMessage.StatusCode, httpResponseMessage.ReasonPhrase);
                    }
                    if (string.Compare(contentType.MediaType, _mediaTypeHeaderValue.MediaType, StringComparison.OrdinalIgnoreCase) != 0)
                    {
                        throw new RandomOrgRequestException(Strings.GetString("protocol.content.type.invalid_value"),
                            method, httpResponseMessage.StatusCode, httpResponseMessage.ReasonPhrase);
                    }

                    var contentLength = httpResponseMessage.Content.Headers.ContentLength;

                    if (contentLength == null)
                    {
                        throw new RandomOrgRequestException(Strings.GetString("protocol.content.length.not_specified"),
                            method, httpResponseMessage.StatusCode, httpResponseMessage.ReasonPhrase);
                    }

                    httpResponseString = await httpResponseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);

                    if (httpResponseString?.Length != contentLength)
                    {
                        throw new RandomOrgRequestException(Strings.GetString("protocol.content.length.invalid_value"),
                            method, httpResponseMessage.StatusCode, httpResponseMessage.ReasonPhrase);
                    }

                    if (resultType != null)
                    {
                        _methodSchemeBindings[jsonRpcRequest.Id] = _signedResultMethodSchemeBindings[resultType];
                    }
                    else
                    {
                        _methodNameBindings[jsonRpcRequest.Id] = jsonRpcRequest.Method;
                    }

                    var responseData = default(JsonRpcData<JsonRpcResponse>);

                    try
                    {
                        responseData = resultType != null ?
                            _jsonRpcSerializer.DeserializeResponseData(httpResponseString, _methodSchemeBindings) :
                            _jsonRpcSerializer.DeserializeResponseData(httpResponseString, _methodNameBindings);
                    }
                    finally
                    {
                        if (resultType != null)
                        {
                            _methodSchemeBindings.Clear();
                        }
                        else
                        {
                            _methodNameBindings.Clear();
                        }
                    }

                    if (!responseData.IsSingle)
                    {
                        throw new RandomOrgRequestException(Strings.GetString("protocol.response.content.invalid_structure"),
                            method, httpResponseMessage.StatusCode, httpResponseMessage.ReasonPhrase);
                    }
                    if (!responseData.SingleItem.IsValid)
                    {
                        throw new RandomOrgRequestException(Strings.GetString("protocol.message.invalid_value"),
                            method, httpResponseMessage.StatusCode, httpResponseMessage.ReasonPhrase);
                    }

                    var jsonRpcResponse = responseData.SingleItem.Message;

                    if (!jsonRpcResponse.Success)
                    {
                        throw new RandomOrgException(jsonRpcResponse.Error.Code, jsonRpcResponse.Error.Message);
                    }
                    if (jsonRpcRequest.Id != jsonRpcResponse.Id)
                    {
                        throw new RandomOrgRequestException(Strings.GetString("protocol.message.identifier.invalid_value"),
                            method, httpResponseMessage.StatusCode, httpResponseMessage.ReasonPhrase);
                    }

                    return jsonRpcResponse.Result;
                }
            }
        }

        private static HttpMessageInvoker CreateHttpMessageInvoker()
        {
            var httpClientHandler = new HttpClientHandler
            {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
            };

            var httpClient = new HttpClient(httpClientHandler);

            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(_mediaTypeHeaderValue.MediaType));
            httpClient.DefaultRequestHeaders.ExpectContinue = false;
            httpClient.Timeout = TimeSpan.FromMinutes(2);

            return httpClient;
        }

        private static JsonRpcSerializer CreateJsonRpcSerializer()
        {
            var scheme = new JsonRpcSerializerScheme();

            scheme.Methods["getUsage"] =
                new JsonRpcMethodScheme(typeof(RpcGetUsageResult), null);
            scheme.Methods["verifySignature"] =
                new JsonRpcMethodScheme(typeof(RpcVerifyResult), null);
            scheme.Methods["generateIntegers"] =
                new JsonRpcMethodScheme(typeof(RpcSimpleRandomResult<int>), null);
            scheme.Methods["generateDecimalFractions"] =
                new JsonRpcMethodScheme(typeof(RpcSimpleRandomResult<decimal>), null);
            scheme.Methods["generateGaussians"] =
                new JsonRpcMethodScheme(typeof(RpcSimpleRandomResult<decimal>), null);
            scheme.Methods["generateStrings"] =
                new JsonRpcMethodScheme(typeof(RpcSimpleRandomResult<string>), null);
            scheme.Methods["generateUUIDs"] =
                new JsonRpcMethodScheme(typeof(RpcSimpleRandomResult<Guid>), null);
            scheme.Methods["generateBlobs"] =
                new JsonRpcMethodScheme(typeof(RpcSimpleRandomResult<string>), null);
            scheme.Methods["generateSignedIntegers"] =
                new JsonRpcMethodScheme(typeof(RpcSignedRandomResult<RpcSignedIntegersRandom, int>), null);
            scheme.Methods["generateSignedDecimalFractions"] =
                new JsonRpcMethodScheme(typeof(RpcSignedRandomResult<RpcSignedDecimalFractionsRandom, decimal>), null);
            scheme.Methods["generateSignedGaussians"] =
                new JsonRpcMethodScheme(typeof(RpcSignedRandomResult<RpcSignedGaussiansRandom, decimal>), null);
            scheme.Methods["generateSignedStrings"] =
                new JsonRpcMethodScheme(typeof(RpcSignedRandomResult<RpcSignedStringsRandom, string>), null);
            scheme.Methods["generateSignedUUIDs"] =
                new JsonRpcMethodScheme(typeof(RpcSignedRandomResult<RpcSignedUuidsRandom, Guid>), null);
            scheme.Methods["generateSignedBlobs"] =
                new JsonRpcMethodScheme(typeof(RpcSignedRandomResult<RpcSignedBlobsRandom, string>), null);

            var settings = new JsonRpcSerializerSettings
            {
                JsonSerializerBufferPool = new JsonBufferPool()
            };

            return new JsonRpcSerializer(scheme, settings);
        }

        private static IReadOnlyDictionary<Type, JsonRpcMethodScheme> CreateSignedResultMethodSchemeBindings()
        {
            return new Dictionary<Type, JsonRpcMethodScheme>(6)
            {
                [typeof(RpcSignedRandomResult<RpcSignedIntegersRandom, int>)] =
                    new JsonRpcMethodScheme(typeof(RpcSignedRandomResult<RpcSignedIntegersRandom, int>), null),
                [typeof(RpcSignedRandomResult<RpcSignedDecimalFractionsRandom, decimal>)] =
                    new JsonRpcMethodScheme(typeof(RpcSignedRandomResult<RpcSignedDecimalFractionsRandom, decimal>), null),
                [typeof(RpcSignedRandomResult<RpcSignedGaussiansRandom, decimal>)] =
                    new JsonRpcMethodScheme(typeof(RpcSignedRandomResult<RpcSignedGaussiansRandom, decimal>), null),
                [typeof(RpcSignedRandomResult<RpcSignedStringsRandom, string>)] =
                    new JsonRpcMethodScheme(typeof(RpcSignedRandomResult<RpcSignedStringsRandom, string>), null),
                [typeof(RpcSignedRandomResult<RpcSignedUuidsRandom, Guid>)] =
                    new JsonRpcMethodScheme(typeof(RpcSignedRandomResult<RpcSignedUuidsRandom, Guid>), null),
                [typeof(RpcSignedRandomResult<RpcSignedBlobsRandom, string>)] =
                    new JsonRpcMethodScheme(typeof(RpcSignedRandomResult<RpcSignedBlobsRandom, string>), null)
            };
        }

        /// <summary>Releases all resources used by the current instance of the <see cref="RandomOrgClient" />.</summary>
        public void Dispose()
        {
            _httpMessageInvoker.Dispose();
            _requestSemaphore.Dispose();
        }
    }
}