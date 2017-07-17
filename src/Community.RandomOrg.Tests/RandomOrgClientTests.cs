using System;
using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Community.RandomOrg.Data;
using Community.RandomOrg.Tests.Resources;
using Community.RandomOrg.Tests.Stubbing;
using Newtonsoft.Json.Linq;
using Xunit;

namespace Community.RandomOrg.Tests
{
    public sealed class RandomOrgClientTests
    {
        private const string _RANDOM_API_KEY = "00000000-0000-0000-0000-000000000000";
        private const string _HTTP_MEDIA_TYPE = "application/json";
        private const string _TIMESTAMP_FORMAT = "yyyy'-'MM'-'dd' 'HH':'mm':'ss.FFFFFFFK";

        private static Task<string> HandleRequestContent(string requestContent, JToken expectedRequestObject, JToken expectedResponseObject)
        {
            var requestJsonObjectActual = JObject.Parse(requestContent);

            expectedRequestObject["id"] = requestJsonObjectActual["id"];
            expectedResponseObject["id"] = requestJsonObjectActual["id"];

            Assert.True(JToken.DeepEquals(expectedRequestObject, requestJsonObjectActual));

            return Task.FromResult(expectedResponseObject.ToString());
        }

        private static void VerifyGenerationInfo<TValue>(SimpleGenerationInfo<TValue> typedObject, JObject jsonObject)
        {
            Assert.NotNull(typedObject);
            Assert.Equal(jsonObject["result"]["bitsUsed"], typedObject.BitsUsed);
            Assert.Equal(jsonObject["result"]["bitsLeft"], typedObject.BitsLeft);
            Assert.Equal(jsonObject["result"]["requestsLeft"], typedObject.RequestsLeft);
            Assert.NotNull(typedObject.Random);
            Assert.Equal(jsonObject["result"]["random"]["completionTime"], typedObject.Random.CompletionTime.ToString(_TIMESTAMP_FORMAT));
            Assert.NotNull(typedObject.Random.Data);
            Assert.Equal(jsonObject["result"]["random"]["data"].ToObject<TValue[]>(), typedObject.Random.Data);
        }

        private static void VerifyGenerationInfo<TRandom, TValue>(SignedGenerationInfo<TRandom, TValue> typedObject, JObject jsonObject)
            where TRandom : SignedRandom<TValue>
        {
            Assert.NotNull(typedObject);
            Assert.Equal(jsonObject["result"]["bitsUsed"], typedObject.BitsUsed);
            Assert.Equal(jsonObject["result"]["bitsLeft"], typedObject.BitsLeft);
            Assert.Equal(jsonObject["result"]["requestsLeft"], typedObject.RequestsLeft);
            Assert.Equal(jsonObject["result"]["signature"], Convert.ToBase64String(typedObject.Signature));
            Assert.NotNull(typedObject.Random);
            Assert.Equal(jsonObject["result"]["random"]["completionTime"], typedObject.Random.CompletionTime.ToString(_TIMESTAMP_FORMAT));
            Assert.Equal(jsonObject["result"]["random"]["hashedApiKey"], Convert.ToBase64String(typedObject.Random.ApiKeyHash));
            Assert.Equal(jsonObject["result"]["random"]["serialNumber"], typedObject.Random.SerialNumber);
            Assert.Equal(jsonObject["result"]["random"]["userData"], typedObject.Random.UserData);
            Assert.NotNull(typedObject.Random.Data);
            Assert.Equal(jsonObject["result"]["random"]["data"].ToObject<TValue[]>(), typedObject.Random.Data);
            Assert.NotNull(typedObject.Random.License);
            Assert.Equal(jsonObject["result"]["random"]["license"]["type"], typedObject.Random.License.Type);
            Assert.Equal(jsonObject["result"]["random"]["license"]["text"], typedObject.Random.License.Text);
            Assert.Equal(jsonObject["result"]["random"]["license"]["infoUrl"], typedObject.Random.License.InfoUrl?.OriginalString);
        }

        private static string ConvertApiKeyStatus(ApiKeyStatus value)
        {
            switch (value)
            {
                case ApiKeyStatus.Stopped:
                    return "stopped";
                case ApiKeyStatus.Paused:
                    return "paused";
                case ApiKeyStatus.Running:
                    return "running";
                default:
                    throw new NotSupportedException();
            }
        }

        [Fact]
        public void ConstructorWhenApiKeyIsNull()
        {
            Assert.Throws<ArgumentNullException>(
                () => new RandomOrgClient(default(string)));
        }

        [Fact]
        public void ConstructorWhenApiKeyIsInvalid()
        {
            Assert.Throws<ArgumentException>(
                () => new RandomOrgClient("test_value"));
        }

        [Fact]
        public void ConstructorWhenHttpMessageInvokerIsNull()
        {
            Assert.Throws<ArgumentNullException>(
                () => new RandomOrgClient(default(HttpMessageInvoker)));
        }

        [Theory]
        [InlineData(00000, 0, 5)]
        [InlineData(10001, 0, 5)]
        [InlineData(1, -1000000001, 5)]
        [InlineData(1, +1000000001, 5)]
        [InlineData(1, 0, -1000000001)]
        [InlineData(1, 0, +1000000001)]
        public async void GenerateIntegersWhenArgumentOutOfRange(int count, int minimum, int maximum)
        {
            var stubHandler = (Func<string, Task<string>>)(requestContent => throw new NotSupportedException());
            var httpClient = new HttpClient(new HttpMessageHandlerStub(stubHandler, _HTTP_MEDIA_TYPE));

            using (var client = new RandomOrgClient(_RANDOM_API_KEY, httpClient))
            {
                await Assert.ThrowsAsync<ArgumentOutOfRangeException>(
                    () => client.GenerateIntegersAsync(count, minimum, maximum, false, CancellationToken.None));
            }
        }

        [Fact]
        public async void GenerateIntegers()
        {
            var requestJsonObject = JObject.Parse(EmbeddedResourceManager.GetString("Assets.gen_pur_int_req.json"));
            var responseJsonObject = JObject.Parse(EmbeddedResourceManager.GetString("Assets.gen_pur_int_res.json"));

            var stubHandler = (Func<string, Task<string>>)
                (requestContent => HandleRequestContent(requestContent, requestJsonObject, responseJsonObject));

            var httpClient = new HttpClient(new HttpMessageHandlerStub(stubHandler, _HTTP_MEDIA_TYPE));

            using (var client = new RandomOrgClient(_RANDOM_API_KEY, httpClient))
            {
                var result = await client.GenerateIntegersAsync(
                    requestJsonObject["params"]["n"].ToObject<int>(),
                    requestJsonObject["params"]["min"].ToObject<int>(),
                    requestJsonObject["params"]["max"].ToObject<int>(),
                    true,
                    CancellationToken.None);

                VerifyGenerationInfo(result, responseJsonObject);
            }
        }

        [Theory]
        [InlineData(00000, 2)]
        [InlineData(10001, 2)]
        [InlineData(1, 00)]
        [InlineData(1, 21)]
        public async void GenerateDecimalFractionsWhenArgumentOutOfRange(int count, int decimalPlaces)
        {
            var stubHandler = (Func<string, Task<string>>)(requestContent => throw new NotSupportedException());
            var httpClient = new HttpClient(new HttpMessageHandlerStub(stubHandler, _HTTP_MEDIA_TYPE));

            using (var client = new RandomOrgClient(_RANDOM_API_KEY, httpClient))
            {
                await Assert.ThrowsAsync<ArgumentOutOfRangeException>(
                    () => client.GenerateDecimalFractionsAsync(count, decimalPlaces, false, CancellationToken.None));
            }
        }

        [Fact]
        public async void GenerateDecimalFractions()
        {
            var requestJsonObject = JObject.Parse(EmbeddedResourceManager.GetString("Assets.gen_pur_dfr_req.json"));
            var responseJsonObject = JObject.Parse(EmbeddedResourceManager.GetString("Assets.gen_pur_dfr_res.json"));

            var stubHandler = (Func<string, Task<string>>)
                (requestContent => HandleRequestContent(requestContent, requestJsonObject, responseJsonObject));

            var httpClient = new HttpClient(new HttpMessageHandlerStub(stubHandler, _HTTP_MEDIA_TYPE));

            using (var client = new RandomOrgClient(_RANDOM_API_KEY, httpClient))
            {
                var result = await client.GenerateDecimalFractionsAsync(
                    requestJsonObject["params"]["n"].ToObject<int>(),
                    requestJsonObject["params"]["decimalPlaces"].ToObject<int>(),
                    true,
                    CancellationToken.None);

                VerifyGenerationInfo(result, responseJsonObject);
            }
        }

        [Theory]
        [InlineData(00000, "0.0", "0.0", 2)]
        [InlineData(10001, "0.0", "0.0", 2)]
        [InlineData(1, "-1000001.0", "0.0", 2)]
        [InlineData(1, "+1000001.0", "0.0", 2)]
        [InlineData(1, "0.0", "-1000001.0", 2)]
        [InlineData(1, "0.0", "+1000001.0", 2)]
        [InlineData(1, "0.0", "0.0", 01)]
        [InlineData(1, "0.0", "0.0", 21)]
        public async void GenerateGaussiansWhenArgumentOutOfRange(int count, string mean, string standardDeviation, int significantDigits)
        {
            var stubHandler = (Func<string, Task<string>>)(requestContent => throw new NotSupportedException());
            var httpClient = new HttpClient(new HttpMessageHandlerStub(stubHandler, _HTTP_MEDIA_TYPE));

            using (var client = new RandomOrgClient(_RANDOM_API_KEY, httpClient))
            {
                var meanValue = decimal.Parse(mean, CultureInfo.InvariantCulture);
                var standardDeviationValue = decimal.Parse(standardDeviation, CultureInfo.InvariantCulture);

                await Assert.ThrowsAsync<ArgumentOutOfRangeException>(
                    () => client.GenerateGaussiansAsync(count, meanValue, standardDeviationValue, significantDigits, CancellationToken.None));
            }
        }

        [Fact]
        public async void GenerateGaussians()
        {
            var requestJsonObject = JObject.Parse(EmbeddedResourceManager.GetString("Assets.gen_pur_gss_req.json"));
            var responseJsonObject = JObject.Parse(EmbeddedResourceManager.GetString("Assets.gen_pur_gss_res.json"));

            var stubHandler = (Func<string, Task<string>>)
                (requestContent => HandleRequestContent(requestContent, requestJsonObject, responseJsonObject));

            var httpClient = new HttpClient(new HttpMessageHandlerStub(stubHandler, _HTTP_MEDIA_TYPE));

            using (var client = new RandomOrgClient(_RANDOM_API_KEY, httpClient))
            {
                var result = await client.GenerateGaussiansAsync(
                    requestJsonObject["params"]["n"].ToObject<int>(),
                    requestJsonObject["params"]["mean"].ToObject<decimal>(),
                    requestJsonObject["params"]["standardDeviation"].ToObject<decimal>(),
                    requestJsonObject["params"]["significantDigits"].ToObject<int>(),
                    CancellationToken.None);

                VerifyGenerationInfo(result, responseJsonObject);
            }
        }

        [Theory]
        [InlineData(00000, 1)]
        [InlineData(10001, 1)]
        [InlineData(1, 00)]
        [InlineData(1, 21)]
        public async void GenerateStringsWhenArgumentOutOfRange(int count, int length)
        {
            var stubHandler = (Func<string, Task<string>>)(requestContent => throw new NotSupportedException());
            var httpClient = new HttpClient(new HttpMessageHandlerStub(stubHandler, _HTTP_MEDIA_TYPE));

            using (var client = new RandomOrgClient(_RANDOM_API_KEY, httpClient))
            {
                await Assert.ThrowsAsync<ArgumentOutOfRangeException>(
                    () => client.GenerateStringsAsync(count, length, "abcde", false, CancellationToken.None));
            }
        }

        [Fact]
        public async void GenerateStringsWhenCharactersIsNull()
        {
            var stubHandler = (Func<string, Task<string>>)(requestContent => throw new NotSupportedException());
            var httpClient = new HttpClient(new HttpMessageHandlerStub(stubHandler, _HTTP_MEDIA_TYPE));

            using (var client = new RandomOrgClient(_RANDOM_API_KEY, httpClient))
            {
                await Assert.ThrowsAsync<ArgumentNullException>(
                    () => client.GenerateStringsAsync(1, 1, null, false, CancellationToken.None));
            }
        }

        [Theory]
        [InlineData(00)]
        [InlineData(81)]
        public async void GenerateStringsWhenCharactersNumberIsInvalid(int number)
        {
            var stubHandler = (Func<string, Task<string>>)(requestContent => throw new NotSupportedException());
            var httpClient = new HttpClient(new HttpMessageHandlerStub(stubHandler, _HTTP_MEDIA_TYPE));

            using (var client = new RandomOrgClient(_RANDOM_API_KEY, httpClient))
            {
                var characters = new string('a', number);

                await Assert.ThrowsAsync<ArgumentException>(
                    () => client.GenerateStringsAsync(1, 1, characters, false, CancellationToken.None));
            }
        }

        [Fact]
        public async void GenerateStrings()
        {
            var requestJsonObject = JObject.Parse(EmbeddedResourceManager.GetString("Assets.gen_pur_str_req.json"));
            var responseJsonObject = JObject.Parse(EmbeddedResourceManager.GetString("Assets.gen_pur_str_res.json"));

            var stubHandler = (Func<string, Task<string>>)
                (requestContent => HandleRequestContent(requestContent, requestJsonObject, responseJsonObject));

            var httpClient = new HttpClient(new HttpMessageHandlerStub(stubHandler, _HTTP_MEDIA_TYPE));

            using (var client = new RandomOrgClient(_RANDOM_API_KEY, httpClient))
            {
                var result = await client.GenerateStringsAsync(
                    requestJsonObject["params"]["n"].ToObject<int>(),
                    requestJsonObject["params"]["length"].ToObject<int>(),
                    requestJsonObject["params"]["characters"].ToObject<string>(),
                    true,
                    CancellationToken.None);

                VerifyGenerationInfo(result, responseJsonObject);
            }
        }

        [Theory]
        [InlineData(0000)]
        [InlineData(1001)]
        public async void GenerateUuidsWhenArgumentOutOfRange(int count)
        {
            var stubHandler = (Func<string, Task<string>>)(requestContent => throw new NotSupportedException());
            var httpClient = new HttpClient(new HttpMessageHandlerStub(stubHandler, _HTTP_MEDIA_TYPE));

            using (var client = new RandomOrgClient(_RANDOM_API_KEY, httpClient))
            {
                await Assert.ThrowsAsync<ArgumentOutOfRangeException>(
                    () => client.GenerateUuidsAsync(count, CancellationToken.None));
            }
        }

        [Fact]
        public async void GenerateUuids()
        {
            var requestJsonObject = JObject.Parse(EmbeddedResourceManager.GetString("Assets.gen_pur_uid_req.json"));
            var responseJsonObject = JObject.Parse(EmbeddedResourceManager.GetString("Assets.gen_pur_uid_res.json"));

            var stubHandler = (Func<string, Task<string>>)
                (requestContent => HandleRequestContent(requestContent, requestJsonObject, responseJsonObject));

            var httpClient = new HttpClient(new HttpMessageHandlerStub(stubHandler, _HTTP_MEDIA_TYPE));

            using (var client = new RandomOrgClient(_RANDOM_API_KEY, httpClient))
            {
                var result = await client.GenerateUuidsAsync(
                    requestJsonObject["params"]["n"].ToObject<int>(),
                    CancellationToken.None);

                VerifyGenerationInfo(result, responseJsonObject);
            }
        }

        [Theory]
        [InlineData(000, 0000008)]
        [InlineData(101, 0000008)]
        [InlineData(001, 0000000)]
        [InlineData(001, 1048577)]
        [InlineData(001, 0000007)]
        [InlineData(002, 1048576)]
        public async void GenerateBlobsWhenArgumentOutOfRange(int count, int size)
        {
            var stubHandler = (Func<string, Task<string>>)(requestContent => throw new NotSupportedException());
            var httpClient = new HttpClient(new HttpMessageHandlerStub(stubHandler, _HTTP_MEDIA_TYPE));

            using (var client = new RandomOrgClient(_RANDOM_API_KEY, httpClient))
            {
                await Assert.ThrowsAsync<ArgumentOutOfRangeException>(
                    () => client.GenerateBlobsAsync(count, size, CancellationToken.None));
            }
        }

        [Fact]
        public async void GenerateBlobs()
        {
            var requestJsonObject = JObject.Parse(EmbeddedResourceManager.GetString("Assets.gen_pur_blb_req.json"));
            var responseJsonObject = JObject.Parse(EmbeddedResourceManager.GetString("Assets.gen_pur_blb_res.json"));

            var stubHandler = (Func<string, Task<string>>)
                (requestContent => HandleRequestContent(requestContent, requestJsonObject, responseJsonObject));

            var httpClient = new HttpClient(new HttpMessageHandlerStub(stubHandler, _HTTP_MEDIA_TYPE));

            using (var client = new RandomOrgClient(_RANDOM_API_KEY, httpClient))
            {
                var result = await client.GenerateBlobsAsync(
                    requestJsonObject["params"]["n"].ToObject<int>(),
                    requestJsonObject["params"]["size"].ToObject<int>(),
                    CancellationToken.None);

                Assert.NotNull(result);
                Assert.Equal(responseJsonObject["result"]["bitsUsed"], result.BitsUsed);
                Assert.Equal(responseJsonObject["result"]["bitsLeft"], result.BitsLeft);
                Assert.Equal(responseJsonObject["result"]["requestsLeft"], result.RequestsLeft);
                Assert.NotNull(result.Random);
                Assert.Equal(responseJsonObject["result"]["random"]["completionTime"], result.Random.CompletionTime.ToString(_TIMESTAMP_FORMAT));
                Assert.NotNull(result.Random.Data);

                var dataJsonObject = (JArray)responseJsonObject["result"]["random"]["data"];

                for (var i = 0; i < dataJsonObject.Count; i++)
                {
                    Assert.Equal(dataJsonObject[i], Convert.ToBase64String(result.Random.Data[i]));
                }
            }
        }

        [Fact]
        public async void GetUsageWhenApiKeyIsNull()
        {
            var stubHandler = (Func<string, Task<string>>)(requestContent => throw new NotSupportedException());
            var httpClient = new HttpClient(new HttpMessageHandlerStub(stubHandler, _HTTP_MEDIA_TYPE));

            using (var client = new RandomOrgClient(httpClient))
            {
                await Assert.ThrowsAsync<InvalidOperationException>(
                    () => client.GetUsageAsync(CancellationToken.None));
            }
        }

        [Fact]
        public async void GetUsage()
        {
            var requestJsonObject = JObject.Parse(EmbeddedResourceManager.GetString("Assets.get_usg_req.json"));
            var responseJsonObject = JObject.Parse(EmbeddedResourceManager.GetString("Assets.get_usg_res.json"));

            var stubHandler = (Func<string, Task<string>>)
                (requestContent => HandleRequestContent(requestContent, requestJsonObject, responseJsonObject));

            var httpClient = new HttpClient(new HttpMessageHandlerStub(stubHandler, _HTTP_MEDIA_TYPE));

            using (var client = new RandomOrgClient(_RANDOM_API_KEY, httpClient))
            {
                var result = await client.GetUsageAsync(CancellationToken.None);

                Assert.NotNull(result);
                Assert.Equal(responseJsonObject["result"]["status"], ConvertApiKeyStatus(result.Status));
                Assert.Equal(responseJsonObject["result"]["creationTime"], result.CreationTime.ToString(_TIMESTAMP_FORMAT));
                Assert.Equal(responseJsonObject["result"]["bitsLeft"], result.BitsLeft);
                Assert.Equal(responseJsonObject["result"]["requestsLeft"], result.RequestsLeft);
                Assert.Equal(responseJsonObject["result"]["totalBits"], result.TotalBits);
                Assert.Equal(responseJsonObject["result"]["totalRequests"], result.TotalRequests);
            }
        }

        [Theory]
        [InlineData(00000, 0, 5)]
        [InlineData(10001, 0, 5)]
        [InlineData(1, -1000000001, 5)]
        [InlineData(1, +1000000001, 5)]
        [InlineData(1, 0, -1000000001)]
        [InlineData(1, 0, +1000000001)]
        public async void GenerateSignedIntegersWhenArgumentOutOfRange(int count, int minimum, int maximum)
        {
            var stubHandler = (Func<string, Task<string>>)(requestContent => throw new NotSupportedException());
            var httpClient = new HttpClient(new HttpMessageHandlerStub(stubHandler, _HTTP_MEDIA_TYPE));

            using (var client = new RandomOrgClient(_RANDOM_API_KEY, httpClient))
            {
                await Assert.ThrowsAsync<ArgumentOutOfRangeException>(
                    () => client.GenerateSignedIntegersAsync(count, minimum, maximum, false, null, CancellationToken.None));
            }
        }

        [Fact]
        public async void GenerateSignedIntegers()
        {
            var requestJsonObject = JObject.Parse(EmbeddedResourceManager.GetString("Assets.gen_sig_int_req.json"));
            var responseJsonObject = JObject.Parse(EmbeddedResourceManager.GetString("Assets.gen_sig_int_res.json"));

            var stubHandler = (Func<string, Task<string>>)
                (requestContent => HandleRequestContent(requestContent, requestJsonObject, responseJsonObject));

            var httpClient = new HttpClient(new HttpMessageHandlerStub(stubHandler, _HTTP_MEDIA_TYPE));

            using (var client = new RandomOrgClient(_RANDOM_API_KEY, httpClient))
            {
                var result = await client.GenerateSignedIntegersAsync(
                    requestJsonObject["params"]["n"].ToObject<int>(),
                    requestJsonObject["params"]["min"].ToObject<int>(),
                    requestJsonObject["params"]["max"].ToObject<int>(),
                    true,
                    null,
                    CancellationToken.None);

                VerifyGenerationInfo(result, responseJsonObject);

                Assert.Equal(responseJsonObject["result"]["random"]["min"], result.Random.Minimum);
                Assert.Equal(responseJsonObject["result"]["random"]["max"], result.Random.Maximum);
                Assert.Equal(responseJsonObject["result"]["random"]["replacement"], result.Random.Replacement);
            }
        }

        [Theory]
        [InlineData(00000, 2)]
        [InlineData(10001, 2)]
        [InlineData(1, 00)]
        [InlineData(1, 21)]
        public async void GenerateSignedDecimalFractionsWhenArgumentOutOfRange(int count, int decimalPlaces)
        {
            var stubHandler = (Func<string, Task<string>>)(requestContent => throw new NotSupportedException());
            var httpClient = new HttpClient(new HttpMessageHandlerStub(stubHandler, _HTTP_MEDIA_TYPE));

            using (var client = new RandomOrgClient(_RANDOM_API_KEY, httpClient))
            {
                await Assert.ThrowsAsync<ArgumentOutOfRangeException>(
                    () => client.GenerateSignedDecimalFractionsAsync(count, decimalPlaces, false, null, CancellationToken.None));
            }
        }

        [Fact]
        public async void GenerateSignedDecimalFractions()
        {
            var requestJsonObject = JObject.Parse(EmbeddedResourceManager.GetString("Assets.gen_sig_dfr_req.json"));
            var responseJsonObject = JObject.Parse(EmbeddedResourceManager.GetString("Assets.gen_sig_dfr_res.json"));

            var stubHandler = (Func<string, Task<string>>)
                (requestContent => HandleRequestContent(requestContent, requestJsonObject, responseJsonObject));

            var httpClient = new HttpClient(new HttpMessageHandlerStub(stubHandler, _HTTP_MEDIA_TYPE));

            using (var client = new RandomOrgClient(_RANDOM_API_KEY, httpClient))
            {
                var result = await client.GenerateSignedDecimalFractionsAsync(
                    requestJsonObject["params"]["n"].ToObject<int>(),
                    requestJsonObject["params"]["decimalPlaces"].ToObject<int>(),
                    true,
                    null,
                    CancellationToken.None);

                VerifyGenerationInfo(result, responseJsonObject);

                Assert.Equal(responseJsonObject["result"]["random"]["decimalPlaces"], result.Random.DecimalPlaces);
                Assert.Equal(responseJsonObject["result"]["random"]["replacement"], result.Random.Replacement);
            }
        }

        [Theory]
        [InlineData(00000, "0.0", "0.0", 2)]
        [InlineData(10001, "0.0", "0.0", 2)]
        [InlineData(1, "-1000001.0", "0.0", 2)]
        [InlineData(1, "+1000001.0", "0.0", 2)]
        [InlineData(1, "0.0", "-1000001.0", 2)]
        [InlineData(1, "0.0", "+1000001.0", 2)]
        [InlineData(1, "0.0", "0.0", 01)]
        [InlineData(1, "0.0", "0.0", 21)]
        public async void GenerateSignedGaussiansWhenArgumentOutOfRange(int count, string mean, string standardDeviation, int significantDigits)
        {
            var stubHandler = (Func<string, Task<string>>)(requestContent => throw new NotSupportedException());
            var httpClient = new HttpClient(new HttpMessageHandlerStub(stubHandler, _HTTP_MEDIA_TYPE));

            using (var client = new RandomOrgClient(_RANDOM_API_KEY, httpClient))
            {
                var meanValue = decimal.Parse(mean, CultureInfo.InvariantCulture);
                var standardDeviationValue = decimal.Parse(standardDeviation, CultureInfo.InvariantCulture);

                await Assert.ThrowsAsync<ArgumentOutOfRangeException>(
                    () => client.GenerateSignedGaussiansAsync(count, meanValue, standardDeviationValue, significantDigits, null, CancellationToken.None));
            }
        }

        [Fact]
        public async void GenerateSignedGaussians()
        {
            var requestJsonObject = JObject.Parse(EmbeddedResourceManager.GetString("Assets.gen_sig_gss_req.json"));
            var responseJsonObject = JObject.Parse(EmbeddedResourceManager.GetString("Assets.gen_sig_gss_res.json"));

            var stubHandler = (Func<string, Task<string>>)
                (requestContent => HandleRequestContent(requestContent, requestJsonObject, responseJsonObject));

            var httpClient = new HttpClient(new HttpMessageHandlerStub(stubHandler, _HTTP_MEDIA_TYPE));

            using (var client = new RandomOrgClient(_RANDOM_API_KEY, httpClient))
            {
                var result = await client.GenerateSignedGaussiansAsync(
                    requestJsonObject["params"]["n"].ToObject<int>(),
                    requestJsonObject["params"]["mean"].ToObject<decimal>(),
                    requestJsonObject["params"]["standardDeviation"].ToObject<decimal>(),
                    requestJsonObject["params"]["significantDigits"].ToObject<int>(),
                    null,
                    CancellationToken.None);

                VerifyGenerationInfo(result, responseJsonObject);

                Assert.Equal(responseJsonObject["result"]["random"]["mean"], result.Random.Mean);
                Assert.Equal(responseJsonObject["result"]["random"]["standardDeviation"], result.Random.StandardDeviation);
                Assert.Equal(responseJsonObject["result"]["random"]["significantDigits"], result.Random.SignificantDigits);
            }
        }

        [Theory]
        [InlineData(00000, 1)]
        [InlineData(10001, 1)]
        [InlineData(1, 00)]
        [InlineData(1, 21)]
        public async void GenerateSignedStringsWhenArgumentOutOfRange(int count, int length)
        {
            var stubHandler = (Func<string, Task<string>>)(requestContent => throw new NotSupportedException());
            var httpClient = new HttpClient(new HttpMessageHandlerStub(stubHandler, _HTTP_MEDIA_TYPE));

            using (var client = new RandomOrgClient(_RANDOM_API_KEY, httpClient))
            {
                await Assert.ThrowsAsync<ArgumentOutOfRangeException>(
                    () => client.GenerateSignedStringsAsync(count, length, "abcde", false, null, CancellationToken.None));
            }
        }

        [Fact]
        public async void GenerateSignedStringsWhenCharactersIsNull()
        {
            var stubHandler = (Func<string, Task<string>>)(requestContent => throw new NotSupportedException());
            var httpClient = new HttpClient(new HttpMessageHandlerStub(stubHandler, _HTTP_MEDIA_TYPE));

            using (var client = new RandomOrgClient(_RANDOM_API_KEY, httpClient))
            {
                await Assert.ThrowsAsync<ArgumentNullException>(
                    () => client.GenerateSignedStringsAsync(1, 1, null, false, null, CancellationToken.None));
            }
        }

        [Fact]
        public async void GenerateSignedStringsWhenCharactersCountIsInvalid()
        {
            var stubHandler = (Func<string, Task<string>>)(requestContent => throw new NotSupportedException());
            var httpClient = new HttpClient(new HttpMessageHandlerStub(stubHandler, _HTTP_MEDIA_TYPE));

            using (var client = new RandomOrgClient(_RANDOM_API_KEY, httpClient))
            {
                var characters = new string('a', 81);

                await Assert.ThrowsAsync<ArgumentException>(
                    () => client.GenerateSignedStringsAsync(1, 1, characters, false, null, CancellationToken.None));
            }
        }

        [Fact]
        public async void GenerateSignedStrings()
        {
            var requestJsonObject = JObject.Parse(EmbeddedResourceManager.GetString("Assets.gen_sig_str_req.json"));
            var responseJsonObject = JObject.Parse(EmbeddedResourceManager.GetString("Assets.gen_sig_str_res.json"));

            var stubHandler = (Func<string, Task<string>>)
                (requestContent => HandleRequestContent(requestContent, requestJsonObject, responseJsonObject));

            var httpClient = new HttpClient(new HttpMessageHandlerStub(stubHandler, _HTTP_MEDIA_TYPE));

            using (var client = new RandomOrgClient(_RANDOM_API_KEY, httpClient))
            {
                var result = await client.GenerateSignedStringsAsync(
                    requestJsonObject["params"]["n"].ToObject<int>(),
                    requestJsonObject["params"]["length"].ToObject<int>(),
                    requestJsonObject["params"]["characters"].ToObject<string>(),
                    true,
                    null,
                    CancellationToken.None);

                VerifyGenerationInfo(result, responseJsonObject);

                Assert.Equal(responseJsonObject["result"]["random"]["length"], result.Random.Length);
                Assert.Equal(responseJsonObject["result"]["random"]["characters"], result.Random.Characters);
                Assert.Equal(responseJsonObject["result"]["random"]["replacement"], result.Random.Replacement);
            }
        }

        [Theory]
        [InlineData(0000)]
        [InlineData(1001)]
        public async void GenerateSignedUuidsWhenArgumentOutOfRange(int count)
        {
            var stubHandler = (Func<string, Task<string>>)(requestContent => throw new NotSupportedException());
            var httpClient = new HttpClient(new HttpMessageHandlerStub(stubHandler, _HTTP_MEDIA_TYPE));

            using (var client = new RandomOrgClient(_RANDOM_API_KEY, httpClient))
            {
                await Assert.ThrowsAsync<ArgumentOutOfRangeException>(
                    () => client.GenerateSignedUuidsAsync(count, null, CancellationToken.None));
            }
        }

        [Fact]
        public async void GenerateSignedUuids()
        {
            var requestJsonObject = JObject.Parse(EmbeddedResourceManager.GetString("Assets.gen_sig_uid_req.json"));
            var responseJsonObject = JObject.Parse(EmbeddedResourceManager.GetString("Assets.gen_sig_uid_res.json"));

            var stubHandler = (Func<string, Task<string>>)
                (requestContent => HandleRequestContent(requestContent, requestJsonObject, responseJsonObject));

            var httpClient = new HttpClient(new HttpMessageHandlerStub(stubHandler, _HTTP_MEDIA_TYPE));

            using (var client = new RandomOrgClient(_RANDOM_API_KEY, httpClient))
            {
                var result = await client.GenerateSignedUuidsAsync(
                    requestJsonObject["params"]["n"].ToObject<int>(),
                    null,
                    CancellationToken.None);

                VerifyGenerationInfo(result, responseJsonObject);
            }
        }

        [Theory]
        [InlineData(000, 0000008)]
        [InlineData(101, 0000008)]
        [InlineData(001, 0000000)]
        [InlineData(001, 1048577)]
        [InlineData(001, 0000007)]
        [InlineData(002, 1048576)]
        public async void GenerateSignedBlobsWhenArgumentOutOfRange(int count, int size)
        {
            var stubHandler = (Func<string, Task<string>>)(requestContent => throw new NotSupportedException());
            var httpClient = new HttpClient(new HttpMessageHandlerStub(stubHandler, _HTTP_MEDIA_TYPE));

            using (var client = new RandomOrgClient(_RANDOM_API_KEY, httpClient))
            {
                await Assert.ThrowsAsync<ArgumentOutOfRangeException>(
                    () => client.GenerateSignedBlobsAsync(count, size, null, CancellationToken.None));
            }
        }

        [Fact]
        public async void GenerateSignedBlobs()
        {
            var requestJsonObject = JObject.Parse(EmbeddedResourceManager.GetString("Assets.gen_sig_blb_req.json"));
            var responseJsonObject = JObject.Parse(EmbeddedResourceManager.GetString("Assets.gen_sig_blb_res.json"));

            var stubHandler = (Func<string, Task<string>>)
                (requestContent => HandleRequestContent(requestContent, requestJsonObject, responseJsonObject));

            var httpClient = new HttpClient(new HttpMessageHandlerStub(stubHandler, _HTTP_MEDIA_TYPE));

            using (var client = new RandomOrgClient(_RANDOM_API_KEY, httpClient))
            {
                var result = await client.GenerateSignedBlobsAsync(
                    requestJsonObject["params"]["n"].ToObject<int>(),
                    requestJsonObject["params"]["size"].ToObject<int>(),
                    null,
                    CancellationToken.None);

                VerifyGenerationInfo(result, responseJsonObject);

                Assert.Equal(responseJsonObject["result"]["random"]["size"], result.Random.Size);
            }
        }

        [Fact]
        public async void VerifySignatureWhenRandomIsNull()
        {
            var stubHandler = (Func<string, Task<string>>)(requestContent => throw new NotSupportedException());
            var httpClient = new HttpClient(new HttpMessageHandlerStub(stubHandler, _HTTP_MEDIA_TYPE));

            using (var client = new RandomOrgClient(_RANDOM_API_KEY, httpClient))
            {
                var signature = Convert.FromBase64String(
                    "Pd46xecjlt/EEOdNGFQNCFHVJhZ5lfVUFLziDusOpLIKGbbHh4kCRM8+el8xh3ASUgR7qfL+K7pzERsIIHIheiIt8EXq9Xr/DW3N" +
                    "3qfKL/4Cai+zyBGn4xThSJnXSJPvM+LuXI+B2dPx7GYJa2PSVgF+fV3j0eN+exAa4fjAQFGiPDQo+dCQO3cSO8aZ76tSGJpo/6b1" +
                    "rR/sdIt8uAWRsfL56tFA+2+xUtuXU5vM8HT+KO6o4N7TWpvBz1rw/0S9iiIRdIDJ5M/AjKXeXKMMkDBHUaeUWCDEZkZOMYTk6IvO" +
                    "pERlsWz0dz2xrs4NE8vZsi4nIVU5izGLYceMRf2TmZVC7XXdZDbeEnJnCmDacRBONqTYklJ22wGrXrFs8GElEoG3IqOgjS1ZME3/" +
                    "jBGW7G3eUB1xb0lT773V2YvGBPTqsnz3tawO8UDdIpoGjSQjtgql/j7gzNmyW6AC9wvrLyXeh7OP6rhp3SLNUAjNhr+QqjHrqPEv" +
                    "+QqkZspYCxTMVyRILCe+z+jTbNnTClVpgxIP5hSfLkinea/TpomZqWj6KwYPqKuiNMrEXsbWlAytzr1v1tsbHbESb2XKytUDWHqm" +
                    "BDCNxHphcgVTmn9+bi2+WPjueBBUH+b0ZOvHIZNxld9P76x8IlcOVKRLl770j0yD3ActIZNaHvUNYeBoK2M=");

                await Assert.ThrowsAsync<ArgumentNullException>(
                    () => client.VerifySignatureAsync(default(SignedIntegersRandom), signature, CancellationToken.None));
            }
        }

        [Fact]
        public async void VerifySignatureWhenSignatureIsNull()
        {
            var stubHandler = (Func<string, Task<string>>)(requestContent => throw new NotSupportedException());
            var httpClient = new HttpClient(new HttpMessageHandlerStub(stubHandler, _HTTP_MEDIA_TYPE));

            using (var client = new RandomOrgClient(_RANDOM_API_KEY, httpClient))
            {
                var random = new SignedIntegersRandom();

                await Assert.ThrowsAsync<ArgumentNullException>(
                    () => client.VerifySignatureAsync(random, null, CancellationToken.None));
            }
        }

        [Fact]
        public async void VerifySignature()
        {
            var requestJsonObject = JObject.Parse(EmbeddedResourceManager.GetString("Assets.ver_sig_req.json"));
            var responseJsonObject = JObject.Parse(EmbeddedResourceManager.GetString("Assets.ver_sig_res.json"));

            var stubHandler = (Func<string, Task<string>>)
                (requestContent => HandleRequestContent(requestContent, requestJsonObject, responseJsonObject));

            var httpClient = new HttpClient(new HttpMessageHandlerStub(stubHandler, _HTTP_MEDIA_TYPE));

            using (var client = new RandomOrgClient(httpClient))
            {
                var random = new SignedIntegersRandom
                {
                    ApiKeyHash = Convert.FromBase64String(requestJsonObject["params"]["random"]["hashedApiKey"].ToObject<string>()),
                    CompletionTime = requestJsonObject["params"]["random"]["completionTime"].ToObject<DateTime>(),
                    SerialNumber = requestJsonObject["params"]["random"]["serialNumber"].ToObject<int>(),
                    Minimum = requestJsonObject["params"]["random"]["min"].ToObject<int>(),
                    Maximum = requestJsonObject["params"]["random"]["max"].ToObject<int>(),
                    Replacement = requestJsonObject["params"]["random"]["replacement"].ToObject<bool>(),
                    Data = requestJsonObject["params"]["random"]["data"].ToObject<int[]>(),
                    UserData = requestJsonObject["params"]["random"]["userData"].ToObject<string>(),
                };

                random.License.Type = "commercial-2";
                random.License.Text = "These values are licensed for commercial (non-gambling) use.";
                random.License.InfoUrl = new Uri("https://api.random.org/licenses/commercial-2");

                var signature = Convert.FromBase64String(requestJsonObject["params"]["signature"].ToObject<string>());
                var result = await client.VerifySignatureAsync(random, signature, CancellationToken.None);

                Assert.True(result);
            }
        }

        [Fact]
        public async void GetResult()
        {
            var requestJsonObject = JObject.Parse(EmbeddedResourceManager.GetString("Assets.get_res_req.json"));
            var responseJsonObject = JObject.Parse(EmbeddedResourceManager.GetString("Assets.get_res_res.json"));

            var stubHandler = (Func<string, Task<string>>)
                (requestContent => HandleRequestContent(requestContent, requestJsonObject, responseJsonObject));

            var httpClient = new HttpClient(new HttpMessageHandlerStub(stubHandler, _HTTP_MEDIA_TYPE));

            using (var client = new RandomOrgClient(_RANDOM_API_KEY, httpClient))
            {
                var result = await client.GetResultAsync<SignedIntegersRandom, int>(
                    requestJsonObject["params"]["serialNumber"].ToObject<int>(),
                    CancellationToken.None);

                VerifyGenerationInfo(result, responseJsonObject);

                Assert.Equal(responseJsonObject["result"]["random"]["min"], result.Random.Minimum);
                Assert.Equal(responseJsonObject["result"]["random"]["max"], result.Random.Maximum);
                Assert.Equal(responseJsonObject["result"]["random"]["replacement"], result.Random.Replacement);
            }
        }

        [Fact]
        public async void GetResultWhenRandomTypeIsInvalid()
        {
            var requestJsonObject = JObject.Parse(EmbeddedResourceManager.GetString("Assets.get_res_req.json"));
            var responseJsonObject = JObject.Parse(EmbeddedResourceManager.GetString("Assets.get_res_res.json"));

            var stubHandler = (Func<string, Task<string>>)
                (requestContent => HandleRequestContent(requestContent, requestJsonObject, responseJsonObject));

            var httpClient = new HttpClient(new HttpMessageHandlerStub(stubHandler, _HTTP_MEDIA_TYPE));

            using (var client = new RandomOrgClient(_RANDOM_API_KEY, httpClient))
            {
                await Assert.ThrowsAsync<InvalidOperationException>(
                    () => client.GetResultAsync<SignedUuidsRandom, Guid>(
                        requestJsonObject["params"]["serialNumber"].ToObject<int>(),
                        CancellationToken.None));
            }
        }

        [Fact]
        public async void InvokeMethodWhenHttpStatusCodeIsInvalid()
        {
            var stubHandler = (Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>>)
                (
                    (requestMessage, cancellationToken) =>
                    {
                        var responseContent = EmbeddedResourceManager.GetString("Assets.get_usg_res.json");

                        var responseMessage = new HttpResponseMessage
                        {
                            RequestMessage = requestMessage,
                            StatusCode = HttpStatusCode.BadRequest,
                            Content = new StringContent(responseContent, Encoding.UTF8, _HTTP_MEDIA_TYPE),
                            Version = new Version(1, 1)
                        };

                        return Task.FromResult(responseMessage);
                    }
                );

            var httpClient = new HttpClient(new HttpMessageHandlerStub(stubHandler));

            using (var client = new RandomOrgClient(_RANDOM_API_KEY, httpClient))
            {
                await Assert.ThrowsAsync<HttpRequestException>(
                    () => client.GetUsageAsync(CancellationToken.None));
            }
        }

        [Fact]
        public async void InvokeMethodWhenContentTypeIsInvalid()
        {
            var stubHandler = (Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>>)
                (
                    (requestMessage, cancellationToken) =>
                    {
                        var responseContentString = EmbeddedResourceManager.GetString("Assets.get_usg_res.json");
                        var responseContent = new StringContent(responseContentString, Encoding.UTF8, "application/octet-stream");

                        var responseMessage = new HttpResponseMessage
                        {
                            RequestMessage = requestMessage,
                            StatusCode = HttpStatusCode.OK,
                            Content = responseContent,
                            Version = new Version(1, 1)
                        };

                        return Task.FromResult(responseMessage);
                    }
                );

            var httpClient = new HttpClient(new HttpMessageHandlerStub(stubHandler));

            using (var client = new RandomOrgClient(_RANDOM_API_KEY, httpClient))
            {
                await Assert.ThrowsAsync<HttpRequestException>(
                    () => client.GetUsageAsync(CancellationToken.None));
            }
        }

        [Fact]
        public async void InvokeMethodWhenContentLengthIsInvalid()
        {
            var stubHandler = (Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>>)
            (
                (requestMessage, cancellationToken) =>
                {
                    var responseContentString = EmbeddedResourceManager.GetString("Assets.get_usg_res.json");
                    var responseContent = new StringContent(responseContentString, Encoding.UTF8, _HTTP_MEDIA_TYPE);

                    responseContent.Headers.ContentLength = 2;

                    var responseMessage = new HttpResponseMessage
                    {
                        RequestMessage = requestMessage,
                        StatusCode = HttpStatusCode.OK,
                        Content = responseContent,
                        Version = new Version(1, 1)
                    };

                    return Task.FromResult(responseMessage);
                }
            );

            var httpClient = new HttpClient(new HttpMessageHandlerStub(stubHandler));

            using (var client = new RandomOrgClient(_RANDOM_API_KEY, httpClient))
            {
                await Assert.ThrowsAsync<HttpRequestException>(
                    () => client.GetUsageAsync(CancellationToken.None));
            }
        }

        [Fact]
        public async void RandomOrgErrorWithNoData()
        {
            var requestJsonObject = JObject.Parse(EmbeddedResourceManager.GetString("Assets.get_usg_req_101.json"));
            var responseJsonObject = JObject.Parse(EmbeddedResourceManager.GetString("Assets.get_usg_res_101.json"));

            var stubHandler = (Func<string, Task<string>>)
                (requestContent => HandleRequestContent(requestContent, requestJsonObject, responseJsonObject));

            var httpClient = new HttpClient(new HttpMessageHandlerStub(stubHandler, _HTTP_MEDIA_TYPE));

            using (var client = new RandomOrgClient(_RANDOM_API_KEY, httpClient))
            {
                var exception = await Assert.ThrowsAsync<RandomOrgException>(
                    () => client.GetUsageAsync(CancellationToken.None));

                Assert.Equal(101, exception.Code);
            }
        }

        [Fact]
        public async void RandomOrgErrorWithData()
        {
            var requestJsonObject = JObject.Parse(EmbeddedResourceManager.GetString("Assets.get_usg_req_501.json"));
            var responseJsonObject = JObject.Parse(EmbeddedResourceManager.GetString("Assets.get_usg_res_501.json"));

            var stubHandler = (Func<string, Task<string>>)
                (requestContent => HandleRequestContent(requestContent, requestJsonObject, responseJsonObject));

            var httpClient = new HttpClient(new HttpMessageHandlerStub(stubHandler, _HTTP_MEDIA_TYPE));

            using (var client = new RandomOrgClient(_RANDOM_API_KEY, httpClient))
            {
                var exception = await Assert.ThrowsAsync<RandomOrgException>(
                    () => client.GetUsageAsync(CancellationToken.None));

                Assert.Equal(501, exception.Code);
            }
        }
    }
}