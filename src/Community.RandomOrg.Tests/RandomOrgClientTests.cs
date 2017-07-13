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

        private static Task<string> HandleRequestContent(string requestContent, string expectedRequestAsset, string expectedResponseAsset)
        {
            var requestJsonObjectActual = JObject.Parse(requestContent);
            var requestJsonObject = JObject.Parse(EmbeddedResourceManager.GetString(expectedRequestAsset));
            var responseJsonObject = JObject.Parse(EmbeddedResourceManager.GetString(expectedResponseAsset));

            requestJsonObject["id"] = requestJsonObjectActual["id"];
            responseJsonObject["id"] = requestJsonObjectActual["id"];

            Assert.True(JToken.DeepEquals(requestJsonObject, requestJsonObjectActual));

            return Task.FromResult(responseJsonObject.ToString());
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
                    () => client.GenerateIntegersAsync(count, minimum, maximum, false));
            }
        }

        [Fact]
        public async void GenerateIntegers()
        {
            var stubHandler = (Func<string, Task<string>>)
                (requestContent => HandleRequestContent(requestContent, "Assets.gen_pur_int_req.json", "Assets.gen_pur_int_res.json"));

            var httpClient = new HttpClient(new HttpMessageHandlerStub(stubHandler, _HTTP_MEDIA_TYPE));

            using (var client = new RandomOrgClient(_RANDOM_API_KEY, httpClient))
            {
                var result = await client.GenerateIntegersAsync(6, 1, 6, true);

                Assert.NotNull(result);
                Assert.Equal(16, result.BitsUsed);
                Assert.Equal(199984, result.BitsLeft);
                Assert.Equal(9999, result.RequestsLeft);
                Assert.NotNull(result.Random);
                Assert.Equal(new DateTime(2011, 10, 10, 13, 19, 12, 00, DateTimeKind.Utc), result.Random.CompletionTime);
                Assert.NotNull(result.Random.Data);

                var data = new[]
                {
                    1,
                    5,
                    4,
                    6,
                    6,
                    4
                };

                Assert.Equal(data, result.Random.Data);
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
                    () => client.GenerateDecimalFractionsAsync(count, decimalPlaces, false));
            }
        }

        [Fact]
        public async void GenerateDecimalFractions()
        {
            var stubHandler = (Func<string, Task<string>>)
                (requestContent => HandleRequestContent(requestContent, "Assets.gen_pur_dfr_req.json", "Assets.gen_pur_dfr_res.json"));

            var httpClient = new HttpClient(new HttpMessageHandlerStub(stubHandler, _HTTP_MEDIA_TYPE));

            using (var client = new RandomOrgClient(_RANDOM_API_KEY, httpClient))
            {
                var result = await client.GenerateDecimalFractionsAsync(10, 8, true);

                Assert.NotNull(result);
                Assert.Equal(266, result.BitsUsed);
                Assert.Equal(199734, result.BitsLeft);
                Assert.Equal(8463, result.RequestsLeft);
                Assert.NotNull(result.Random);
                Assert.Equal(new DateTime(2013, 01, 25, 19, 16, 42, 00, DateTimeKind.Utc), result.Random.CompletionTime);
                Assert.NotNull(result.Random.Data);

                var data = new[]
                {
                    0.07532050m,
                    0.59823072m,
                    0.46109946m,
                    0.28453638m,
                    0.92390558m,
                    0.53087566m,
                    0.48139983m,
                    0.06829921m,
                    0.18780000m,
                    0.10107864m
                };

                Assert.Equal(data, result.Random.Data);
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
                    () => client.GenerateGaussiansAsync(count, meanValue, standardDeviationValue, significantDigits));
            }
        }

        [Fact]
        public async void GenerateGaussians()
        {
            var stubHandler = (Func<string, Task<string>>)
                (requestContent => HandleRequestContent(requestContent, "Assets.gen_pur_gss_req.json", "Assets.gen_pur_gss_res.json"));

            var httpClient = new HttpClient(new HttpMessageHandlerStub(stubHandler, _HTTP_MEDIA_TYPE));

            using (var client = new RandomOrgClient(_RANDOM_API_KEY, httpClient))
            {
                var result = await client.GenerateGaussiansAsync(4, 0.0m, 1.0m, 8);

                Assert.NotNull(result);
                Assert.Equal(106, result.BitsUsed);
                Assert.Equal(199894, result.BitsLeft);
                Assert.Equal(5442, result.RequestsLeft);
                Assert.NotNull(result.Random);
                Assert.Equal(new DateTime(2013, 01, 25, 19, 16, 42, 00, DateTimeKind.Utc), result.Random.CompletionTime);
                Assert.NotNull(result.Random.Data);

                var data = new[]
                {
                    +0.40250410m,
                    -1.49188310m,
                    +0.64733849m,
                    +0.52222420m
                };

                Assert.Equal(data, result.Random.Data);
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
                    () => client.GenerateStringsAsync(count, length, "abcde", false));
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
                    () => client.GenerateStringsAsync(1, 1, null, false));
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
                    () => client.GenerateStringsAsync(1, 1, characters, false));
            }
        }

        [Fact]
        public async void GenerateStrings()
        {
            var stubHandler = (Func<string, Task<string>>)
                (requestContent => HandleRequestContent(requestContent, "Assets.gen_pur_str_req.json", "Assets.gen_pur_str_res.json"));

            var httpClient = new HttpClient(new HttpMessageHandlerStub(stubHandler, _HTTP_MEDIA_TYPE));

            using (var client = new RandomOrgClient(_RANDOM_API_KEY, httpClient))
            {
                var result = await client.GenerateStringsAsync(8, 10, "abcdefghijklmnopqrstuvwxyz", true);

                Assert.NotNull(result);
                Assert.Equal(376, result.BitsUsed);
                Assert.Equal(199624, result.BitsLeft);
                Assert.Equal(9999, result.RequestsLeft);
                Assert.NotNull(result.Random);
                Assert.Equal(new DateTime(2011, 10, 10, 13, 19, 12, 00, DateTimeKind.Utc), result.Random.CompletionTime);
                Assert.NotNull(result.Random.Data);

                var data = new[]
                {
                    "grvhglvahj",
                    "hjrmosjwed",
                    "nivjyqptyy",
                    "lhogeshsmi",
                    "syilbgsytb",
                    "birvcmgdrz",
                    "wgclyynpcq",
                    "eujwnhgonh"
                };

                Assert.Equal(data, result.Random.Data);
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
                    () => client.GenerateUuidsAsync(count));
            }
        }

        [Fact]
        public async void GenerateUuids()
        {
            var stubHandler = (Func<string, Task<string>>)
                (requestContent => HandleRequestContent(requestContent, "Assets.gen_pur_uid_req.json", "Assets.gen_pur_uid_res.json"));

            var httpClient = new HttpClient(new HttpMessageHandlerStub(stubHandler, _HTTP_MEDIA_TYPE));

            using (var client = new RandomOrgClient(_RANDOM_API_KEY, httpClient))
            {
                var result = await client.GenerateUuidsAsync(1);

                Assert.NotNull(result);
                Assert.Equal(122, result.BitsUsed);
                Assert.Equal(998532, result.BitsLeft);
                Assert.Equal(199996, result.RequestsLeft);
                Assert.NotNull(result.Random);
                Assert.Equal(new DateTime(2013, 02, 11, 16, 42, 07, 00, DateTimeKind.Utc), result.Random.CompletionTime);
                Assert.NotNull(result.Random.Data);

                var data = new[]
                {
                    new Guid("47849fd4-b790-492e-8b93-c601a91b662d")
                };

                Assert.Equal(data, result.Random.Data);
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
                    () => client.GenerateBlobsAsync(count, size));
            }
        }

        [Fact]
        public async void GenerateBlobs()
        {
            var stubHandler = (Func<string, Task<string>>)
                (requestContent => HandleRequestContent(requestContent, "Assets.gen_pur_blb_req.json", "Assets.gen_pur_blb_res.json"));

            var httpClient = new HttpClient(new HttpMessageHandlerStub(stubHandler, _HTTP_MEDIA_TYPE));

            using (var client = new RandomOrgClient(_RANDOM_API_KEY, httpClient))
            {
                var result = await client.GenerateBlobsAsync(1, 1024);

                Assert.NotNull(result);
                Assert.Equal(1024, result.BitsUsed);
                Assert.Equal(198976, result.BitsLeft);
                Assert.Equal(9999, result.RequestsLeft);
                Assert.NotNull(result.Random);
                Assert.Equal(new DateTime(2011, 10, 10, 13, 19, 12, 00, DateTimeKind.Utc), result.Random.CompletionTime);
                Assert.NotNull(result.Random.Data);

                var data = new[]
                {
                    Convert.FromBase64String(
                       "aNB8L3hY3kWYXgTUQxGVB5njMe2e0l3LCjkDCN1u12kPBPrsDcWMLTCDlB60kRhAlGbvPqoBHhjg6ZbOM4LfD3T9/wfhvnqJ1FTr" +
                       "aamW2IAUnyKxz27fgcPw1So6ToIBL0fGQLpMQDF2/nEmNmFRNa9s6sQ+400IGA+ZeaOAgjE=")
                };

                Assert.Equal(data, result.Random.Data);
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
                    () => client.GetUsageAsync());
            }
        }

        [Fact]
        public async void GetUsage()
        {
            var stubHandler = (Func<string, Task<string>>)
                (requestContent => HandleRequestContent(requestContent, "Assets.get_usg_req.json", "Assets.get_usg_res.json"));

            var httpClient = new HttpClient(new HttpMessageHandlerStub(stubHandler, _HTTP_MEDIA_TYPE));

            using (var client = new RandomOrgClient(_RANDOM_API_KEY, httpClient))
            {
                var result = await client.GetUsageAsync();

                Assert.NotNull(result);
                Assert.Equal(ApiKeyStatus.Running, result.Status);
                Assert.Equal(new DateTime(2013, 02, 01, 17, 53, 40, 00, DateTimeKind.Utc), result.CreationTime);
                Assert.Equal(998532, result.BitsLeft);
                Assert.Equal(199996, result.RequestsLeft);
                Assert.Equal(1646421, result.TotalBits);
                Assert.Equal(65036, result.TotalRequests);
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
                    () => client.GenerateSignedIntegersAsync(count, minimum, maximum, false));
            }
        }

        [Fact]
        public async void GenerateSignedIntegers()
        {
            var stubHandler = (Func<string, Task<string>>)
                (requestContent => HandleRequestContent(requestContent, "Assets.gen_sig_int_req.json", "Assets.gen_sig_int_res.json"));

            var httpClient = new HttpClient(new HttpMessageHandlerStub(stubHandler, _HTTP_MEDIA_TYPE));

            using (var client = new RandomOrgClient(_RANDOM_API_KEY, httpClient))
            {
                var result = await client.GenerateSignedIntegersAsync(6, 1, 6, true);

                Assert.NotNull(result);

                var apiKeyHash = Convert.FromBase64String(
                       "oT3AdLMVZKajz0pgW/8Z+t5sGZkqQSOnAi1aB8Li0tXgWf8LolrgdQ1wn9sKx1ehxhUZmhwUIpAtM8QeRbn51Q==");

                Assert.Equal(apiKeyHash, result.Random.ApiKeyHash);
                Assert.Equal(1, result.Random.Minimum);
                Assert.Equal(6, result.Random.Maximum);
                Assert.True(result.Random.Replacement);
                Assert.Equal(69260, result.Random.SerialNumber);
                Assert.Equal(16, result.BitsUsed);
                Assert.Equal(932400, result.BitsLeft);
                Assert.Equal(199991, result.RequestsLeft);
                Assert.NotNull(result.Random);
                Assert.Equal(new DateTime(2013, 09, 30, 14, 58, 03, 00, DateTimeKind.Utc), result.Random.CompletionTime);
                Assert.NotNull(result.Random.Data);

                var data = new[]
                {
                    2,
                    4,
                    4,
                    1,
                    5,
                    3
                };

                Assert.Equal(data, result.Random.Data);

                var signature = Convert.FromBase64String(
                    "BxHxajeRg7Q+XGjBdFS1c7wkZbJgJlverfZ5TVDyzCKqo2K5A4pD+54EMqmysRYwkL3w2NS2DFLVrsyO1o96bW9BGp5zjjrEegz9" +
                    "mB+04iOTaRwmdQnLJAj/m3WRptA+qzodPCTaqud8YWBifqWCM34q98XwjX+nlahyHVHT9vf5KO0YVkD/yRI1WN5M/qX21chVvSxh" +
                    "WdmIrdCkrovGnysFq8SzCRNhpYx+/1P+YT2IKsH8jth9z82IAz1ANVh918H/UdpuD1dR7TD6nk3ntRgGrIiu2qqVzFi8A7/6viVg" +
                    "RqtffE4KVZY6O9mUJ+sGkF5Ohayms7LHSFy1VC8wMbMgwod+A8nr5yzjAC4SCUkT1bKAyWNF3SdVcLtvWdcf97Ew6RjohzCW4Vs3" +
                    "jUlh6jF/pj3b3++U3lBHCh43IIonw8MQ7afwpqP12yvyDym1isNjhMKYjmzWRerSvnsMyQIH8xFW7IHt2g/0qnzJgABFmUNBRKJP" +
                    "CD9CMgjh60sSwW7EyrGMy7/qisfE0IU74P/F7KCty/g1jIlXX5/O1lQjwY34wnoP0NXL08QteukRZZUfJQnscx1NGE+HX1c9bMBI" +
                    "8LC0ZFYFk+uY6ib/0rCV5OcLLE9PihCdC8WoI1x3bobr8tbtfgnXMTjogxwVXiiSN1TMnTIWlJ+KM5eSWrw=");

                Assert.Equal(signature, result.Signature);
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
                    () => client.GenerateSignedDecimalFractionsAsync(count, decimalPlaces, false));
            }
        }

        [Fact]
        public async void GenerateSignedDecimalFractions()
        {
            var stubHandler = (Func<string, Task<string>>)
                (requestContent => HandleRequestContent(requestContent, "Assets.gen_sig_dfr_req.json", "Assets.gen_sig_dfr_res.json"));

            var httpClient = new HttpClient(new HttpMessageHandlerStub(stubHandler, _HTTP_MEDIA_TYPE));

            using (var client = new RandomOrgClient(_RANDOM_API_KEY, httpClient))
            {
                var result = await client.GenerateSignedDecimalFractionsAsync(10, 8, true);

                Assert.NotNull(result);

                var apiKeyHash = Convert.FromBase64String(
                       "oT3AdLMVZKajz0pgW/8Z+t5sGZkqQSOnAi1aB8Li0tXgWf8LolrgdQ1wn9sKx1ehxhUZmhwUIpAtM8QeRbn51Q==");

                Assert.Equal(apiKeyHash, result.Random.ApiKeyHash);
                Assert.Equal(8, result.Random.DecimalPlaces);
                Assert.True(result.Random.Replacement);
                Assert.Equal(69259, result.Random.SerialNumber);
                Assert.Equal(266, result.BitsUsed);
                Assert.Equal(932416, result.BitsLeft);
                Assert.Equal(199992, result.RequestsLeft);
                Assert.NotNull(result.Random);
                Assert.Equal(new DateTime(2013, 09, 30, 14, 54, 11, 00, DateTimeKind.Utc), result.Random.CompletionTime);
                Assert.NotNull(result.Random.Data);

                var data = new[]
                {
                    0.95500371m,
                    0.65189604m,
                    0.10816501m,
                    0.74836463m,
                    0.55116651m,
                    0.62433960m,
                    0.17433560m,
                    0.60179234m,
                    0.26488912m,
                    0.42426186m
                };

                Assert.Equal(data, result.Random.Data);

                var signature = Convert.FromBase64String(
                    "ENJ8vaXRW6OnZSSnML16OaZC4ROYAU+jFsjcVFg41Y6nAR6p9/wYde9GmV8OBgI1IkocyJbuWqTiD0y8mm/RQRNNXLXhVWwe8MuF" +
                    "p6CNR9N7drJMMY3/51PxmRYc9ottUFakn/JxCE2a3kwyYaD5y50WuzqTWgA1yqxRuudcWQMl4WrvIcqc8LWAqgAMUfCn/va3xQ74" +
                    "9CzI9gpiemzbWcfEUjU56D4Kv7hJtUTN4WsVCD2TWGK5kUpmiz+R1tVfNQkgsmeFMA5J8KFAv3kVMdK09EVfCn+Y1bWEotQvkQcp" +
                    "epD6Lm/ZlZhriDyutv8mqSftecTVQI8VyRFilkReI0Wb7URJgS5kaYFkXwP4lVlFvvFMJCvTZ5MNZvu9boh0tYwqGQW4mZPNcaGm" +
                    "1BA5IShlPFz71K8SeyQAhKSxhYoK8/0zlcT0or7uJb7hcIBcErThoUqx1VzwbSLP3sr83usLhw4pqVxXQnxjV1HEVmo/csEVwM7l" +
                    "o6bBgN4ev0cUiBzdWzMKAv5MfjshgPyN1zUSmSkrkNgDxxhbiAkCZ6QagGTszVK0HxFG4qAhMPIwNihQNkfNalDewiodkShMWDXc" +
                    "bY8imqhgkfvGCXT7FdjTpmhtBHrOUInMOAEEnIurT78eNZBHDadjjwuoOogTAwXo3fLVJj5vg/ch4qGZAQ0=");

                Assert.Equal(signature, result.Signature);
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
                    () => client.GenerateSignedGaussiansAsync(count, meanValue, standardDeviationValue, significantDigits));
            }
        }

        [Fact]
        public async void GenerateSignedGaussians()
        {
            var stubHandler = (Func<string, Task<string>>)
                (requestContent => HandleRequestContent(requestContent, "Assets.gen_sig_gss_req.json", "Assets.gen_sig_gss_res.json"));

            var httpClient = new HttpClient(new HttpMessageHandlerStub(stubHandler, _HTTP_MEDIA_TYPE));

            using (var client = new RandomOrgClient(_RANDOM_API_KEY, httpClient))
            {
                var result = await client.GenerateSignedGaussiansAsync(4, 0.0m, 1.0m, 8);

                Assert.NotNull(result);

                var apiKeyHash = Convert.FromBase64String(
                       "oT3AdLMVZKajz0pgW/8Z+t5sGZkqQSOnAi1aB8Li0tXgWf8LolrgdQ1wn9sKx1ehxhUZmhwUIpAtM8QeRbn51Q==");

                Assert.Equal(apiKeyHash, result.Random.ApiKeyHash);
                Assert.Equal(0.0m, result.Random.Mean);
                Assert.Equal(1.0m, result.Random.StandardDeviation);
                Assert.Equal(8, result.Random.SignificantDigits);
                Assert.Equal(69263, result.Random.SerialNumber);
                Assert.Equal(106, result.BitsUsed);
                Assert.Equal(927902, result.BitsLeft);
                Assert.Equal(199988, result.RequestsLeft);
                Assert.NotNull(result.Random);
                Assert.Equal(new DateTime(2013, 09, 30, 16, 02, 19, 00, DateTimeKind.Utc), result.Random.CompletionTime);
                Assert.NotNull(result.Random.Data);

                var data = new[]
                {
                    +1.15038670m,
                    -0.71234425m,
                    +1.77860770m,
                    +0.14022009m
                };

                Assert.Equal(data, result.Random.Data);

                var signature = Convert.FromBase64String(
                    "hM5qlrWiB5OZlo6hQ8WsrJXVl8uJOTggaXeg/6RpzdEuAObxte6a40QVC0cs8aG7/UdUE7sVtx/IIdyR8A6pomiKGzu4DXMNaBes" +
                    "x6OdKKlLSkzgZcHWvTSey95OOjinzO4XCxp+ZP2j+TVUSvyNy1701u7Yq1pYmhzz5h64QlKzJZob9lFdabxR8H05EdjJugb6cgDF" +
                    "UMiTbzqUWukJa/oyrs1Y5ZgdgC0RiT9xgFWsnaizv+SJsvhwh5bvxo1hQOtrnKZ7bxF9WBghou2VaCT2gsjmViJwrX6xOPUO2wRP" +
                    "snYm+6o5cs/pZPwVkwumj7LUDm/jAvVslXbMcpOzOBvquR0uNtJT8jnF6Ev4dvOmrtjN38DRx0OmP4YDp4fj1+1Hk/jXCeXqsuTd" +
                    "0qdk6cFOqmyje4agjm1Qwewvl+R2rVtuoUB3FQNHd+fHbz3QmsxpL/kFRhsW7gu8Gc9w6ruc30AFUBRn+2Le9dFAHbnfsH1nP23w" +
                    "2AVBQONyaD5evkv0W3IuM6D9zwHCwlSkJBYfWr6AzwG0wNAkfHPRD5ZkKyEvAfl7ykqOagQVWGuXqZ3YUl1z10/c2ZTx3sMEqAqI" +
                    "ba/SQnMkweROcIbbLOXIUWwfAJh++mVsbRvKecffTBKzodssAMvBs9/9UUsmm8/7gXNoGMDfCX1xPlKkRw8=");

                Assert.Equal(signature, result.Signature);
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
                    () => client.GenerateSignedStringsAsync(count, length, "abcde", false));
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
                    () => client.GenerateSignedStringsAsync(1, 1, null, false));
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
                    () => client.GenerateSignedStringsAsync(1, 1, characters, false));
            }
        }

        [Fact]
        public async void GenerateSignedStrings()
        {
            var stubHandler = (Func<string, Task<string>>)
                (requestContent => HandleRequestContent(requestContent, "Assets.gen_sig_str_req.json", "Assets.gen_sig_str_res.json"));

            var httpClient = new HttpClient(new HttpMessageHandlerStub(stubHandler, _HTTP_MEDIA_TYPE));

            using (var client = new RandomOrgClient(_RANDOM_API_KEY, httpClient))
            {
                var result = await client.GenerateSignedStringsAsync(8, 10, "abcdefghijklmnopqrstuvwxyz", true);

                Assert.NotNull(result);

                var apiKeyHash = Convert.FromBase64String(
                       "oT3AdLMVZKajz0pgW/8Z+t5sGZkqQSOnAi1aB8Li0tXgWf8LolrgdQ1wn9sKx1ehxhUZmhwUIpAtM8QeRbn51Q==");

                Assert.Equal(apiKeyHash, result.Random.ApiKeyHash);
                Assert.Equal(10, result.Random.Length);
                Assert.Equal("abcdefghijklmnopqrstuvwxyz", result.Random.Characters);
                Assert.True(result.Random.Replacement);
                Assert.Equal(69266, result.Random.SerialNumber);
                Assert.Equal(376, result.BitsUsed);
                Assert.Equal(898958, result.BitsLeft);
                Assert.Equal(199985, result.RequestsLeft);
                Assert.NotNull(result.Random);
                Assert.Equal(new DateTime(2013, 09, 30, 16, 56, 46, 00, DateTimeKind.Utc), result.Random.CompletionTime);
                Assert.NotNull(result.Random.Data);

                var data = new[]
                {
                    "vnbbvnytjx",
                    "wazkxrindd",
                    "bdkyzpwggk",
                    "veanpfbyun",
                    "dbeswfjiak",
                    "gfgmrppopl",
                    "chwkiiwozg",
                    "xvdfnqdqdw"
                };

                Assert.Equal(data, result.Random.Data);

                var signature = Convert.FromBase64String(
                    "XFGHFilFS5jNvL8jmlvzvBogF8SQqFTGkeIUzs4NzzKGe4Qy7R8Uhl0Oi/gBuxBGIL94IjfXmPaEWD4gycuxOv+Yg5GEKlSVyeXY" +
                    "0D1AJF6E6e5HEhpCp0leo+rEQi2uR8tM21BVrGbAq9mWuJy8pkBa4CxQcvkyu2+4ct9Ci1PMTyr+ia+yo46rzW1s8cHXtIHUL4BO" +
                    "C6JZWahzknR+zzdY2sBQ2D+7FZkXo9J34HLtnoXmFkXMFU/yIGfUnfeEKlqBtNFk11C6JkWULX7cBdfMxOGaEyu+StRYnhjjixIx" +
                    "QvbJgPUG7T9L1+e/rP/iTYvuEZUViRhiJxLKkB85mlPSZYkclgZjV/90d0SR+ZciVJwkGdHvM+5vyN3XzoDAhZpDmAr3sp5uDD1o" +
                    "OA4KEKqqnZYiTQaHgVauWi9cXsGznoEZFB6AUtEWz5z+kvAayKwNP1//2Ob3KcELNZDOVgoo3uEJW1QjwyonmWoO2gVKC2vwcgyL" +
                    "nMDme21Nh8q+yRusSCveCOCVr3zWInW0K6Oy8CpvfpS5HReN5q7eYbN/mIKrdgYDro04jtjNCDVTx/+WzZEVbnN6Egi+3RpyPhI6" +
                    "t48//9sE9LELhnjf+D++JOo8Z52KckfXRHu+UFkBvvrEwyM68ivbkZiJMJYme4ZPt6UVFmv6yHy1O15bkNo=");

                Assert.Equal(signature, result.Signature);
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
                    () => client.GenerateSignedUuidsAsync(count));
            }
        }

        [Fact]
        public async void GenerateSignedUuids()
        {
            var stubHandler = (Func<string, Task<string>>)
                (requestContent => HandleRequestContent(requestContent, "Assets.gen_sig_uid_req.json", "Assets.gen_sig_uid_res.json"));

            var httpClient = new HttpClient(new HttpMessageHandlerStub(stubHandler, _HTTP_MEDIA_TYPE));

            using (var client = new RandomOrgClient(_RANDOM_API_KEY, httpClient))
            {
                var result = await client.GenerateSignedUuidsAsync(1);

                Assert.NotNull(result);

                var apiKeyHash = Convert.FromBase64String(
                       "oT3AdLMVZKajz0pgW/8Z+t5sGZkqQSOnAi1aB8Li0tXgWf8LolrgdQ1wn9sKx1ehxhUZmhwUIpAtM8QeRbn51Q==");

                Assert.Equal(apiKeyHash, result.Random.ApiKeyHash);
                Assert.Equal(69269, result.Random.SerialNumber);
                Assert.Equal(122, result.BitsUsed);
                Assert.Equal(897974, result.BitsLeft);
                Assert.Equal(199982, result.RequestsLeft);
                Assert.NotNull(result.Random);
                Assert.Equal(new DateTime(2013, 09, 30, 17, 24, 33, 00, DateTimeKind.Utc), result.Random.CompletionTime);
                Assert.NotNull(result.Random.Data);

                var data = new[]
                {
                    new Guid("9f7bc2c6-9673-4c0a-aef4-aa6dd9d62ec8")
                };

                Assert.Equal(data, result.Random.Data);

                var signature = Convert.FromBase64String(
                    "FvhPYF3jd+KLk6vxrvDIPU9xrkDhjv/WMOWbmBhKjYG6+vxGYFBY0s68DXHirF6mWpbaXLv27fw7GK32pzbUR7SmSDT9xmdwqedV" +
                    "MJzDXoX06MSh000k5j6gK2xV/1o84FxxLpsmOeOLCRQy0CeHax21E7SptJn90PLl3a1xu08b7FOU60AbktrQiBG3nxsCmLjfiBJ8" +
                    "qACowGcG34pq8FE1NYijnZO4h9q2TxySeHFPlJ69dnOUbXuWUniGxHctOqNY4cRrNo808Z61MuqgPteX62MtR9xslhLC107jc/yO" +
                    "YUiTVblTihABsfWPvGbt8+S7b9M92jn5vxN7eNUx1duEBtaKnV0MX/ycm2D4lqqxQ4cYuB8h7NCwJwFnC5M/n4QDVcNGN7I7uXyH" +
                    "tl+7kU4nThZ7g6PVSiEV1dVRusc2AdO+KEGukRZMz8QjD1SAEchf9URUp/TLJID75JcykzBckLkOwpI4DnVodgKhdetFc3HEQ7Bx" +
                    "M6FOyb7xuuzrq4BAMEqS2kvAo4SGHafk/8eilCCNkC3jfdWdHMikz1VZxJ6ykjdvjM2sMhfxSN+kNW93RuYtL0tDkkiTbfAw06vu" +
                    "IFTiHgKtiWpNYbVyZcAeVibTzmy6cyG29aYIj3Yyh1fRCuJPx1Js/ZW6vhRGFisSdF5Cakcw4EAzTdhDPxU=");

                Assert.Equal(signature, result.Signature);
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
                    () => client.GenerateSignedBlobsAsync(count, size));
            }
        }

        [Fact]
        public async void GenerateSignedBlobs()
        {
            var stubHandler = (Func<string, Task<string>>)
                (requestContent => HandleRequestContent(requestContent, "Assets.gen_sig_blb_req.json", "Assets.gen_sig_blb_res.json"));

            var httpClient = new HttpClient(new HttpMessageHandlerStub(stubHandler, _HTTP_MEDIA_TYPE));

            using (var client = new RandomOrgClient(_RANDOM_API_KEY, httpClient))
            {
                var result = await client.GenerateSignedBlobsAsync(1, 1024);

                Assert.NotNull(result);

                var apiKeyHash = Convert.FromBase64String(
                       "oT3AdLMVZKajz0pgW/8Z+t5sGZkqQSOnAi1aB8Li0tXgWf8LolrgdQ1wn9sKx1ehxhUZmhwUIpAtM8QeRbn51Q==");

                Assert.Equal(apiKeyHash, result.Random.ApiKeyHash);
                Assert.Equal(1024, result.Random.Size);
                Assert.Equal(69271, result.Random.SerialNumber);
                Assert.Equal(1024, result.BitsUsed);
                Assert.Equal(895974, result.BitsLeft);
                Assert.Equal(199980, result.RequestsLeft);
                Assert.NotNull(result.Random);
                Assert.Equal(new DateTime(2013, 09, 30, 17, 42, 32, 00, DateTimeKind.Utc), result.Random.CompletionTime);
                Assert.NotNull(result.Random.Data);

                var data = new[]
                {
                    Convert.FromBase64String(
                        "KchEoBw5W2J4l7UARgR2Ku6sTkiudQbHs/0hocw/8RahUaslx4EAHNZMoeN5P7jIakx+UFnLfuyjbijVySzoz/Ar1QC52rBQxTo" +
                        "3Ya+m3dCWpLmkN355UnD6nBbtieHlBd2qBUszXebneulhDJNrVuNBeX7qwpTwsXO+kD/Zxxc=")
                };

                Assert.Equal(data, result.Random.Data);

                var signature = Convert.FromBase64String(
                    "RP+b5vmSpQJK37t5BbVyJSxHB1qvv2eMQEZBNSWG2bH0j3etSNpi+LDw5af6pDDunomBcUhirQn0+utH11WXbrTnl8UhnKoNzHzU" +
                    "5fhpBTCFwf42Tenlp9bsrAMIhB/hmoSkTWMXiVcftAGWMVxuwdE2A2jLIPv1R0++EbzQqaPAA9nIdP74c4EchA44X/p3Dv2sHeaY" +
                    "ShY9wSHf8K7uI+ILnl7jssNwXmxvjZbEpUPvbldvkVO+IhUh6MEqPXJElNjRUjFAnoOXJe+uSMnqujE8jUPpsKLkkIJmeUmmnIOI" +
                    "Z1D063qCZoj1V7uKZq/GhV+JnZD6geo7sCQf1UW6hdwK50KbrVZetXUg0bxUz30dpY3WB5a/BJC0/+2QxEMq/p30YYTExRdMcdbD" +
                    "y8Nfgp/T1M5nkd9/5YTyulZCsacvvFNytrkS86kpmB++zoPwHPMKG6CDNY8JuMPbmJhMSvErhUHWY+1Au50uN7Qi0nGq5yltuLZI" +
                    "j/HEvFhAQx5iGV9fBZZ2Vz39Q9+DYFvswS6q7ahZb2LvEUxjgrgtcJV13SIEXg9SWB2VP156YFfOsNDjBSvnmnOiN3sH4WDShCyX" +
                    "iGeisIxOUeA2xs/cDAPwUoimBMOn1qnqD9NT9CGAeSdvX8q6qm+bU8dH2F9ZRRDIFojm7DRwgSnaECxomXw=");

                Assert.Equal(signature, result.Signature);
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
                    () => client.VerifySignatureAsync(default(SignedIntegersRandom), signature));
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
                    () => client.VerifySignatureAsync(random, null));
            }
        }

        [Fact]
        public async void VerifySignature()
        {
            var stubHandler = (Func<string, Task<string>>)
                (requestContent => HandleRequestContent(requestContent, "Assets.ver_sig_req.json", "Assets.ver_sig_res.json"));

            var httpClient = new HttpClient(new HttpMessageHandlerStub(stubHandler, _HTTP_MEDIA_TYPE));

            using (var client = new RandomOrgClient(httpClient))
            {
                var apiKeyHash = Convert.FromBase64String(
                    "oT3AdLMVZKajz0pgW/8Z+t5sGZkqQSOnAi1aB8Li0tXgWf8LolrgdQ1wn9sKx1ehxhUZmhwUIpAtM8QeRbn51Q==");

                var data = new[]
                {
                    3,
                    6,
                    4,
                    6,
                    1,
                    5
                };

                var random = new SignedIntegersRandom
                {
                    ApiKeyHash = apiKeyHash,
                    CompletionTime = new DateTime(2013, 07, 07, 09, 00, 43, 00, DateTimeKind.Utc),
                    SerialNumber = 69149,
                    Minimum = 1,
                    Maximum = 6,
                    Replacement = true,
                    Data = data
                };

                var signature = Convert.FromBase64String(
                    "Pd46xecjlt/EEOdNGFQNCFHVJhZ5lfVUFLziDusOpLIKGbbHh4kCRM8+el8xh3ASUgR7qfL+K7pzERsIIHIheiIt8EXq9Xr/DW3N" +
                    "3qfKL/4Cai+zyBGn4xThSJnXSJPvM+LuXI+B2dPx7GYJa2PSVgF+fV3j0eN+exAa4fjAQFGiPDQo+dCQO3cSO8aZ76tSGJpo/6b1" +
                    "rR/sdIt8uAWRsfL56tFA+2+xUtuXU5vM8HT+KO6o4N7TWpvBz1rw/0S9iiIRdIDJ5M/AjKXeXKMMkDBHUaeUWCDEZkZOMYTk6IvO" +
                    "pERlsWz0dz2xrs4NE8vZsi4nIVU5izGLYceMRf2TmZVC7XXdZDbeEnJnCmDacRBONqTYklJ22wGrXrFs8GElEoG3IqOgjS1ZME3/" +
                    "jBGW7G3eUB1xb0lT773V2YvGBPTqsnz3tawO8UDdIpoGjSQjtgql/j7gzNmyW6AC9wvrLyXeh7OP6rhp3SLNUAjNhr+QqjHrqPEv" +
                    "+QqkZspYCxTMVyRILCe+z+jTbNnTClVpgxIP5hSfLkinea/TpomZqWj6KwYPqKuiNMrEXsbWlAytzr1v1tsbHbESb2XKytUDWHqm" +
                    "BDCNxHphcgVTmn9+bi2+WPjueBBUH+b0ZOvHIZNxld9P76x8IlcOVKRLl770j0yD3ActIZNaHvUNYeBoK2M=");

                var result = await client.VerifySignatureAsync(random, signature);

                Assert.True(result);
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
                    () => client.GetUsageAsync());
            }
        }

        [Fact]
        public async void InvokeMethodWhenMediaTypeInvalid()
        {
            var stubHandler = (Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>>)
                (
                    (requestMessage, cancellationToken) =>
                    {
                        var responseContent = EmbeddedResourceManager.GetString("Assets.get_usg_res.json");

                        var responseMessage = new HttpResponseMessage
                        {
                            RequestMessage = requestMessage,
                            StatusCode = HttpStatusCode.OK,
                            Content = new StringContent(responseContent, Encoding.UTF8, "application/octet-stream"),
                            Version = new Version(1, 1)
                        };

                        return Task.FromResult(responseMessage);
                    }
                );

            var httpClient = new HttpClient(new HttpMessageHandlerStub(stubHandler));

            using (var client = new RandomOrgClient(_RANDOM_API_KEY, httpClient))
            {
                await Assert.ThrowsAsync<InvalidOperationException>(
                    () => client.GetUsageAsync());
            }
        }

        [Fact]
        public async void RandomOrgErrorWithNoData()
        {
            var stubHandler = (Func<string, Task<string>>)
                (requestContent => HandleRequestContent(requestContent, "Assets.get_usg_req_101.json", "Assets.get_usg_res_101.json"));

            var httpClient = new HttpClient(new HttpMessageHandlerStub(stubHandler, _HTTP_MEDIA_TYPE));

            using (var client = new RandomOrgClient(_RANDOM_API_KEY, httpClient))
            {
                var exception = await Assert.ThrowsAsync<RandomOrgException>(
                    () => client.GetUsageAsync());

                Assert.Equal(101, exception.Code);
            }
        }

        [Fact]
        public async void RandomOrgErrorWithData()
        {
            var stubHandler = (Func<string, Task<string>>)
                (requestContent => HandleRequestContent(requestContent, "Assets.get_usg_req_501.json", "Assets.get_usg_res_501.json"));

            var httpClient = new HttpClient(new HttpMessageHandlerStub(stubHandler, _HTTP_MEDIA_TYPE));

            using (var client = new RandomOrgClient(_RANDOM_API_KEY, httpClient))
            {
                var exception = await Assert.ThrowsAsync<RandomOrgException>(
                    () => client.GetUsageAsync());

                Assert.Equal(501, exception.Code);
            }
        }
    }
}